using Serenity.TypeScript;

namespace Serenity.TypeScript;

partial class Scanner
{

    // Converts a bigint literal string, e.g. `0x1234n`,
    // to its decimal string representation, e.g. `4660`.
    string ParsePseudoBigInt(string stringValue)
    {
        int log2Base;
        switch ((int)stringValue[1])
        { // "x" in "0x123"
            case CharacterCodes.b:
            case CharacterCodes.B: // 0b or 0B
                log2Base = 1;
                break;
            case CharacterCodes.o:
            case CharacterCodes.O: // 0o or 0O
                log2Base = 3;
                break;
            case CharacterCodes.x:
            case CharacterCodes.X: // 0x or 0X
                log2Base = 4;
                break;
            default: // already in decimal; omit trailing "n"
                var nIndex = stringValue.Length - 1;
                // Skip leading 0s
                var nonZeroStart = 0;
                while (stringValue[nonZeroStart] == CharacterCodes._0)
                {
                    nonZeroStart++;
                }
                return nonZeroStart < nIndex ? stringValue[nonZeroStart..nIndex] : "0";
        }

        // Omit leading "0b", "0o", or "0x", and trailing "n"
        var startIndex = 2;
        var endIndex = stringValue.Length - 1;
        var bitsNeeded = (endIndex - startIndex) * log2Base;
        // Stores the value specified by the string as a LE array of 16-bit integers
        // using Uint16 instead of Uint32 so combining steps can use bitwise operators
        var segments = new ushort[((bitsNeeded >>> 4) + ((bitsNeeded & 15) != 0 ? 1 : 0))];
        var bitOffset = 0;
        // Add the digits, one at a time
        for (var i = endIndex - 1; i >= startIndex; i--, bitOffset += log2Base)
        {
            var segment = bitOffset >>> 4;
            var digitChar = stringValue[i];
            // Find character range: 0-9 < A-F < a-f
            var digit = digitChar <= CharacterCodes._9
                ? digitChar - CharacterCodes._0
                : 10 + digitChar -
                    (digitChar <= CharacterCodes.F ? CharacterCodes.A : CharacterCodes.a);
            var shiftedDigit = digit << (bitOffset & 15);
            segments[segment] |= (ushort)shiftedDigit;
            var residual = shiftedDigit >>> 16;
            if (residual != 0) segments[segment + 1] |= (ushort)residual; // overflows segment
        }
        // Repeatedly divide segments by 10 and add remainder to base10Value
        var base10Value = "";
        var firstNonzeroSegment = segments.Length - 1;
        var segmentsRemaining = true;
        while (segmentsRemaining)
        {
            var mod10 = 0;
            segmentsRemaining = false;
            for (var segment = firstNonzeroSegment; segment >= 0; segment--)
            {
                var newSegment = mod10 << 16 | segments[segment];
                var segmentValue = (newSegment / 10) | 0;
                segments[segment] = (ushort)segmentValue;
                mod10 = newSegment - segmentValue * 10;
                if (segmentValue != 0 && !segmentsRemaining)
                {
                    firstNonzeroSegment = segment;
                    segmentsRemaining = true;
                }
            }
            base10Value = mod10 + base10Value;
        }
        return base10Value;
    }

}