---
sidebar_position: 100
---

# Benchmarks

## 2025-10-16

```
BenchmarkDotNet v0.15.2, macOS 26.0.1 (25A362) [Darwin 25.0.0]
Apple M3 Pro, 1 CPU, 11 logical and 11 physical cores
.NET SDK 10.0.100-rc.2.25502.107
  [Host]    : .NET 10.0.0 (10.0.25.50307), Arm64 RyuJIT AdvSIMD
  .NET 10.0 : .NET 10.0.0 (10.0.25.50307), Arm64 RyuJIT AdvSIMD
  .NET 9.0  : .NET 9.0.9 (9.0.925.41916), Arm64 RyuJIT AdvSIMD


| Method      | Job       | Runtime   | Mean     | Error     | StdDev    | Gen0   | Gen1   | Allocated |
|------------ |---------- |---------- |---------:|----------:|----------:|-------:|-------:|----------:|
| Jinn_Parse  | .NET 10.0 | .NET 10.0 | 1.191 us | 0.0063 us | 0.0052 us | 0.7057 | 0.0057 |   5.77 KB |
| Jinn_Invoke | .NET 10.0 | .NET 10.0 | 1.387 us | 0.0119 us | 0.0111 us | 0.8411 | 0.0095 |   6.88 KB |
| Jinn_Parse  | .NET 9.0  | .NET 9.0  | 1.498 us | 0.0090 us | 0.0084 us | 0.7439 | 0.0076 |   6.09 KB |
| Jinn_Invoke | .NET 9.0  | .NET 9.0  | 1.765 us | 0.0071 us | 0.0067 us | 0.8793 | 0.0095 |    7.2 KB |
```

## 2025-06-01

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