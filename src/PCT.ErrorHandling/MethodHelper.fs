[<RequireQualifiedAccess>]
module PCT.ErrorHandling.MethodHelper


open System
open System.Reflection


/// <summary>Get current method name.</summary>
/// <remarks>
/// Caution: In <c>async { ... }</c> blocks, <c>seq { ... }</c> blocks and so on, 
/// the compiler will compile them into state machine.<br/>
/// This will make getted method name is not right. 
/// It returns like <c>[Namespace.Class]::doWorkAsync@57</c>, not <c>[Namespace.Class]::doWorkAsync</c>.
/// </remarks>
let getCurrentMethodFullName () : string =
    let method = MethodBase.GetCurrentMethod()
    let typeName = method.DeclaringType.FullName
    sprintf "[%s]::%s" typeName method.Name