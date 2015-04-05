namespace Lapin

open System
open RabbitMQ.Client

open Lapin.Channel
open Lapin.Types
open Lapin.Exchange

module Basic =
    let publish(ch: IChannel, endpoint: ExchangeAndRoutingKey, body: Body, metadata: Option<Metadata>): unit =
        let (mandatory, props: IBasicProperties) =
            match metadata with
            | Some m -> match m with
                           | { Metadata.mandatory = v; Metadata.properties = None }   -> (v, null)
                           | { Metadata.mandatory = v; Metadata.properties = Some p } -> (v, p)
            | None   -> (false, null)
        ch.BasicPublish(endpoint.exchange, endpoint.routingKey, mandatory, props, body)

    type GetResponse = {
        properties: IBasicProperties
        body: Body
        deliveryTag: DeliveryTag
        redelivered: bool
        messageCount: uint32
        routingContext: ExchangeAndRoutingKey
    }

    let private convertBasicGetToRecord(result: BasicGetResult): GetResponse =
        { properties = result.BasicProperties;
          body = result.Body;
          deliveryTag = result.DeliveryTag;
          redelivered = result.Redelivered;
          messageCount = result.MessageCount;
          routingContext = { exchange = result.Exchange;
                            routingKey = result.RoutingKey } }

    let get(ch: IChannel, queue: Name, automaticallyAck: bool): GetResponse =
        convertBasicGetToRecord(ch.BasicGet(queue, automaticallyAck))

    let getAutoAck(ch: IChannel, queue: Name): GetResponse =
        get(ch, queue, true)

    let consume(ch: IChannel, queue: Name, automaticallyAck: bool, consumer: IBasicConsumer) =
        ch.BasicConsume(queue, automaticallyAck, consumer)

    let consumeAutoAck(ch: IChannel, queue: Name, consumer: IBasicConsumer) =
        ch.BasicConsume(queue, true, consumer)