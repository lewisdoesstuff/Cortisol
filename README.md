# Cortisol
![Build](https://github.com/lewisdoesstuff/Stress-for-Windows/actions/workflows/dotnet.yml/badge.svg)  

A .NET cross-platform CPU stress testing program for the command line.

## Description

Cortisol (StressforWindows) is a CPU stress testing tool inspired by the Linux tool `stress`. 
The program has 2 stress tests, a prime number search similar to Prime95, and a usage % configurable resource consumption (using Stopwatch()) test.


## Getting Started

### Dependencies

* .NET 6.0 Runtime(SDK to build)

### Build

* Build using Visual Studio (or other .NET IDE)
* Build using `dotnet build` 

### Executing program

* Run `Stress.exe` from the command line.

#### Arguments

```
  -u, --usage      Set the desired CPU usage% for the test. Does not apply to the Mersenne Prime test.

  -t, --threads    Set the desired amount of threads for the test to run on. 0 will run on all available threads.

  -d, --time       Set the desired time for the test to run. 0 will run until cancelled.

  --help           Display this help screen.

  --version        Display version information.
```


## Version History

* No versions published.

## License

This project is licensed under the Apache 2.0 License - see the LICENSE.md file for details

