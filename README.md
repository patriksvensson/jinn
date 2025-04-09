# Jinn

An experiment to build a flexible, POSIX compliant, AOT friendly, command line parser to later be used as 
the base for `Spectre.Console.Cli`.

Very much inspired by `System.CommandLine`.

> [!NOTE]  
> Not usable at all in the current state :)

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