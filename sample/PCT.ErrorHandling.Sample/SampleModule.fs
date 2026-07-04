module PCT.ErrorHandling.Sample.SampleApp


open System
open System.IO

open Newtonsoft.Json
open PCT.ErrorHandling


type AppConfig = {
    osMinVersion: Version
    timeout: int
}

let requestUuidApi (timeout: int) =
    let m1 = MethodHelper.getCurrentMethodFullName()
    let getUuid =
        async {
            let randomNumber = Random.Shared.Next(0, 10) *1000
            do! Async.Sleep randomNumber
            let uuid = Guid.NewGuid()
            return uuid
        }
    try
        let uuid = Async.RunSynchronously(getUuid, timeout)
        Ok(uuid)
    with 
        | :? TimeoutException as tex ->
            Resultx.nerrorwm 258 (sprintf "Request UUID API timeout after %d ms" timeout) [ m1 ]
        | ex -> 
            Error(RuntimeExn ex)

let parseConfig (filePath: string) : Resultx<AppConfig> =
    try
        let content = File.ReadAllText(filePath)
        let appcfg = JsonConvert.DeserializeObject<AppConfig>(content)
        Ok(appcfg)
    with ex ->
        Error (Errorx.RuntimeExn ex)

let checkVersion (appcfg: AppConfig) =
    let checker () =
        let currentVer = Environment.OSVersion.Version
        let minVer = appcfg.osMinVersion
        if currentVer < minVer then
            false
        else
            true
    Resultx.ensure (checker()) 1150 "Windows OS version is too low"

let checkTimeout (appcfg: AppConfig) =
    let checker () = not (appcfg.timeout < 0 && appcfg.timeout <> -1)
    Resultx.ensure (checker()) 87 "Timeout value is invalid. It should be -1 or a non-negative integer."

let startApp (filePath: string) =
    let cfg = parseConfig filePath |> Resultx.expect "Failed to get app config"
    
    match checkVersion cfg with
        | Ok(()) -> ()
        | Error e -> printfn "Error: %A" e

    let timeout =
        match checkTimeout cfg with
            | Ok(()) -> cfg.timeout
            | Error e -> 
                printfn "Error: %A" e
                3000

    match requestUuidApi timeout with
        | Ok uuid -> 
            printfn "Info: Request UUID API success: %A" uuid
            0
        | Error e -> 
            printfn "Error: %A" e
            -1