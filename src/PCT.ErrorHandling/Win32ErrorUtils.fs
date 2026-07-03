/// (Windows only) Some Win32 error utilities.
[<RequireQualifiedAccess>]
[<System.Runtime.Versioning.SupportedOSPlatform("windows")>]
module PCT.ErrorHandling.Win32Utils


open System
open System.ComponentModel
open System.Runtime.InteropServices


/// (Windows only) Get Win32 error messages with error code.
[<System.Runtime.Versioning.SupportedOSPlatform("windows")>]
let getInformationFromWinError (errCode: int) (isComException: bool) : (int * string) option =
    if isComException then
        let ex = Marshal.GetExceptionForHR(errCode)
        if not(isNull ex) then 
            Some(
                (errCode, ex.Message)
            )
        else
            None
    else
        if errCode = 0 then
            None
        else
            Some(
                (errCode, Win32Exception(errCode).Message)
            )