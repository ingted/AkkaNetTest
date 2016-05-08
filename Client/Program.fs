open System.IO
open Akka.FSharp
open System
open Contracts

[<EntryPoint>]
let main _ = 
    let system = 
        Configuration.parse (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let _actor =
        spawn system "Client" <| fun (mb: Actor<Response>) ->
            let server = select "akka.tcp://ActorSystem1@wl-legio-17:2551/user/Server" system
            let startTime = DateTime.UtcNow
            let rec loop count = 
                actor {
                    server <! Request.Add (1, 2)
                    //Logging.logInfo mb "Sent request."
                    let! _msg = mb.Receive()
                    //Logging.logInfo mb "Got response."
                    if count % 1000 = 0 then
                        Logging.logInfof 
                            mb "%d total, %f /s" 
                            count 
                            (1000. * float count / (DateTime.UtcNow - startTime).TotalMilliseconds)
                    return! loop (count + 1)
                }
            loop 0

    Console.ReadKey() |> ignore
    0
