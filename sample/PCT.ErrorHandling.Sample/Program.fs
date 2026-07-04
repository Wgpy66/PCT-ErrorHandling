module PCT.ErrorHandling.Sample.Program
    

[<EntryPoint>]
let main _ =
    printfn "PCT.ErrorHandling Sample."
    SampleApp.startApp(".\\appcfg.json")