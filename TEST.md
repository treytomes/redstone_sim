# How to Test

- Run tests with code coverage:

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura
```

- Generate a report:

```bash
reportgenerator "-reports:./TestResults/Coverage/coverage.cobertura.xml" "-targetdir:./TestResults/CoverageReport" -reporttypes:Html
```

## VS.Code

- Extensions:

    - ryanluker.vscode-coverage-gutters
    - formulahendry.dotnet-test-explorer

