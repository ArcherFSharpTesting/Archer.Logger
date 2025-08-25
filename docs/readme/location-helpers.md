<!-- (dl
(section-meta
    (title Archer.Logger Location Helpers)
)
) -->

This document describes the location helper functions in Archer.Logger. These helpers are used to determine solution roots and generate relative file paths for test reporting and logging.

<!-- (dl (# getSolutionRoot)) -->
**getSolutionRoot**
- Finds the root directory of the solution by searching for a `.sln` file, starting from the given assembly's location and moving up the directory tree.
- Signature: `Assembly -> string`

<!-- (dl (# getRelativePath)) -->
**getRelativePath**
- Returns the relative path from the solution root to the given directory.
- Signature: `Assembly -> DirectoryInfo -> string`

<!-- (dl (# getRelativePathFromPath)) -->
**getRelativePathFromPath**
- Returns the relative path from the solution root to the directory specified by a path string.
- Signature: `Assembly -> string -> string`

<!-- (dl (# getRelativePathFromTest)) -->
**getRelativePathFromTest**
- Returns the relative path from the solution root to the file location of a test.
- Signature: `Assembly -> ITestLocationInfo -> string`

<!-- (dl (# getRelativeFilePath)) -->
**getRelativeFilePath**
- Returns the relative file path (including file name) for a test's location.
- Signature: `Assembly -> ITestLocationInfo -> string`
