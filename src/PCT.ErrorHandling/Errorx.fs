namespace PCT.ErrorHandling


open System

/// <summary>
/// A self-defined Error union type.
/// <c>RuntimeExn</c> provide a wrappper of <c>System.Exception</c> class.
/// <c>NormalError</c> provide a template of business error, with error code, message, called methods list and detail context.
/// </summary>
type Errorx = 
    | RuntimeExn of ex: exn
    | NormalError of code: int * msg: string * methods: string list * context: Map<string, obj> option

    /// Add called methods full name.
    member this.pushMethods (methodFullName: string) : Errorx =
        match this with
            | RuntimeExn _ -> this
            | NormalError(code, msg, methods, context) ->
                NormalError(code, msg, methods @ [ methodFullName ], context)

    /// Add detail context.
    member this.addContext (context': Map<string, obj>) : Errorx =
        match this with
            | RuntimeExn _ -> this
            | NormalError(code, msg, methods, context) ->
                let newContext =
                    match context with
                        | Some c -> 
                            (c, context') ||> Map.fold (fun acc key value -> Map.add key value acc)
                        | None -> context'
                NormalError(code, msg, methods, Some newContext)
    
    /// Add a message.
    member this.addMessage (msg: string) : Errorx =
        match this with
            | RuntimeExn _ -> this
            | NormalError(code, oldMsg, methods, context) ->
                let newMsg = sprintf "%s (%s)" oldMsg msg
                NormalError(code, newMsg, methods, context)