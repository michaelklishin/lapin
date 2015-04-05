namespace Lapin

open System
open System.Text
open NUnit.Framework
open FsUnit
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open Lapin.Basic

module PublishingTests =
    [<TestFixture>]
    type PublishingTests() =
        let defaultFanout = "lapin.tests.fanout"
        let enc = Encoding.UTF8
        let defaultTimespan = TimeSpan.FromSeconds(5.0)

        member t.ExchangeDeclareArgs =
            { name = defaultFanout
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
            Lapin.Queue.bind(ch, { exchange = defaultFanout
                                   queue = q
                                   routingKey = ""
                                   arguments = None })
            q

        [<Test>]
        member t.``basic.publish to an existing exchange without metadata``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            Lapin.Basic.publish(ch,
                                { exchange = defaultFanout
                                  routingKey = "" }, enc.GetBytes("msg"), None)

        [<Test>]
        member t.``basic.publish with mandatory = true to an existing exchange with a queue bound``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn) |> enablePublisherConfirms
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            let q    = t.declareTemporaryQueueBoundToDefaultFanout(ch)
            Lapin.Queue.messageCount(ch, q) |> should equal 0
            Lapin.Basic.publish(ch, { exchange = defaultFanout
                                      routingKey = "" }, enc.GetBytes("msg"),
                                Some { mandatory = true
                                       properties = None })
            Lapin.Queue.messageCount(ch, q) |> should equal 1
            Lapin.Queue.delete(ch, q, None)

        [<Test>]
        member t.``basic.publish followed by basic.get with automatic ack``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn) |> enablePublisherConfirms
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            let q    = t.declareTemporaryQueueBoundToDefaultFanout(ch)
            let body = enc.GetBytes("msg")
            Lapin.Basic.publish(ch, { exchange = defaultFanout
                                      routingKey = "" }, body,
                                None)
            Lapin.Channel.waitForConfirms(ch, defaultTimespan) |> should equal true
            let resp = Lapin.Basic.getAutoAck(ch, q)
            resp.body |> should equal body
            resp.routingContext.routingKey |> should equal ""
            resp.redelivered |> should equal false
            Lapin.Queue.delete(ch, q, None)