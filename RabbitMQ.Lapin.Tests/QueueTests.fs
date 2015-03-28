namespace Lapin

open NUnit.Framework
open FsUnit

open Lapin.Core
open Lapin.Channel
open Lapin.Queue
open RabbitMQ.Client

module QueueTests =

    [<TestFixture>]
    type QueueTests () =
        [<Test>]
        member t.``queue.declare with server-generated name and default attributes``() =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            let q    = Lapin.Queue.declare(ch, {name = Lapin.Queue.ServerGenerated;
                                                durable = false;
                                                exclusive = false;
                                                autoDelete = false;
                                                arguments = None})
            q.queueName.StartsWith("amq.") |> should equal true