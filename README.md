Medo.Uuid7
==========

This project is implementation of UUID version 7 algorithm as defined in
[New UUID Formats draft 04 RFC](https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html).


## Format

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
implements most of [monotonic random counter](https://www.ietf.org/archive/id/draft-peabody-dispatch-new-uuid-format-04.html#monotonicity_counters)
recommendations.

Implementation uses randomly seeded 26 bit monotonic counter (25 random bits + 1
rollover guard bit) with a 4-bit increment.

Counter uses 12-bits from rand_a field and it "steals" 14 bits from rand_b
field. Counter will have its 25 bits fully randomized each millisecond tick.

Within the same millisecond tick, counter will be increased using the lowest 4
bits of current nanosecond-resolution time as its increment. While this is not
strictly random as recommended, it should be sufficiently unguessable.

In the case of multithreaded use, the counter seed is different for each thread.

The last 48 bits are filled with random data that is different for each
generated UUID.

As each UUID uses 48 random bits in addition to at least 21 bits of randomly
seeded counter (25 bits with up to 4-bit increment), this means we have at least
69 bits of entropy (and that is without taking 48-bit timestamp into account).

As long as there is no more than 2^21 UUIDs generated per millisecond each
thread will produce monotonically increasing UUID values.

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

Alternative string representation is Id25 (Base-35), courtesy of [stevesimmons](https://github.com/stevesimmons/uuid7-csharp/).
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
