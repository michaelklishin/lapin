namespace Lapin

open System
open System.Text

open NUnit.Framework
open FsUnit

open Lapin.Core
open Lapin.Channel
open Lapin.Queue
open Lapin.Exchange
open Lapin.Basic
open RabbitMQ.Client

module PublishingTests =
    [<TestFixture>]
    type ExchangeTests () =
        let defaultFanout = "lapin.tests.fanout"
        let enc = Encoding.UTF8

        member t.ExchangeDeclareArgs = {name = defaultFanout;
                                       ``type`` = Fanout;
                                        durable = false;
                                        autoDelete = false;
                                        arguments = None}

        [<Test>]
        member t.``basic.publish to an existing exchange without metadata``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            Lapin.Exchange.declare(ch, t.ExchangeDeclareArgs)
            Lapin.Basic.publish(ch, {exchange = defaultFanout; routingKey = ""}, enc.GetBytes("msg"), None)