namespace Lapin

open System
open RabbitMQ.Client

open Lapin.Channel
open Lapin.Types
open Lapin.Exchange

module Basic =
    type ExchangeAndRoutingKey = {
        exchange: Name
        routingKey: RoutingKey
    }
    type Metadata = {
        mandatory: Mandatory
        properties: Option<IBasicProperties>
    }

    let publish(ch: IChannel, endpoint: ExchangeAndRoutingKey, body: byte[], metadata: Option<Metadata>): unit =
        let (mandatory, props: IBasicProperties) =
            match metadata with
            | Some m -> match m with
                           | { Metadata.mandatory = v; Metadata.properties = None }   -> (v, null)
                           | { Metadata.mandatory = v; Metadata.properties = Some p } -> (v, p)
            | None   -> (false, null)
        ch.BasicPublish(endpoint.exchange, endpoint.routingKey, mandatory, props, body)