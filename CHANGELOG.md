CHANGELOG

Changelog at https://github.com/medo64/Medo.Uuid7/blob/main/CHANGELOG.md


[3.1.0] (2025-03-16)

- Added ID26 output


[3.0.0] (2024-12-03)

- .NET 9 suppport
- **BREAKING CHANGE**: Guid conversion is done in native endianess (was always big endian before)
  - this was done to match `Guid.CreateVersion7()` behavior by default
  - old behavior possible by using `ToGuid(bigEndian: true)`
- **BREAKING CHANGE**: all overloads that used `matchGuidEndianess` were changed to `bigEndian` parameter.
  - this was done to match most of .NET 9 overloads
  - it is possible to get the old behavior by using opposite boolean value (e.g., `ToGuid(bigEndian: !matchGuidEndianess)`, on little-endian platform)
- improved `ToGuid()` performance
- removed obsoleted functions
- removed explicit .NET 6 support


[2.1.1] (2024-11-08)

- Fixed handling of non-initialized struct


[2.1.0] (2024-11-05)

- Added timestamp support for FillMsSqlUniqueIdentifier
- Removed explicit .NET 7 support
- Fixed handling of non-initialized struct


[2.0.0] (2024-06-29)

- Compatible with the final [RFC 9562](https://datatracker.ietf.org/doc/rfc9562/)
- Fixed compatibility with Microsoft SQL Server UniqueIdentifier
- Added support for Guid endianness when converting from and to System.Guid
- Added support for creating Uuid7 with custom timestamp


[1.9.1] (2024-01-21)

- More parsing performance improvement - courtesy of [Joel Mueller](https://github.com/jtmueller)


[1.9.0] (2024-01-06)

- Parsing performance improvement - courtesy of [Joel Mueller](https://github.com/jtmueller)
- Improved ToString performance - courtesy of [Joel Mueller](https://github.com/jtmueller)


[1.8.2] (2023-11-24)

- Added ToDateTime and ToDateTimeOffset methods


[1.8.1] (2023-11-15)

- Fixed .NET 8 AOT compilation support


[1.8.0] (2023-11-15)

- Added .NET 8 AOT compilation support
- ToString optimizations


[1.7.0] (2023-07-06)

- Improved performance
- Added ISpanFormattable interface
- Added ISpanParsable interface
- Added format specifiers for Id25 ('5') and Id22 ('2')
- Added Fill method for .NET Standard 2.0 targets


[1.6.1] (2023-07-04)

- Static methods use locks to allow for sequencing even when called from
  different threads (e.g. usage with async)


[1.6.0] (2023-07-02)

- Added Entity Framework Core version (separate NuGet package)
- Added NewGuid() method
- Added MS SQL Guid format support (NewGuidMsSql and ToGuidMsSql)
- Added implicit conversion from and to Guid


[1.5.0] (2023-06-29)

- Added TryWriteBytes method
- Updated GetHashCode method
- HW acceleration for Equals method


[1.4.0] (2023-06-19)

- Major optimizations (buffered RandomNumberGenerator calls)
- Changed NuGet package name to Medo.Uuid7
- Obsoleted NuGet package Uuid7


[1.3.5] (2023-06-19)

- Updated readme


[1.3.4] (2023-06-12)

- Minor optimizations
- Updated readme


[1.3.3] (2023-06-07)

- Minor optimizations
- Added fully random v4 UUID support


[1.3.2] (2023-05-17)

- .NET Standard 2.0 support
- ToString() performance optimizations


[1.3.1] (2023-05-16)

- Performance optimizations


[1.3.0] (2023-04-30)

- Added IFormattable interface
- Fixed EF NullReferenceException when CompareArrays gets a null input


[1.2.0] (2023-04-12)

- Timestamps are monotonically increasing even if time goes backward


[1.1.1] (2023-01-14)

- Fixed monotonicity bug


[1.1.0] (2023-01-14)

- Using random increment (was tick based before)
- Performance improvements


[1.0.2] (2023-01-13)

- Fixed docs


[1.0.1] (2023-01-13)

- Added readme


[1.0.0] (2023-01-13)

- First release



[unreleased]: https://github.com/medo64/Medo.uuid7
[3.1.0]: https://www.nuget.org/packages/Medo.Uuid7/3.1.0
[3.0.0]: https://www.nuget.org/packages/Medo.Uuid7/3.0.0
[2.1.1]: https://www.nuget.org/packages/Medo.Uuid7/2.1.1
[2.1.0]: https://www.nuget.org/packages/Medo.Uuid7/2.1.0
[2.0.0]: https://www.nuget.org/packages/Medo.Uuid7/2.0.0
[1.9.1]: https://www.nuget.org/packages/Medo.Uuid7/1.9.1
[1.9.0]: https://www.nuget.org/packages/Medo.Uuid7/1.9.0
[1.8.2]: https://www.nuget.org/packages/Medo.Uuid7/1.8.2
[1.8.1]: https://www.nuget.org/packages/Medo.Uuid7/1.8.1
[1.8.0]: https://www.nuget.org/packages/Medo.Uuid7/1.8.0
[1.7.0]: https://www.nuget.org/packages/Medo.Uuid7/1.7.0
[1.6.1]: https://www.nuget.org/packages/Medo.Uuid7/1.6.1
[1.6.0]: https://www.nuget.org/packages/Medo.Uuid7/1.6.0
[1.5.0]: https://www.nuget.org/packages/Medo.Uuid7/1.5.0
[1.4.0]: https://www.nuget.org/packages/Medo.Uuid7/1.4.0
[1.3.5]: https://www.nuget.org/packages/Uuid7/1.3.5
[1.3.4]: https://www.nuget.org/packages/Uuid7/1.3.4
[1.3.3]: https://www.nuget.org/packages/Uuid7/1.3.3
[1.3.2]: https://www.nuget.org/packages/Uuid7/1.3.2
[1.3.1]: https://www.nuget.org/packages/Uuid7/1.3.1
[1.3.0]: https://www.nuget.org/packages/Uuid7/1.3.0
[1.2.0]: https://www.nuget.org/packages/Uuid7/1.2.0
[1.1.1]: https://www.nuget.org/packages/Uuid7/1.1.1
[1.1.0]: https://www.nuget.org/packages/Uuid7/1.1.0
[1.0.2]: https://www.nuget.org/packages/Uuid7/1.0.2
[1.0.1]: https://www.nuget.org/packages/Uuid7/1.0.1
[1.0.0]: https://www.nuget.org/packages/Uuid7/1.0.0
