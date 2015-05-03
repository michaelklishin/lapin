namespace Lapin

open System
open System.Collections.Generic
open RabbitMQ.Client

module Types =
    type Hostname   = string
    type Port       = int

    type IChannel   = IModel

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
    type DeliveryMode =
        | Transient = 1uy
        | Persistent = 2uy
    type Expiration =
        | MillisecondsAsString of string
        | Milliseconds of uint32

    type MessageProps = {
        contentEncoding: Option<string>
        contentType: Option<string>
        correlationId: Option<string>
        deliveryMode: DeliveryMode
        expiration: Option<Expiration>
        priority: Option<byte>
        messageId: Option<string>
        ``type``: Option<string>
        replyTo: Option<string>

    }
    type Metadata = {
        mandatory: Mandatory
        properties: Option<MessageProps>
    }

    let argumentsToIDictionary(args: Arguments): IDictionary<string, Object> =
        // Map<string, Object> won't accept a null value which some C# methods expect
        match args with
        | Some m -> m |> Map.toSeq |> dict
        | None   -> null

    let messagePropertiesToIBasicProperties(ch: IChannel, props: MessageProps): IBasicProperties =
        let bp = ch.CreateBasicProperties()
        bp.DeliveryMode <- (byte)props.deliveryMode
        match props.expiration with
            | None   -> ignore()
            | Some e -> bp.Expiration <- match e with
                                            | MillisecondsAsString s -> s
                                            | Milliseconds n -> Convert.ToString(n)

        match props.contentEncoding with
            | Some s -> bp.ContentEncoding <- s
            | None   -> ignore()
        match props.contentType with
            | Some s -> bp.ContentType <- s
            | None   -> ignore()
        match props.replyTo with
            | Some s -> bp.ReplyTo <- s
            | None   -> ignore()
        match props.correlationId with
            | Some s -> bp.CorrelationId <- s
            | None   -> ignore()
        match props.messageId with
            | Some s -> bp.MessageId <- s
            | None   -> ignore()
        match props.``type`` with
            | Some s -> bp.Type <- s
            | None   -> ignore()
        bp.Priority <- match props.priority with
                            | Some n -> n
                            | None   -> Unchecked.defaultof<byte>
        bp

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
