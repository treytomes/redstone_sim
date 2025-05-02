# Create solution
dotnet new sln -n RedstoneSimulator

# Create Core project
dotnet new classlib -n RedstoneSimulator.Core -o src/RedstoneSimulator.Core

# Create Test project
dotnet new xunit -n RedstoneSimulator.Tests -o tests/RedstoneSimulator.Tests

# Add projects to solution
dotnet sln add src/RedstoneSimulator.Core/RedstoneSimulator.Core.csproj
dotnet sln add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj

# Add reference from Tests to Core
dotnet add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj reference src/RedstoneSimulator.Core/RedstoneSimulator.Core.csproj

# Add libraries.

dotnet add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj package xunit
dotnet add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj package xunit.runner.visualstudio
dotnet add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj package Microsoft.NET.Test.Sdk
dotnet add tests/RedstoneSimulator.Tests/RedstoneSimulator.Tests.csproj package Moq