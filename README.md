# Jinn

An experiment to build a minimal, performant, AOT compliant, 
command line parser to later be used as the base for Spectre.Console.Cli 

Based on and inspired by the fantastic `System.CommandLine`.

> [!NOTE]  
> Not at all usable in the current state

## Building

We're using [Cake](https://github.com/cake-build/cake) as a 
[dotnet tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) 
for building. So make sure that you've restored Cake by running 
the following in the repository root:

```
> dotnet tool restore
```

After that, running the build is as easy as writing:

```
> dotnet cake
```

## Copyright

Copyright Patrik Svensson