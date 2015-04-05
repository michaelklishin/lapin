namespace Lapin

open System
open System.Text
open NUnit.Framework
open FsUnit
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open Lapin.Basic
open Lapin.Tests

module PublishingTests =
    [<TestFixture>]
    type PublishingTests() =
        inherit IntegrationTest()

        [<Test>]
        member t.``basic.publish to an existing exchange without metadata``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            Lapin.Basic.publish(ch,
                                { exchange = t.defaultFanout
                                  routingKey = "" }, t.enc.GetBytes("msg"), None)

        [<Test>]
        member t.``basic.publish with mandatory = true to an existing exchange with a queue bound``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn) |> enablePublisherConfirms
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            let q    = t.declareTemporaryQueueBoundToDefaultFanout(ch)
            Lapin.Queue.messageCount(ch, q) |> should equal 0
            Lapin.Basic.publish(ch, { exchange = t.defaultFanout
                                      routingKey = "" }, t.enc.GetBytes("msg"),
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
            let body = t.enc.GetBytes("msg")
            Lapin.Basic.publish(ch, { exchange = t.defaultFanout
                                      routingKey = "" }, body,
                                None)
            Lapin.Channel.waitForConfirms(ch, t.defaultTimespan) |> should equal true
            let resp = Lapin.Basic.getAutoAck(ch, q)
            resp.body |> should equal body
            resp.routingContext.routingKey |> should equal ""
            resp.redelivered |> should equal false
            Lapin.Queue.delete(ch, q, None)