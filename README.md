# Jinn

An experiment to build a minimal, performant, AOT compliant, 
command line parser to later be used as the base for Spectre.Console.Cli 

Based on and inspired by the fantastic `System.CommandLine`.

> [!NOTE]  
> Not at all usable in the current state

## Building

We're using [Cake](https://github.com/cake-build/cake) for building. 

```shell
$ dotnet run build.cs
```

### Building using .NET Make

You can also use [.NET Make](https://github.com/patriksvensson/dotnet-make) 
to run the build. _.NET Make_ is a tool that detects what build orchestration
you are using and takes care of executing the build for you, regardless 
what your current directory in the repository is.

```shell
$ dotnet tool restore
```

After that, running the build is as easy as writing:

```shell
$ dotnet make
```

## Copyright

Copyright Patrik Svensson