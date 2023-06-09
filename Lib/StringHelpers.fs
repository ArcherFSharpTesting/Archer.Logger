﻿module Archer.Logger.StringHelpers

open System

let trim (value: string) =
    if value = null then value
    else value.Trim ()

let trimEnd (value: string) =
    if value = null then value
    else value.TrimEnd ()
    
let replace (toBeReplaced: string) (toReplace: string) (inValue: string) =
    inValue.Replace (toBeReplaced, toReplace)
    
let appendNewLine value =
    $"%s{value}%s{Environment.NewLine}"
    
let appendNewLineIfNotEmpty value =
    if String.IsNullOrEmpty value then value
    else $"%s{value}%s{Environment.NewLine}"
    
let removeLastChar (value: string) =
    value.Remove (value.Length - 1)
    
let linesToString (items: string list) =
    items
    |> String.concat Environment.NewLine
    |> trimEnd