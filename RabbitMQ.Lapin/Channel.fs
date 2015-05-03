namespace Lapin

open System
open RabbitMQ.Client

module Channel =
    type IChannel = IModel
    let ``open``(conn : IConnection) : IChannel =
        conn.CreateModel()
    let close(ch : IChannel) =
        ch.Close()
    let isOpen(ch : IChannel) : bool =
        ch.IsOpen
    let isClosed(ch : IChannel) : bool =
        ch.IsClosed
    let confirmSelect(ch : IChannel) : IChannel =
        ch.ConfirmSelect() |> ignore
        ch
    let enablePublisherConfirms = confirmSelect
    let waitForConfirms(ch : IChannel, timeout: TimeSpan): bool =
        ch.WaitForConfirms(timeout)