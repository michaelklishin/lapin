namespace Lapin

open NUnit.Framework
open FsUnit

open Lapin.Core
open RabbitMQ.Client

module ConnectionTests =

    [<TestFixture>]
    type ConnectionTests() =
        [<Test>]
        member t.``Connect with all defaults`` () =
            let conn = Lapin.Core.connectWithAllDefaults()
            conn.IsOpen |> should equal true
            conn.Close()
            conn.IsOpen |> should equal false

        [<Test>]
        member t.``Connect with explicitly provided hostname and port`` () =
            use conn = Lapin.Core.connect({hostname = Some "127.0.0.1"; port = Some 5672})
            conn.IsOpen |> should equal true

        member t.``Disconnect with close`` () =
            let conn = Lapin.Core.connectWithAllDefaults()
            Lapin.Core.isOpen conn |> should equal true
            Lapin.Core.close conn |> Lapin.Core.isOpen |> should equal false
            Lapin.Core.isClosed conn |> should equal true

        member t.``Disconnect with abort`` () =
            let conn = Lapin.Core.connectWithAllDefaults()
            Lapin.Core.isOpen conn |> should equal true
            // ignores any possible exceptions
            Lapin.Core.abort conn |> Lapin.Core.isOpen |> should equal false
            Lapin.Core.isClosed conn |> should equal true
