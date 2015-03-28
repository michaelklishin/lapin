namespace Lapin

open System

open NUnit.Framework
open FsUnit

open Lapin.Core
open Lapin.Channel
open Lapin.Queue
open RabbitMQ.Client

module QueueTests =

    [<TestFixture>]
    type QueueTests () =
        member t.DeclareArgs = {name = Lapin.Queue.ServerGenerated;
                                durable = false;
                                exclusive = false;
                                autoDelete = false;
                                arguments = None}

        [<Test>]
        member t.``queue.declare with server-generated name and default attributes``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let ok   = Lapin.Queue.declare(ch, {t.DeclareArgs with name = Lapin.Queue.ServerGenerated})
            ok.queueName.StartsWith("amq.") |> should equal true

        [<Test>]
        member t.``queue.declare with client-provided name``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = "lapin.tests."
            let ok   = Lapin.Queue.declare(ch, {t.DeclareArgs with name = s})
            ok.queueName |> should equal s
            ok.messageCount |> should equal 0
            ok.consumerCount |> should equal 0
            Lapin.Queue.delete(ch, s, None)

        [<Test>]
        member t.``assertExistence when queue exists`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = "lapin.tests."
            Lapin.Queue.declare(ch, {t.DeclareArgs with name = s}) |> ignore
            Lapin.Queue.assertExistence(conn, s) |> should equal true
            Lapin.Queue.delete(ch, s, None)

        [<Test>]
        member t.``assertExistence when queue DOES NOT exist`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = Guid.NewGuid().ToString()
            Lapin.Queue.delete(ch, s, None)
            Lapin.Queue.assertExistence(conn, s) |> should equal false

        [<Test>]
        member t.``deleteQueue when queue DOES NOT exist is a no-op`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = Guid.NewGuid().ToString()
            Lapin.Queue.delete(ch, s, None)

        [<Test>]
        member t.``deleteQueue when queue exists`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let s    = "lapin.tests."
            Lapin.Queue.declare(ch, {t.DeclareArgs with name = s}) |> ignore
            Lapin.Queue.assertExistence(conn, s) |> should equal true
            Lapin.Queue.delete(ch, s, None)
            Lapin.Queue.assertExistence(conn, s) |> should equal false