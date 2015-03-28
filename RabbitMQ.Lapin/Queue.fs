namespace Lapin

open RabbitMQ.Client
open Lapin.Core
open Lapin.Types
open Lapin.Channel

open System

module Queue =
    type QueueOptions = {
        name: Name
        durable: Durable
        exclusive: Exclusive
        autoDelete: AutoDelete
        arguments: Arguments
    }

    type DeclareOk = {
        queueName: Name
        messageCount: uint32
        consumerCount: uint32
    }

    let [<Literal>] ServerGenerated = Lapin.Core.ServerGenerated

    let declare(ch: IChannel, opts: QueueOptions): DeclareOk =
        let args = Lapin.Types.argumentsToIDictionary(opts.arguments)
        let ok   = ch.QueueDeclare(opts.name, opts.durable, opts.exclusive, opts.autoDelete, args)
        {queueName     = ok.QueueName;
         messageCount  = ok.MessageCount;
         consumerCount = ok.ConsumerCount}