open Akka
open Akka.Actor
open Akka.Remote
open Akka.Configuration
open Akka.Configuration.Hocon
open System.IO
open Akka.FSharp
open Contracts
open System

[<EntryPoint>]
let main _ = 
    let system = 
        Configuration.parse (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let actor =
        spawn system "Server" <| fun mb ->
            let inline info x = Logging.logInfo mb x
            let inline infof x = Logging.logInfof mb x
            let rec loop() = actor {
                info "Receiving request..."
                let! msg = mb.Receive()
                infof "Got %+A" msg
                match msg with
                | Request.Add (x, y) ->
                    info "Sending response..." 
                    mb.Sender() <! Response.Added (x + y)
                    info "Response has been sent."
                return! loop()
            }
            loop()
    Console.ReadKey() |> ignore
    0
