open Akka
open Akka.Actor
open Akka.Remote
open Akka.Configuration
open Akka.Configuration.Hocon
open System.IO
open Akka.FSharp
open Contracts
open System
open Contracts

[<EntryPoint>]
let main _ = 
    let system = 
        Configuration.parse (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let actor =
        spawn system "Client" <| fun (mb: Actor<Response>) ->
            let inline info x = Logging.logInfo mb x
            let server = select "akka.tcp://ActorSystem1@localhost:2551/user/Server" system
            let rec loop() = actor {
                info "Sending request..."
                server <! Request.Add (1, 2)
                info "Request has been sent. Receiving response..."
                let! msg = mb.Receive()
                Logging.logInfof mb "Received: %+A" msg
                Async.Sleep 1000 |> Async.RunSynchronously
                return! loop()
            }
            loop()
//    async {
//        let! response = server <? Request.Add (1, 2)
//        printfn "Got response: %+A" response
//    } |> Async.RunSynchronously

    Console.ReadKey() |> ignore
    0
