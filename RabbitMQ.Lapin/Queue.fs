namespace Lapin

open RabbitMQ.Client
open RabbitMQ.Client.Exceptions
open Lapin.Types
open Lapin.Channel

module Queue =
    type QueueOptions =
        { name : Name
          durable : Durable
          exclusive : Exclusive
          autoDelete : AutoDelete
          arguments : Arguments }

    type DeclareOk =
        { queueName : Name
          messageCount : uint32
          consumerCount : uint32 }

    [<Literal>]
    let ServerGenerated = Lapin.Core.ServerGenerated

    let declare(ch: IChannel, opts : QueueOptions) : DeclareOk =
        let args = Lapin.Types.argumentsToIDictionary(opts.arguments)
        let ok = ch.QueueDeclare(opts.name, opts.durable, opts.exclusive, opts.autoDelete, args)
        { queueName = ok.QueueName
          messageCount = ok.MessageCount
          consumerCount = ok.ConsumerCount }

    let assertExistence(conn : IConnection, s : Name) =
        try
            let tmpCh = Lapin.Channel.``open``(conn)
            tmpCh.QueueDeclarePassive(s) |> ignore
            true
        with :? OperationInterruptedException -> false

    type DeleteOptions =
        { ifUnused : bool
          ifEmpty : bool }

    let delete(ch: IChannel, queue : Name, opts : Option<DeleteOptions>) : unit =
        match opts with
        | Some del -> ch.QueueDelete(queue, del.ifUnused, del.ifEmpty) |> ignore
        | None -> ch.QueueDelete(queue) |> ignore

    type BindOptions =
        { exchange : Name
          queue : Name
          routingKey : RoutingKey
          arguments : Arguments }

    let bind(ch: IChannel, binding : BindOptions) : unit =
        let args = Lapin.Types.argumentsToIDictionary(binding.arguments)
        ch.QueueBind(binding.queue, binding.exchange, binding.routingKey, args)

    let messageCount(ch: IChannel, queue: Name): uint32 =
        let ok = ch.QueueDeclarePassive(queue)
        ok.MessageCount