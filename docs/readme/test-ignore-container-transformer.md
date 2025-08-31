<!-- (dl
(section-meta
    (title Archer.Reporting Test Ignore Container Transformer)
)
) -->

This document describes the Test Ignore Container Transformer in Archer.Reporting. This module provides functions to transform and format test ignore containers for reporting and output.

<!-- (dl (# getWrappedTestIgnoreMessage)) -->
**getWrappedTestIgnoreMessage**
- Wraps a test ignore in a formatted message using the provided indenter and assembly.
- Signature: `Assembly -> IIndentTransformer -> TestIgnore -> string`

<!-- (dl (# transformTestIgnoreType)) -->
**transformTestIgnoreType**
- Transforms a test ignore type and test into a detailed string using the appropriate transformer for each ignore type.
- Signature: `IIndentTransformer -> (TestIgnoreType * ITest) -> string`

<!-- (dl (# testIgnoreContainerTransformer)) -->
**testIgnoreContainerTransformer**
- Recursively transforms a `TestIgnoreContainer` into a formatted string, handling nested containers and ignores.
- Signature: `(IIndentTransformer -> (TestIgnoreType * ITest) -> string) -> IIndentTransformer -> TestIgnoreContainer -> string`

<!-- (dl (# defaultTestIgnoreContainerTransformer)) -->
**defaultTestIgnoreContainerTransformer**
- Default transformer for a single `TestIgnoreContainer` using the standard ignore type transformer.
- Signature: `IIndentTransformer -> TestIgnoreContainer -> string`

<!-- (dl (# defaultTestIgnoreContainerAllTransformer)) -->
**defaultTestIgnoreContainerAllTransformer**
- Transforms a list of `TestIgnoreContainer` values into a single formatted string.
- Signature: `IIndentTransformer -> TestIgnoreContainer list -> string`
