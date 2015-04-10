namespace Lapin

open System

open NUnit.Framework
open FsUnit

open Lapin.Core
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open RabbitMQ.Client
open Lapin.Tests

module ExchangeTests =

    [<TestFixture>]
    type ExchangeTests () =
        inherit IntegrationTest()

        member t.DeclareArgs = {name = t.defaultFanout;
                                ``type`` = Fanout;
                                durable = false;
                                autoDelete = false;
                                arguments = None}

        [<Test>]
        member t.``exchange.declare with fanout type and default attributes``() =
            t.WithChannel(fun conn ch ->
                Lapin.Exchange.delete(ch, t.defaultFanout, None)
                Lapin.Exchange.assertExistence(conn, t.defaultFanout) |> should equal false
                Lapin.Exchange.declare(ch, t.DeclareArgs)
                Lapin.Exchange.assertExistence(conn, t.defaultFanout) |> should equal true)

        [<Test>]
        member t.``assertExistence when exchange exists`` () =
            t.WithChannel(fun conn ch ->
                let s    = "lapin.tests.topic"
                Lapin.Exchange.declare(ch, {t.DeclareArgs with name = s; ``type`` = Topic}) |> ignore
                Lapin.Exchange.assertExistence(conn, s) |> should equal true
                Lapin.Exchange.delete(ch, s, None))

        [<Test>]
        member t.``assertExistence when queue DOES NOT exist`` () =
            t.WithChannel(fun conn ch ->
                let s    = Guid.NewGuid().ToString()
                Lapin.Exchange.delete(ch, s, None)
                Lapin.Exchange.assertExistence(conn, s) |> should equal false)

        [<Test>]
        member t.``deleteExchange when queue DOES NOT exist is a no-op`` () =
            t.WithChannel(fun _ ch ->
                let s    = Guid.NewGuid().ToString()
                Lapin.Exchange.delete(ch, s, None))

        [<Test>]
        member t.``deleteExchange when queue exists`` () =
            t.WithChannel(fun conn ch ->
                let s    = "lapin.tests.topic"
                Lapin.Exchange.declare(ch, {t.DeclareArgs with name = s; ``type`` = Topic}) |> ignore
                Lapin.Exchange.assertExistence(conn, s) |> should equal true
                Lapin.Exchange.delete(ch, s, None)
                Lapin.Exchange.assertExistence(conn, s) |> should equal false)