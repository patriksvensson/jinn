---
sidebar_position: 100
---

# Benchmarks

Latest benchmarks as of `2025-06-01`.

```
BenchmarkDotNet v0.14.0, macOS Sequoia 15.5 (24F74) [Darwin 24.5.0]
Apple M3 Pro, 1 CPU, 11 logical and 11 physical cores
.NET SDK 9.0.202
  [Host]     : .NET 9.0.3 (9.0.325.11113), Arm64 RyuJIT AdvSIMD
  DefaultJob : .NET 9.0.3 (9.0.325.11113), Arm64 RyuJIT AdvSIMD


| Method      | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------ |---------:|----------:|----------:|-------:|-------:|----------:|
| Jinn_Parse  | 1.630 us | 0.0298 us | 0.0279 us | 0.7877 | 0.0076 |   6.44 KB |
| Jinn_Invoke | 1.880 us | 0.0364 us | 0.0357 us | 0.9136 | 0.0095 |   7.47 KB |
```