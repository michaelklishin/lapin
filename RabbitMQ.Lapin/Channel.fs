namespace Lapin

open RabbitMQ.Client

module Channel =
    type IChannel = IModel

    let ``open``(conn: IConnection): IChannel =
        conn.CreateModel()

    let close(ch: IChannel) =
        ch.Close()

    let isOpen(ch: IChannel): bool =
        ch.IsOpen

    let isClosed(ch: IChannel): bool =
        ch.IsClosed