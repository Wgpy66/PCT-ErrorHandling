namespace PCT.ErrorHandling


open System


/// Type alias for Result<'T, Errorx>
type Resultx<'T> = Result<'T, Errorx> 

/// Utils for Resultx<'T>
[<RequireQualifiedAccess>]
module Resultx =
    
    /// If an operation is successful, return this value. Otherwise, raise an exception.
    let unwrap (r: Resultx<'T>) : 'T =
        match r with
            | Ok v -> v
            | Error err ->
                match err with
                | RuntimeExn ex -> raise ex
                | NormalError(code, msg, methods, context) ->
                    let ex = Exception(sprintf "%s (Error code: %d) (Error from: %A)" msg code methods)
                    context |> Option.iter (Map.iter (fun k v -> ex.Data.[k] <- v))
                    raise ex
    
    /// If an operation is successful, return this value. Otherwise, raise an exception with specific message.
    let expect (msg: string) (r: Resultx<'T>) : 'T =
        match r with
            | Ok v -> v
            | Error err ->
                match err with
                | RuntimeExn ex ->
                    ex.Data.["expectMessage"] <- msg
                    raise ex
                | NormalError(code, errMsg, methods, context) ->
                    let finalMsg = sprintf "%s (Reason: %s) (Error code: %d) (Error from: %A)" msg errMsg code methods
                    let ex = Exception(finalMsg)
                    context |> Option.iter (Map.iter (fun k v -> ex.Data.[k] <- v))
                    ex.Data.["expectMessage"] <- msg
                    ex.Data.["errorCode"] <- code
                    ex.Data.["isFromNormalError"] <- true
                    raise ex

    let addContext (context: Map<string, obj>) (r: Resultx<'T>) : Resultx<'T> =
        match r with
            | Ok _ -> r
            | Error err ->
                Error(Errorx.addContext context err)

    let addMessage (msg: string) (r: Resultx<'T>) : Resultx<'T> =
        match r with
            | Ok _ -> r
            | Error err ->
                Error(Errorx.addMessage msg err)

    let pushMethods (methodsFullName: string list) (r: Resultx<'T>) : Resultx<'T> =
        match r with
            | Ok _ -> r
            | Error err ->
                Error(Errorx.pushMethods methodsFullName err)
    
    /// New a <c>Errorx.NormalError</c> instance with error code and message quickly
    let nerror code msg =
        Error(NormalError(code, msg, [], None))

    /// New a <c>Errorx.NormalError</c> instance with error code, message, called methods list quickly
    let nerrorwm code msg methods =
        Error(NormalError(code, msg, methods, None))
    
    /// <summary>
    /// Verify a condition and new a <c>Errorx.NormalError</c> with error code and message.
    /// If it is true, return <c>Ok(())</c>. Otherwise, return <c>Error(NormalError(...))</c>
    /// </summary>
    let ensure condition code msg =
        if condition then
            Ok(())
        else 
            let ctx = Map.ofList [("conditionFailedMsg", box msg)]
            Error(NormalError(code, msg, [], Some ctx))

    /// <summary>
    /// Verify a condition and new a <c>Errorx.NormalError</c> with error code, message, called methods list. 
    /// If it is true, return <c>Ok(())</c>. Otherwise, return <c>Error(NormalError(...))</c>
    /// </summary>
    let ensurewm condition code msg method =
        if condition then
            Ok(())
        else 
            let ctx = Map.ofList [("conditionFailedMsg", box msg)]
            Error(NormalError(code, msg, method, Some ctx))
    
    /// Error message formatter.
    let nerrorf code fmt =
        Printf.kprintf (fun msg -> Error(NormalError(code, msg, [], None))) fmt