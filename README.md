# Archer.Logger
A logger to be used in reporting for Archer Test Framework

## Reporter Idea

### Test Reporter

```f#
    // Example Test Name Reporter
    let testNameReporter (test: ITestLocationInfo) =
        $"%s{test.ContainerPath}.%s{test.ContainerName}.%{test.TestName}"
        
    let locationReporter: CodeLocation -> string = 
        builtLocationReporter
            {
                TestPathReporter: testPathReporter
                TestFileNameReporter: testFileNameReporter
                LineNumberReporter: lineNumberReporter
            }
        
    let testReporter: ITestInfo -> string = 
        buildTestReporter
            {
                TestNameReporter: ITestNameInfo -> string = testNameReporeter
                LocationReporter: CodeLocation -> string = locationReporter
            }
```