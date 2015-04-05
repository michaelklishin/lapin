namespace Lapin

open System
open System.Text
open NUnit.Framework
open FsUnit
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open Lapin.Basic
open Lapin.Consumers
open System.Threading

module ConsumersTests =
    [<TestFixture>]
    type ConsumersTests() =
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
        member t.``basic.consume and defaultConsumerFrom``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn) |> enablePublisherConfirms
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            let q    = t.declareTemporaryQueueBoundToDefaultFanout(ch)
            let body = enc.GetBytes("msg")
            let latch = new ManualResetEvent(false)

            let fn    = fun ch tag envelope properties body -> latch.Set() |> ignore
            let cons  = Lapin.Consumers.defaultConsumerFrom(ch, {deliveryHandler  = fn;
                                                                 cancelHandler    = None;
                                                                 cancelOkHandler  = None;
                                                                 shudownHandler   = None;
                                                                 consumeOkHandler = None})
            Lapin.Basic.consumeAutoAck(ch, q, cons) |> ignore
            Lapin.Basic.publish(ch,
                                { exchange = defaultFanout
                                  routingKey = "" }, body, None)
            Lapin.Channel.waitForConfirms(ch, defaultTimespan) |> should equal true
            latch.WaitOne(defaultTimespan) |> should equal true