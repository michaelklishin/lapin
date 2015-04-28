namespace Lapin

open System
open System.Collections.Generic
open RabbitMQ.Client

module Types =
    type Hostname   = string
    type Port       = int

    type Name       = string
    type Durable    = bool
    type Exclusive  = bool
    type AutoDelete = bool
    type Arguments  = Option<Map<string, Object>>

    type Mandatory  = bool
    type RoutingKey = string

    type ExchangeAndRoutingKey = {
        exchange: Name
        routingKey: RoutingKey
    }
    type Metadata = {
        mandatory: Mandatory
        properties: Option<IBasicProperties>
    }

    let argumentsToIDictionary(args: Arguments): IDictionary<string, Object> =
        // Map<string, Object> won't accept a null value which some C# methods expect
        match args with
        | Some m -> m |> Map.toSeq |> dict
        | None   -> null

    type ConsumerTag = string
    type DeliveryTag = uint64
    type Envelope = {
        deliveryTag: DeliveryTag
        redelivered: bool
        exchange: Name
        routingKey: RoutingKey
    }
    type Body = byte[]

    type ShutdownHandler = Object -> ShutdownEventArgs -> unit

module Core =
    let DefaultHostname = Some "127.0.0.1"
    let DefaultPort     = Some Protocols.AMQP_0_9_1.DefaultPort

    let [<Literal>] ServerGenerated = ""

    type ConnectionOptions = {
        hostname: Option<Types.Hostname>
        port: Option<Types.Port>
    }

    let defaultConnectionOptions: ConnectionOptions =
        {
            hostname = DefaultHostname;
            port = DefaultPort
        }

    let connect(opts: ConnectionOptions): IConnection =
        let h  = match opts.hostname with
                        | Some(h) -> h
                        | None    -> DefaultHostname.Value
        let p = match opts.port with
                        | Some(p) -> p
                        | None    -> DefaultPort.Value
        let cf = new ConnectionFactory(HostName = h, Port = p)

        cf.CreateConnection()

    let connectWithAllDefaults(): IConnection =
        connect(defaultConnectionOptions)

    let close(conn: IConnection): IConnection =
        conn.Close()
        conn

    let abort(conn: IConnection): IConnection =
        conn.Abort()
        conn

    let isOpen(conn: IConnection): bool =
        conn.IsOpen

    let isClosed(conn: IConnection): bool =
        not conn.IsOpen
