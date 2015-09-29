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
        ConfigurationFactory.ParseString (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let actor = select "akka.tcp://ActorSystem1@localhost:2551/user/Actor1" system
    async {
        let! response = actor <? Request.Add (1, 2)
        printfn "Got response: %+A" response
    } |> Async.RunSynchronously

    Console.ReadKey() |> ignore
    0
