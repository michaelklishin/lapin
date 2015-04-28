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

Connections can be closed with `Lapin.Core.close` and `Lapin.Core.abort`:

``` fsharp
open Lapin.Core

let conn = Lapin.Core.connectWithAllDefaults()
Lapin.Core.close conn

let conn2 = Lapin.Core.connectWithAllDefaults()
Lapin.Core.abort conn2
```

The difference between the two is in that `Lapin.Core.abort` will ignore any
exceptions that may be thrown.

Alternatively connections can be closed by calling `IConnection.Close` on them.

`Lapin.Core.isOpen` and `Lapin.Core.isClosed` are predicate functions
with pretty self-explanatory names.


### Opening and Closing Channels

Channels are opened with `Lapin.Core.open` and closed with `Lapin.Core.close`:

``` fsharp
open Lapin.Core
open Lapin.Channel

let conn = Lapin.Core.connectWithAllDefaults()
let ch   = Lapin.Channel.open conn

Lapin.Channel.close ch
Lapin.Core.close conn
```

`Lapin.Channel.isOpen` and `Lapin.Channel.isClosed` are predicate functions that...
you knew it!


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
