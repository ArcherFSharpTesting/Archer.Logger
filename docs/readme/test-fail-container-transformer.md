<!-- (dl
(section-meta
    (title Archer.Reporting Test Fail Container Transformer)
)
) -->

This document describes the Test Fail Container Transformer in Archer.Reporting. This module provides functions to transform and format test failure containers for reporting and output.

<!-- (dl (# getWrappedTestFailureMessage)) -->
**getWrappedTestFailureMessage**
- Wraps a test failure in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestFailure -> string`

<!-- (dl (# getIgnoreAssemblyGeneralTestingFailureMessage)) -->
**getIgnoreAssemblyGeneralTestingFailureMessage**
- Formats a general testing failure message for ignored assemblies.
- Signature: `Assembly -> IIndentTransformer -> GeneralTestingFailure -> string`

<!-- (dl (# transformTestFailureType)) -->
**transformTestFailureType**
- Transforms a test failure type and test into a detailed string using the appropriate transformer for each failure type.
- Signature: `IIndentTransformer -> (TestFailureType * ITest) -> string`

<!-- (dl (# testFailContainerTransformer)) -->
**testFailContainerTransformer**
- Recursively transforms a `TestFailContainer` into a formatted string, handling nested containers and failures.
- Signature: `(IIndentTransformer -> (TestFailureType * ITest) -> string) -> IIndentTransformer -> TestFailContainer -> string`

<!-- (dl (# defaultTestFailContainerTransformer)) -->
**defaultTestFailContainerTransformer**
- Default transformer for a single `TestFailContainer` using the standard failure type transformer.
- Signature: `IIndentTransformer -> TestFailContainer -> string`

<!-- (dl (# defaultTestFailContainerAllTransformer)) -->
**defaultTestFailContainerAllTransformer**
- Transforms a list of `TestFailContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestFailContainer list -> string`
