extern crate crossbeam;
extern crate nalgebra;
extern crate num;

macro_rules! stderr (
    ( $( $arg:tt )* ) => {{
        use std::io::Write;

        match writeln!(&mut ::std::io::stderr(), $($arg)* ) {
            Ok(_) => {},
            Err(x) => panic!("Unable to write to stderr: {}", x),
        }
    }}
);

macro_rules! desktop (
    ( $( $arg:tt )* ) => {{
        use std::fs::OpenOptions;
        use std::io::Write;

        let mut f = match OpenOptions::new()
            .create(true)
            .write(true)
            .truncate(false)
            .append(true)
            .open("C:\\Users\\ChirokidZ\\Desktop\\hellofromrust.txt") {
                Ok(f) => f,
                Err(x) => panic!("Unable to open for writing: {}", x),
            };

        match writeln!(f, $($arg)* ) {
            Ok(_) => (),
            Err(x) => panic!("Unable to write to desktop: {}", x),
        }
    }}
);

pub mod barneshut;
