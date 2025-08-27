/// <summary>
/// Provides helper functions for string manipulation and formatting.
/// </summary>
module Archer.Logger.StringHelpers

open System

/// <summary>
/// Trims whitespace from both ends of the string. Returns null if the input is null.
/// </summary>
/// <param name="value">The string to trim.</param>
/// <returns>The trimmed string, or null if input is null.</returns>
let trim (value: string) =
    if value = null then value
    else value.Trim ()

/// <summary>
/// Trims whitespace from the end of the string. Returns null if the input is null.
/// </summary>
/// <param name="value">The string to trim.</param>
/// <returns>The trimmed string, or null if input is null.</returns>
let trimEnd (value: string) =
    if value = null then value
    else value.TrimEnd ()

/// <summary>
/// Replaces all occurrences of a specified substring with another substring in the given string.
/// </summary>
/// <param name="toBeReplaced">The substring to be replaced.</param>
/// <param name="toReplace">The substring to replace with.</param>
/// <param name="inValue">The string in which to perform the replacement.</param>
/// <returns>The resulting string after replacements.</returns>
let replace (toBeReplaced: string) (toReplace: string) (inValue: string) =
    inValue.Replace (toBeReplaced, toReplace)

/// <summary>
/// Appends a newline character to the end of the given string.
/// </summary>
/// <param name="value">The string to append a newline to.</param>
/// <returns>The string with a newline appended.</returns>
let appendNewLine value =
    $"%s{value}%s{Environment.NewLine}"

/// <summary>
/// Appends a newline character to the end of the string if it is not empty or null.
/// </summary>
/// <param name="value">The string to conditionally append a newline to.</param>
/// <returns>The string with a newline appended if not empty; otherwise, the original string.</returns>
let appendNewLineIfNotEmpty value =
    if String.IsNullOrEmpty value then value
    else $"%s{value}%s{Environment.NewLine}"

/// <summary>
/// Removes the last character from the string.
/// </summary>
/// <param name="value">The string to modify.</param>
/// <returns>The string with the last character removed.</returns>
let removeLastChar (value: string) =
    value.Remove (value.Length - 1)

/// <summary>
/// Joins a list of strings into a single string separated by newlines, and trims any trailing whitespace.
/// </summary>
/// <param name="items">The list of strings to join.</param>
/// <returns>A single string with each item separated by a newline, trimmed at the end.</returns>
let linesToString (items: string list) =
    items
    |> String.concat Environment.NewLine
    |> trimEnd