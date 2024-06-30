Medo.Uuid7
==========

The UUID7 library is an implementation of the UUID version 7 and 4 as defined
in [the RFC 9562][rfc9562], which introduces a time-ordered value field derived
from the timestamp source. It offers improved entropy characteristics compared
to versions 1 or 6 of the UUID standard. The inherent monotonicity of UUID
version 7 makes it an excellent choice for utilization as a binary database key.

Features:
* Time-ordered value field: UUID7 utilizes the widely implemented Unix Epoch
  timestamp source to generate a time-ordered value field. This enables easy
  sorting and indexing of resources based on their creation time.
* Enhanced entropy characteristics: UUID7 provides improved entropy
  characteristics over UUID versions 1 or 6. The inclusion of the timestamp
  ensures a high level of uniqueness, minimizing the chances of collisions
  across different systems or instances.
* Multiple string representations: In addition to the standard UUID string
  formatting, library also offers ID22 and ID25 string conversions.
* Wide compatibility: Support for .NET Standard 2.0 makes this library
  compatible with .NET Framework 4.6.1 or higher.
* High performance: Speed comparable to the optimized built-in GUID generator in
  both single-threaded and multi-threaded scenarios under Windows and Linux.
* Hardware acceleration: Vector128 support for Equals method.
* Microsoft SQL Server support (`NewMsSqlUniqueIdentifier()`).
* Support for UUID version 4 (fully random UUID)
* Conversion from and to System.Guid
* .NET 8 AOT support
* Also available as [Entity Framework Core library][nuget_uuid7_efcore].

You can find packaged library at [NuGet][nuget_uuid7].


## Usage

To generate a new database-friendly UUID v7, simply call `NewUuid7` method:
```csharp
using System;
using Medo;

var uuid = Uuid7.NewUuid7();  // or 'Uuid7.NewGuid()'
Console.WriteLine($"UUID : {uuid}");
```

Alternatively, if a fully random UUID v4 is desired, call `NewUuid4` method:
```csharp
using System;
using Medo;

var uuid = Uuid7.NewUuid4();
Console.WriteLine($"UUID : {uuid}");
```

If higher performance is needed and per-thread seqencing is sufficient, you
can instantiate UUID directly:
```csharp
using System;
using Medo;

var uuid = new Uuid7();
Console.WriteLine($"UUID : {uuid}");
```


## Converting to Guid

Converting to and from `System.Guid` is a complicated story. There are two ways
it can be done. One is by preserving binary equivalency and that is what I
selected by default. Any time a conversion into `System.Guid` is done, all raw
bytes are the same but a textual representation between `Medo.Uuid7` and
`System.Guid` on little-endian platforms will differ.

For example, this code will retain binary compatibility during conversion.
```csharp
using Medo;

var uuid = Uuid7.NewUuid7();
Console.WriteLine($"{uuid}");

var guid = (Guid)uuid;
Console.WriteLine($"{guid}");
```

However, that means that textual respresentations look different since Microsoft
prints logically numeric Guid elements in little-endian order instead of
arguably more common big-endian order.
```plain
01904d33-d262-7531-b71c-05555c63df91
334d9001-62d2-3175-b71c-05555c63df91
```

If we want to preserve textual representation, we need to actually use
`ToGuid(matchGuidEndianness)` function overload as this one takes internal
Guid endianess into account.
```csharp
using Medo;

var uuid = Uuid7.NewUuid7();
Console.WriteLine($"{uuid}");

var guid = uuid.ToGuid(matchGuidEndianness: true);
Console.WriteLine($"{guid}");
```

Textual output in this case would be equal but at the cost of raw binary bytes
differing.
```plain
01904d33-d262-7531-b71c-05555c63df91
01904d33-d262-7531-b71c-05555c63df91
```

I view this as a damn-if-you-do-damn-if-you-don't scenario and I decided to be
damned in binary format.


### Configuration

#### Disable RNG Buffering

Buffering of random numbers significantly increases performance at the cost of
less frequent but bigger requests toward random number generator. If buffering
is not desired (e.g. only a small count of UUIDs is needed), you can disable it
using `UUID7_NO_RANDOM_BUFFER` preprocessor constant.

