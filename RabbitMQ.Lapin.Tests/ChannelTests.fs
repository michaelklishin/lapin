namespace Lapin

open NUnit.Framework
open FsUnit

open Lapin.Tests

module ChannelTests =

    [<TestFixture>]
    type ChannelTests() =
        inherit IntegrationTest()

        [<Test>]
        member t.``Opening a channel when channel limit is not reached`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            ch.IsOpen |> should equal true

        [<Test>]
        member t.``Closing a channel`` () =
            use conn = Lapin.Core.connectWithAllDefaults()
            let ch   = Lapin.Channel.``open``(conn)
            ch.IsOpen |> should equal true
            Lapin.Channel.close(ch)
            ch.IsOpen |> should equal false

        [<Test>]
        member t.``Channel predicates`` () =
            t.WithChannel(fun _ ch ->
                Lapin.Channel.isOpen(ch) |> should equal true
                Lapin.Channel.isClosed(ch) |> should equal false
                Lapin.Channel.close(ch)
                Lapin.Channel.isOpen(ch) |> should equal false
                Lapin.Channel.isClosed(ch) |> should equal true)