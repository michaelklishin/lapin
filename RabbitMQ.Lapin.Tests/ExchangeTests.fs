namespace Lapin

open System

open NUnit.Framework
open FsUnit

open Lapin.Core
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open RabbitMQ.Client

module ExchangeTests =

    [<TestFixture>]
    type ExchangeTests () =
        let defaultFanout = "lapin.tests.fanout"

        member t.DeclareArgs = {name = defaultFanout;
                                ``type`` = Fanout;
                                durable = false;
                                autoDelete = false;
                                arguments = None}

        [<Test>]
        member t.``exchange.declare with fanout type and default attributes``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            Lapin.Exchange.delete(ch, defaultFanout, None)
            Lapin.Exchange.assertExistence(conn, defaultFanout) |> should equal false
            Lapin.Exchange.declare(ch, t.DeclareArgs)
            Lapin.Exchange.assertExistence(conn, defaultFanout) |> should equal true

        [<Test>]
        member t.``assertExistence when exchange exists`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = "lapin.tests.topic"
            Lapin.Exchange.declare(ch, {t.DeclareArgs with name = s; ``type`` = Topic}) |> ignore
            Lapin.Exchange.assertExistence(conn, s) |> should equal true
            Lapin.Exchange.delete(ch, s, None)

        [<Test>]
        member t.``assertExistence when queue DOES NOT exist`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = Guid.NewGuid().ToString()
            Lapin.Exchange.delete(ch, s, None)
            Lapin.Exchange.assertExistence(conn, s) |> should equal false

        [<Test>]
        member t.``deleteExchange when queue DOES NOT exist is a no-op`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = Guid.NewGuid().ToString()
            Lapin.Exchange.delete(ch, s, None)

        [<Test>]
        member t.``deleteExchange when queue exists`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = "lapin.tests.topic"
            Lapin.Exchange.declare(ch, {t.DeclareArgs with name = s; ``type`` = Topic}) |> ignore
            Lapin.Exchange.assertExistence(conn, s) |> should equal true
            Lapin.Exchange.delete(ch, s, None)
            Lapin.Exchange.assertExistence(conn, s) |> should equal false