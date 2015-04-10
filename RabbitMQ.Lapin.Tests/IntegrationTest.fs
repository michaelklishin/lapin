namespace Lapin

open System
open System.Text
open NUnit.Framework
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open Lapin.Channel
open RabbitMQ.Client

module Tests =
    [<TestFixture>]
    type IntegrationTest() =
        member t.defaultFanout = "lapin.tests.fanout"
        member t.enc = Encoding.UTF8
        member t.defaultTimespan = TimeSpan.FromSeconds(5.0)

        member t.ExchangeDeclareArgs =
            { name = t.defaultFanout
              ``type`` = Fanout
              durable = false
              autoDelete = false
              arguments = None }

        member t.declareTemporaryQueueBoundToDefaultFanout(ch: IChannel): Lapin.Types.Name =
            let q = Guid.NewGuid().ToString()
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            Lapin.Queue.declare(ch, { name = q
                                      durable = false
                                      exclusive = false
                                      autoDelete = false
                                      arguments = None })
            |> ignore
            Lapin.Queue.bind(ch, { exchange = t.defaultFanout
                                   queue = q
                                   routingKey = ""
                                   arguments = None })
            q

        member t.WithChannel(fn: IConnection -> IChannel -> unit) =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            fn conn ch