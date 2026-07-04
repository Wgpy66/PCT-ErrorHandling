# PCT.ErrorHandling

Error handling with F# easily.

## Installation

Downloaf from the [release](https://github.com/Wgpy66/PCT-ErrorHandling/releases).

Using dotnet CLI to install downloaded packages.

```powershell
dotnet add package PCT.ErrorHandling --source ".\YourPkgDownloadDir"
```

Or using a powershell cmdlet. 

```powershell
Install-Package PCT.ErrorHandling -Source ".\YourPkgDownloadDir"
```

## Quick Start

```fsharp
open System.IO
open PCT.ErrorHandling

type AccessLevel =
    | Administrator
    | User
    | Tourists

type Account = {
    name: string
    access: AccessLevel
}

let validateAdmin (acc: Account) =
    Resultx.ensure (acc.access = AccessLevel.Administrator) 5 "Access is denied"

let loginControlPanel (acc: Account) =
    match validateAdmin acc with
        | Ok _ -> printfn "Hello, %s" acc.name
        | Error err -> 
            printfn "Error: %A" err
            printfn "You don't have correct access."

let startApp () = 
    let accounts = [ { name = "Alex"; access = Administrator }; { name = "Steve"; access = User } ]
    accounts |> List.iter (fun acc -> loginControlPanel acc)
```

## Help document

wip.