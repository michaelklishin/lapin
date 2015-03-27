namespace Lapin

open RabbitMQ.Client

module Core =
    type Hostname = string
    type Port = int

    let DefaultHostname = Some "127.0.0.1"
    let DefaultPort = Some Protocols.AMQP_0_9_1.DefaultPort

    type ConnectionOptions = {
        hostname: Option<Hostname>;
        port: Option<Port>
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