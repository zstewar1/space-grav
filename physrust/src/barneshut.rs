use crossbeam;
use nalgebra::{Vec2, Norm};
use num::{Zero};
use std::cmp::PartialEq;

pub type v2 = Vec2<f32>;

/// Theta is the distance/side ratio where quadtree calculations are allowed to stop.
pub static THETA: f32 = 1.0;

/// G is the constant of gravity.
pub static G: f32 = 1.0;

pub static NUM_THREADS: usize = 8;

/// BarnesHut encapsulates the calculations needed to run a step of the simulation.
pub struct BarnesHut {

    /// The center of the simulation.
    center: v2,
    /// The total width/height of the simulation.
    size: f32,
}

impl BarnesHut {

    /// Create a new Barnes-Hut base thingy.
    pub fn new(center: v2, size: f32) -> BarnesHut {
        BarnesHut {
            center: center,
            size: size,
        }
    }

    /// Calculate the forces acting between all of the given points.
    ///
    /// Returns an iterator with elements in the same order as the input iterator which is
    /// a vector of the forces experienced by each of the point masses.
    pub fn calculate_forces<'a, T: 'a + QuadtreePoint + Sync>(&self, points: &[T], result: &mut [v2]) -> Result<(), &'static str> {

        if points.len() != result.len() {
            return Err("Points and Result must be the same size.");
        }

        let mut root = Quadtree::new(self.center, self.size);

        for point in points.iter() {
            root.insert(point);
        }

        root.calculate_mass_distribution();

        // (x+y-1)/y is integer division with round-up
        crossbeam::scope(|scope| {
            // Immutably borrow root for the threads. The immutable reference is thread safe.
            let root = &root;

            let task_size = (points.len() + NUM_THREADS - 1) / NUM_THREADS;

            for (src, dst) in points.chunks(task_size).zip(result.chunks_mut(task_size)) {
                scope.spawn(move || {
                    for (inp, outp) in src.iter().zip(dst.iter_mut()) {
                        *outp = root.calculate_force(inp);
                    }
                });
            }
        });

        Ok(())
    }
}

/// Generic representation of a point in a quadtree.
pub trait QuadtreePoint : PartialEq {
    /// Where this point is.
    fn position(&self) -> v2;
    /// How much mass this point has.
    fn mass(&self) -> f32;
}

struct Quadtree<'a, T: 'a + QuadtreePoint> {
    /// The center of this quadrant.
    origin: v2,
    /// The size of this quadrant.
    width: f32,

    /// The net position of all masses in this quadrant.
    center_of_mass: v2,
    /// The total ammount of mass in this quadrant.
    mass: f32,

    /// The point held in this quadrant if it is a leaf.
    data: Option<&'a T>,

    /// Sub-quadrants of this quadrant, if it isn't a leaf.
    children: Vec<Quadtree<'a, T>>,
}

impl<'a, T: 'a + QuadtreePoint> Quadtree<'a, T> {

    /// Create a new quadtree with the specified width and heigh.
    fn new(origin: v2, width: f32) -> Quadtree<'a, T> {
        Quadtree::<'a, T> {
            origin: origin,
            width: width,
            center_of_mass: Zero::zero(),
            mass: 0.0,
            data: None,
            children: Vec::new(),
        }
    }

    /// Return the sub-quadrant of this quadrant that a given point lies in.
    fn get_point_quadrant(&self, point: v2) -> usize {
        // Set the second bit if the point is on the positive x axis, and the first bit if the
        // point is on the positive y axis.
        //
        // This gives us a setup where the quadrants are numbered as follows:
        //
        //  1 | 3   01 | 11
        //  --|-- = ---|---
        //  0 | 2   00 | 10
        //
        // (note: there *is* a more functional way to do this, but it's less readable IMO).
        let mut quad = 0;
        if point.x >= self.origin.x { quad |= 2; }
        if point.y >= self.origin.y { quad |= 1; }
        quad
    }

    /// Add the specified element to this quadrant.
    ///
    /// This assumes that the given point is actually *in* the quadrant. If this is ensured at the
    /// root level, then it will remain true recursively.
    fn insert(&mut self, elem: &'a T) {
        if self.children.len() > 0 {
            // Children exists, and children existing is always all or nothing.
            // Just insert the new data into the appropriate child.
            let quad = self.get_point_quadrant(elem.position());
            self.children[quad].insert(elem);
        } else {
            match self.data {
                // If no data already, just set the data to hold the element
                None => self.data = Some(elem),
                Some(existing) => {
                    // If there already is data, allocate the children and move the data to them.
                    self.children.reserve_exact(4);
                    for i in 0..4 {
                        let mut child_origin: v2 = self.origin;
                        child_origin.x += self.width * if i & 2 != 0 { 0.25 } else { -0.25 };
                        child_origin.y += self.width * if i & 1 != 0 { 0.25 } else { -0.25 };
                        self.children.push(Quadtree::new(child_origin, self.width * 0.5));
                    }

                    // Insert the existing element and remove it.
                    let quad = self.get_point_quadrant(existing.position());
                    self.children[quad].insert(existing);
                    self.data = None;

                    // Insert the new element.
                    let quad = self.get_point_quadrant(elem.position());
                    self.children[quad].insert(elem);
                }
            }
        }
    }

    /// Calculate the center of mass and total mass for every quadrant recursively.
    fn calculate_mass_distribution(&mut self) {
        if self.children.len() > 0 {
            // Calculate and add up all children
            for child in self.children.iter_mut() {
                child.calculate_mass_distribution();
                self.mass += child.mass;
                self.center_of_mass = self.center_of_mass + child.center_of_mass * child.mass;
            }
            self.center_of_mass = self.center_of_mass / self.mass;
        } else {
            match self.data {
                Some(dat) => {
                    self.mass = dat.mass();
                    self.center_of_mass = dat.position();
                },
                // If there isn't data in this leaf cell, mass and COM stay zero.
                None => (),
            }
        }
    }

    /// Calcualte the force vector that targ would experience given the masses in this quadtree.
    fn calculate_force(&self, targ: &T) -> v2 {
        if self.children.len() > 0 {
            // If there are children, we see if we can use this subtree as a point mass or if we
            // have to recurse more.
            let dir = self.center_of_mass - targ.position();
            let dist = dir.norm();
            if self.width / dist < THETA {
                // if the ratio is small enough, just treat this tree as a point mass.
                dir * G * self.mass * targ.mass() / (dist * dist * dist)
            } else {
                // Otherwise, sum the forces from all children.
                self.children.iter()
                .map(|child| child.calculate_force(targ))
                .fold(Zero::zero(), |acc, elem| acc + elem)
            }
        } else {
            match self.data {
                // If there is data, calculate the force between the two masses.
                Some(elem) => {
                    let dir = elem.position() - targ.position();
                    let dist = dir.norm();
                    dir * G * elem.mass() * targ.mass() / (dist * dist * dist)
                },
                // Otherwise the force is just zero.
                None => Zero::zero(),
            }
        }
    }
}
