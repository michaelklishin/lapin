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

Lapin API is organised in a number of modules:

 * `Lapin.Core`: connection functions
 * `Lapin.Channel`: operations on channels
 * `Lapin.Queue`: operations on queues
 * `Lapin.Exchange`: operations on exchanges
 * `Lapin.Basic`: `basic.*` protocol methods (publishing, acks, etc)
 * `Lapin.Consumers`: functions related to consumers


### Opening and Closing Connections

`Lapin.Core.connect` is a function that takes a `Lapin.Core.ConnectionOptions`
record with connection settings:

``` fsharp
open Lapin.Core

let conn = Lapin.Core.connect({hostname = Some "127.0.0.1"; port = Some 5672})
```

### Opening and Closing Channels

TBD

### Operations on Queues

TBD

### Consuming Messages

TBD

### Operations on Exchanges

TBD

### Publishing

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
