namespace Lapin

open RabbitMQ.Client
open RabbitMQ.Client.Exceptions
open Lapin.Core
open Lapin.Types
open Lapin.Channel

open System

module Exchange =
    type ExchangeType = Fanout | Topic | Direct | Headers | ConsistentHash | Custom of string

    type ExchangeOptions = {
        name: Name
        ``type``: ExchangeType
        durable: Durable
        autoDelete: AutoDelete
        arguments: Arguments
    }

    let private stringifyExchangeType(input: ExchangeType) =
        match input with
        | Fanout   -> "fanout"
        | Topic    -> "topic"
        | Direct   -> "direct"
        | Headers  -> "headers"
        | ConsistentHash -> "x-consistent-hash"
        | Custom s -> s

    let declare(ch: IChannel, opts: ExchangeOptions): unit =
        let args = Lapin.Types.argumentsToIDictionary(opts.arguments)
        let et   = stringifyExchangeType(opts.``type``)
        ch.ExchangeDeclare(opts.name, et, opts.durable, opts.autoDelete, args)

    let assertExistence(conn: IConnection, s: Name) =
        try
            let tmpCh = Lapin.Channel.``open``(conn)
            tmpCh.ExchangeDeclarePassive(s) |> ignore
            true
        with
            | :? OperationInterruptedException -> false

    type DeleteOptions = {
        ifUnused: bool
    }

    let delete(ch: IChannel, exchange: Name, opts: Option<DeleteOptions>): unit =
        match opts with
        | Some del -> ch.ExchangeDelete(exchange, del.ifUnused) |> ignore
        | None     -> ch.ExchangeDelete(exchange)