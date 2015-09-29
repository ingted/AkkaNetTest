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
        ConfigurationFactory.ParseString (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let actor =
        spawn system "Actor1" <| fun mb ->
            let rec loop() = actor {
                let! msg = mb.Receive()
                Logging.logInfof mb "Got %+A" msg
                match msg with
                | Request.Add (x, y) -> mb.Sender() <! Response.Added (x + y)
                return! loop()
            }
            loop()
    Console.ReadKey() |> ignore
    0
