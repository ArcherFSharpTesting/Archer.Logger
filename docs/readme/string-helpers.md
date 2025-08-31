<!-- (dl
(section-meta
    (title Archer.Reporting String Helpers)
)
) -->

This document describes the string helper functions provided in the Archer.Reporting library. These helpers are used for common string manipulations and formatting tasks within the reporter.

<!-- (dl (# trim)) -->
**trim**
- Removes whitespace from both ends of a string. Returns the original value if it is null.

<!-- (dl (# trimEnd)) -->
**trimEnd**
- Removes whitespace from the end of a string. Returns the original value if it is null.

<!-- (dl (# replace)) -->
**replace**
- Replaces all occurrences of a substring with another substring in the given string.
- Parameters: `toBeReplaced`, `toReplace`, `inValue`

<!-- (dl (# appendNewLine)) -->
**appendNewLine**
- Appends a newline character to the end of the given string.

<!-- (dl (# appendNewLineIfNotEmpty)) -->
**appendNewLineIfNotEmpty**
- Appends a newline character to the string only if it is not empty or null.

<!-- (dl (# removeLastChar)) -->
**removeLastChar**
- Removes the last character from the string.

<!-- (dl (# linesToString)) -->
**linesToString**
- Joins a list of strings into a single string separated by newlines, and trims any trailing whitespace.
