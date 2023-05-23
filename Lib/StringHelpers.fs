module Archer.Logger.StringHelpers

let trim (value: string) =
    if (value = null) then value
    else value.Trim ()
    
let trimEnd (value: string) =
    if (value = null) then value
    else value.TrimEnd ()