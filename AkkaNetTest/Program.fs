open System.IO
open Akka.FSharp
open Contracts
open System

[<EntryPoint>]
let main _ = 
    let system = 
        Configuration.parse (File.ReadAllText "akka.hocon")
        |> System.create "ActorSystem1"

    let _actor =
        spawn system "Server" <| fun mb ->
            let startTime = DateTime.UtcNow
            let rec loop count = 
                actor {
                    let! msg = mb.Receive()
                    //Logging.logInfo mb "Got request."
                    match msg with
                    | Request.Add (x, y) -> 
                        mb.Sender() <! Response.Added (x + y)
                        //Logging.logInfo mb "Sent response."
                    if count % 1000 = 0 then
                        Logging.logInfof 
                            mb 
                            "%d total, %f /s" 
                            count 
                            (1000. * float count / (DateTime.UtcNow - startTime).TotalMilliseconds)
                    return! loop (count + 1)
                }
            loop 0
    Console.ReadKey() |> ignore
    0
