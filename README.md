# Lapin, RabbitMQ client for FSharp

Lapin is a young RabbitMQ client library for F#.
It builds on top of the [RabbitMQ .NET client](http://www.rabbitmq.com/dotnet.html)
and provides a number of API refinements:

 * More obvious and type safe API
 * Significantly less need to use direct C# interoperability
 * Convenience functions where appropriate
 * Functions compose better


## Project Maturity

Lapin is very immature, somewhat incomplete, and terribly
underdocumented.


## NuGet Packages

TBD


## Documentation

TBD


## Contributing and Building from Source

Lapin requires Visual Studio 2013 (Community Edition is OK) or later,
.NET 4.5, and F# 3.1 or later.

To build the project from the command line, run `msbuild.exe` or `xbuild` (on Mono).

To run tests, make sure that RabbitMQ is running on localhost (stock defaults are OK)
and run

    msbuild.exe /t:Test RabbitMQ.Lapin.Tests/Lapin.Tests.fsproj

Contributions are accepted via pull requests on GitHub.


## License

Released under the Apache Software License 2.0.