```plain
<PropertyGroup>
    <DefineConstants>UUID7_NO_RANDOM_BUFFER</DefineConstants>
</PropertyGroup>
```

Note that this will decrease performance significantly.


## UUID Format

The format of UUIDv7 is as specified below.

     0                   1                   2                   3
     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |                           unix_ts_ms                          |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |          unix_ts_ms           |  ver  |       rand_a          |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |var|                        rand_b                             |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |                            rand_b                             |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

*unix_tx_ms*:
48 bit big-endian unsigned number of Unix epoch timestamp.

*ver*:
4 bit UUIDv7 version. Always `0111`.

*rand_a*:
12 bits of pseudo-random data.

*var*:
2 bit variant. Always `10`.

*rand_b*:
Additional 62 bits of pseudo-random data.


### Implementation

As monotonicity is important for UUID version 7 generation, this implementation
implements most of [monotonic random counter][rfc9562#counters]
recommendations.

Implementation uses randomly seeded 26 bit monotonic counter (25 random bits + 1
rollover guard bit) with a 4-bit increment.

Counter uses 12-bits from rand_a field and it "steals" 14 bits from rand_b
field. Counter will have its 25 bits fully randomized each millisecond tick.
Within the same millisecond tick, counter will be randomly increased using 4 bit
increment.

In the case of multithreaded use, the counter seed is different for each thread.

In the worst case, this implementation guarantees at least 2^21 monotonically
increasing UUIDs per millisecond. Up to 2^23 monotonically increasing UUID
values per millisecond can be expected on average. Monotonic increase for each
generated value is guaranteed on per thread basis.

The last 48 bits are filled with random data that is different for each
generated UUID.

As each UUID uses 48 random bits in addition to 25 random bits from the seeded
counter, this means we have at least 73 bits of entropy (without taking 48-bit
timestamp into account).

With those implementation details in mind, the final layout is defined as below.

     0                   1                   2                   3
     0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |                           unix_ts_ms                          |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |          unix_ts_ms           |  ver  |        counter        |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |var|          counter          |            random             |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
    |                            random                             |
    +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

*unix_tx_ms*:
48 bit big-endian unsigned number of Unix epoch timestamp.

*ver*:
4 bit UUIDv7 version. Always `0111`.

*var*:
2 bit variant. Always `10`.

*counter*:
26 bit big-endian unsigned counter.

*random*:
48 bits of random data.


## Textual Representation

While this UUID should be handled and stored in its binary 128 bit form, it's
often useful to provide a textual representation.


### UUID Format

This is a standard hexadecimal representation of UUID with dashes separating
various components. Please note that this component separation doesn't
necessarily correlate with any internal fields.

Example:

    0185aee1-4413-7023-9109-bde493efe31d


### Id25

Alternative string representation is Id25 (Base-35), courtesy of [stevesimmons][git_stevesimmons_uuid7].
While I have seen similar encodings used before, his implementation is the first
one I saw being used on UUIDs. Since it uses only numbers and lowercase
characters, it actually retains lexicographical sorting property the default
UUID text format has.

UUID will always fit in 25 characters.

Example:

    0672016s27hx3fjxmn5ic1hzq


### Id22

If more compact string representation is needed, one can use Id22 (Base-58)
encoding. This is the same encoding Bitcoin uses for its keys.

UUID will always fit in 22 characters.

Example:

    1BuKkq6yWzmN2fCaHBjCRr



[rfc9562]: https://www.rfc-editor.org/rfc/rfc9562.html
[rfc9562#counters]: https://www.rfc-editor.org/rfc/rfc9562.html#name-monotonicity-and-counters
[nuget_uuid7]: https://www.nuget.org/packages/Medo.Uuid7/
[nuget_uuid7_efcore]: https://www.nuget.org/packages/Medo.Uuid7.EntityFrameworkCore/
[git_stevesimmons_uuid7]: https://github.com/stevesimmons/uuid7-csharp/
