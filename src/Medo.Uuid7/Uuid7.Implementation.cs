/* Josip Medved <jmedved@jmedved.com> * www.medo64.com * MIT License */

namespace Medo;

using System;
using System.Runtime.CompilerServices;

public readonly partial struct Uuid7 {

    #region Implementation (v7)

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void FillBytes7(ref byte[] bytes, long ticks, ref long lastMillisecond, ref long millisecondCounter, ref uint monotonicCounter) {
        //   0                   1                   2                   3
        //   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |                       unix_ts_ms[47:16]                       |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |       unix_ts_ms[15:0]        |  ver  |    counter[25:14]     |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |var|       counter[13:0]       |            random             |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |                            random                             |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

        var millisecond = unchecked(ticks / TicksPerMillisecond);
        var msCounter = millisecondCounter;

        var newStep = (millisecond != lastMillisecond);
        if (newStep) {  // we need to switch millisecond (i.e. counter)
            lastMillisecond = millisecond;
            long ms;
            ms = unchecked(millisecond - UnixEpochMilliseconds);
            if (msCounter < ms) {  // normal time progression
                msCounter = ms;
            } else {  // time went backward, just increase counter
                unchecked { msCounter++; }
            }
            millisecondCounter = msCounter;
        }

        // Timestamp
        bytes[0] = (byte)(msCounter >> 40);
        bytes[1] = (byte)(msCounter >> 32);
        bytes[2] = (byte)(msCounter >> 24);
        bytes[3] = (byte)(msCounter >> 16);
        bytes[4] = (byte)(msCounter >> 8);
        bytes[5] = (byte)msCounter;

        // Randomness
        uint monoCounter;
        if (newStep) {
            GetRandomBytes(ref bytes, 6, 10);
            monoCounter = (uint)(((bytes[6] & 0x07) << 22) | (bytes[7] << 14) | ((bytes[8] & 0x3F) << 8) | bytes[9]);  // to use as monotonic random for future calls; total of 26 bits but only 25 are used initially with upper 1 bit reserved for rollover guard
        } else {
            GetRandomBytes(ref bytes, 9, 7);
            monoCounter = unchecked(monotonicCounter + ((uint)bytes[9] >> 4) + 1);  // 4 bit random increment will reduce overall counter space by 3 bits on average (to 2^22 combinations)
            bytes[7] = (byte)(monoCounter >> 14);    // bits 14:21 of monotonics counter
            bytes[9] = (byte)(monoCounter);          // bits 0:7 of monotonics counter
        }
        monotonicCounter = monoCounter;

        //Fixup
        bytes[6] = (byte)(0x70 | ((monoCounter >> 22) & 0x0F));  // set 4-bit version + bits 22:25 of monotonics counter
        bytes[8] = (byte)(0x80 | ((monoCounter >> 8) & 0x3F));   // set 2-bit variant + bits 8:13 of monotonics counter
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
#else
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
    private static void FillBytes7MsSql(ref byte[] bytes, long ticks, ref long lastMillisecond, ref long millisecondCounter, ref uint monotonicCounter) {
        //   0                   1                   2                   3
        //   0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |                            random                             |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |            random             |  ver  |     counter[11:0]     |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |var|      counter[25:12]       |       unix_ts_ms[47:32]       |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        //  |                       unix_ts_ms[31:0]                        |
        //  +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

        var millisecond = unchecked(ticks / TicksPerMillisecond);
        var msCounter = millisecondCounter;

        var newStep = (millisecond != lastMillisecond);
        if (newStep) {  // we need to switch millisecond (i.e. counter)
            lastMillisecond = millisecond;
            long ms;
            ms = unchecked(millisecond - UnixEpochMilliseconds);
            if (msCounter < ms) {  // normal time progression
                msCounter = ms;
            } else {  // time went backward, just increase counter
                unchecked { msCounter++; }
            }
            millisecondCounter = msCounter;
        }

        // Timestamp
        bytes[10] = (byte)(msCounter >> 40);
        bytes[11] = (byte)(msCounter >> 32);
        bytes[12] = (byte)(msCounter >> 24);
        bytes[13] = (byte)(msCounter >> 16);
        bytes[14] = (byte)(msCounter >> 8);
        bytes[15] = (byte)msCounter;

        // Randomness
        uint monoCounter;
        if (newStep) {
            GetRandomBytes(ref bytes, 0, 10);
            monoCounter = (uint)(((bytes[8] & 0x1F) << 20) | (bytes[9] << 12) | ((bytes[6] & 0x0F) << 8) | bytes[7]);  // to use as monotonic random for future calls; total of 26 bits but only 25 are used initially with upper 1 bit reserved for rollover guard
        } else {
            GetRandomBytes(ref bytes, 0, 7);
            monoCounter = unchecked(monotonicCounter + ((uint)bytes[7] >> 4) + 1);  // 4 bit random increment will reduce overall counter space by 3 bits on average (to 2^22 combinations)
            bytes[9] = (byte)(monoCounter >> 12);  // bits 12:19 of monotonics counter
            bytes[7] = (byte)(monoCounter);        // bits 0:7 of monotonics counter
        }
        monotonicCounter = monoCounter;

        //Fixup
        bytes[6] = (byte)(0x70 | ((monoCounter >> 8) & 0x0F));   // set 4-bit version + bits 8:11 of monotonics counter
        bytes[8] = (byte)(0x80 | ((monoCounter >> 20) & 0x3F));  // set 2-bit variant + bits 20:25 of monotonics counter
    }


    [ThreadStatic]
    private static long PerThreadLastMillisecond;  // real time in milliseconds since 0001-01-01

    [ThreadStatic]
    private static long PerThreadMillisecondCounter;  // usually real time but doesn't go backward

    [ThreadStatic]
    private static uint PerThreadMonotonicCounter;  // counter that gets embedded into UUID

    private static readonly object NonThreadedSyncRoot = new();  // sync root for all static counters
    private static long NonThreadedLastMillisecond;  // real time in milliseconds since 0001-01-01
    private static long NonThreadedMillisecondCounter;  // usually real time but doesn't go backward
    private static uint NonThreadedMonotonicCounter;  // counter that gets embedded into UUID

    #endregion Implementation (v7)


    #region Implementation (v4)

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void FillBytes4(ref byte[] bytes) {
        GetRandomBytes(ref bytes, 0, 16);

        //Fixup
        bytes[6] = (byte)(0x40 | (bytes[6] & 0x0F));  // set 4-bit version
        bytes[8] = (byte)(0x20 | (bytes[8] & 0x3F));  // set 2-bit variant
    }

    #endregion Implementation (v4)

}
