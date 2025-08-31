<!-- (dl
(section-meta
    (title Archer.Reporting Test Success Container Transformer)
)
) -->

This document describes the Test Success Container Transformer in Archer.Reporting. This module provides functions to transform and format test success containers for reporting and output.

<!-- (dl (# getWrappedTestSuccessMessage)) -->
**getWrappedTestSuccessMessage**
- Wraps a test success in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestSuccess -> string`

<!-- (dl (# transformTestSuccessType)) -->
**transformTestSuccessType**
- Transforms a test success type and test into a detailed string using the appropriate transformer for each success type.
- Signature: `IIndentTransformer -> (TestSuccessType * ITest) -> string`

<!-- (dl (# testSuccessContainerTransformer)) -->
**testSuccessContainerTransformer**
- Recursively transforms a `TestSuccessContainer` into a formatted string, handling nested containers and successes.
- Signature: `(IIndentTransformer -> (TestSuccessType * ITest) -> string) -> IIndentTransformer -> TestSuccessContainer -> string`

<!-- (dl (# defaultTestSuccessContainerTransformer)) -->
**defaultTestSuccessContainerTransformer**
- Default transformer for a single `TestSuccessContainer` using the standard success type transformer.
- Signature: `IIndentTransformer -> TestSuccessContainer -> string`

<!-- (dl (# defaultTestSuccessContainerAllTransformer)) -->
**defaultTestSuccessContainerAllTransformer**
- Transforms a list of `TestSuccessContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestSuccessContainer list -> string`
