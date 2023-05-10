namespace Serenity.Data;

/// <summary>
///   This class is based on the Base32 class from DC# 0.31
///   All credits to Jonathan Bindel.</summary>
public sealed class Base32
{
    /// <summary>
    ///   Solo constructor is private to prevent external instantiation of this class</summary>
    Base32()
    {
    }

    // lookup table used to encode() groups of 5 bits of data
    const string base32Chars = "abcdefghijklmnopqrstuvwxyz234567";

    const byte XX = 255;

    // lookup table used to decode() characters in Base32 strings
    static readonly byte[] base32Lookup = {
        26, 27, 28, 29, 30, 31, XX, XX, XX, XX, XX, XX, XX, XX, //   23456789:;<=>?
			XX, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, // @ABCDEFGHIJKLMNO
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, XX, XX, XX, XX, XX, // PQRSTUVWXYZ[\]^_
			XX, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, // `abcdefghijklmno
			15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 // pqrstuvwxyz
		};

    static readonly int base32LookupLength = base32Lookup.Length;

    // messages for Illegal Parameter Exceptions in decode()
    const string errorCanonicalLength = "non canonical Base32 string length";
    const string errorCanonicalEnd = "non canonical bits at end of Base32 string";
    const string errorInvalidChar = "invalid character in Base32 string";

    /// <summary>
    ///   Encode a buffer using the default options</summary>
    /// <param name="bytes">
    ///   The buffer (required).</param>
    /// <returns>
    ///   An encoded string that holds the contents of the inputs</returns>
    public static string Encode(byte[] bytes)
    {
        if (bytes == null)
            throw new ArgumentNullException("bytes");

        int len = bytes.Length;
        StringBuilder base32 = new((len * 8 + 4) / 5);

        int currByte, digit, i = 0;
        while (i < len)
        {
            // INVARIANTS FOR EACH STEP n in [0..5[; digit in [0..31[;
            // The remaining n bits are already aligned on top positions
            // of the 5 least bits of digit, the other bits are 0.
            ////// STEP n = 0; insert new 5 bits, leave 3 bits
            currByte = bytes[i++] & 255;
            base32.Append(base32Chars[currByte >> 3]);
            digit = (currByte & 7) << 2;
            if (i >= len)
            { // put the last 3 bits
                base32.Append(base32Chars[digit]);
                break;
            }
            ////// STEP n = 3: insert 2 new bits, then 5 bits, leave 1 bit
            currByte = bytes[i++] & 255;
            base32.Append(base32Chars[digit | (currByte >> 6)]);
            base32.Append(base32Chars[(currByte >> 1) & 31]);
            digit = (currByte & 1) << 4;
            if (i >= len)
            { // put the last 1 bit
                base32.Append(base32Chars[digit]);
                break;
            }
            ////// STEP n = 1: insert 4 new bits, leave 4 bit
            currByte = bytes[i++] & 255;
            base32.Append(base32Chars[digit | (currByte >> 4)]);
            digit = (currByte & 15) << 1;
            if (i >= len)
            { // put the last 4 bits
                base32.Append(base32Chars[digit]);
                break;
            }
            ////// STEP n = 4: insert 1 new bit, then 5 bits, leave 2 bits
            currByte = bytes[i++] & 255;
            base32.Append(base32Chars[digit | (currByte >> 7)]);
            base32.Append(base32Chars[(currByte >> 2) & 31]);
            digit = (currByte & 3) << 3;
            if (i >= len)
            { // put the last 2 bits
                base32.Append(base32Chars[digit]);
                break;
            }
            ///// STEP n = 2: insert 3 new bits, then 5 bits, leave 0 bit
            currByte = bytes[i++] & 255;
            base32.Append(base32Chars[digit | (currByte >> 5)]);
            base32.Append(base32Chars[currByte & 31]);
            //// This point is reached for len multiple of 5
        }
        string s = base32.ToString();
        return s;
    }

    /// <summary>
    ///   Decode a Base32 string into a byte array</summary>
    /// <param name="base32">
    ///   The Base32 string (required)</param>
    /// <returns>
    ///   The decoded byte array</returns>
    public static byte[] Decode(string base32)
    {
        if (base32 == null)
            throw new ArgumentNullException("base32");


        int len = base32.Length;
        // Note that the code below detects could detect non canonical
        // Base32 length within the loop. However canonical Base32 length
        // can be tested before entering the loop.
        // A canonical Base32 length modulo 8 cannot be:
        // 1 (aborts discarding 5 bits at STEP n=0 which produces no byte),
        // 3 (aborts discarding 7 bits at STEP n=2 which produces no byte),
        // 6 (aborts discarding 6 bits at STEP n=1 which produces no byte)
        // So these tests could be avoided within the loop.
        switch (len % 8)
        { // test the length of last subblock
            case 1: //  5 bits in subblock:  0 useful bits but 5 discarded
            case 3: // 15 bits in subblock:  8 useful bits but 7 discarded
            case 6: // 30 bits in subblock: 24 useful bits but 6 discarded
                throw new ArgumentException(errorCanonicalLength);
        }

        byte[] bytes = new byte[len * 5 / 8];
        int offset = 0, i = 0, lookup;
        byte nextByte, digit;

        // Also the code below does test that other discarded bits
        // (1 to 4 bits at end) are effectively 0.
        while (i < len)
        {
            // Read the 1st char in a 8-chars subblock
            // check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 0: leave 5 bits
            nextByte = (byte)(digit << 3);
            // Assert(i < base32.length) // tested before loop
            // Read the 2nd char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 5: insert 3 bits, leave 2 bits
            bytes[offset++] = (byte)(nextByte | (digit >> 2));
            nextByte = (byte)((digit & 3) << 6);
            if (i >= len)
            {
                if (nextByte != 0)
                {
                    throw new ArgumentException(errorCanonicalEnd);
                }
                break; // discard the remaining 2 bits
            }
            // Read the 3rd char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 2: leave 7 bits
            nextByte |= (byte)(digit << 1);
            // Assert(i < base32.length) // tested before loop
            // Read the 4th char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 7: insert 1 bit, leave 4 bits
            bytes[offset++] = (byte)(nextByte | (digit >> 4));
            nextByte = (byte)((digit & 15) << 4);
            if (i >= len)
            {
                if (nextByte != 0)
                {
                    throw new ArgumentException(errorCanonicalEnd);
                }
                break; // discard the remaining 4 bits
            }
            // Read the 5th char in a 8-chars subblock
            // Assert that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 4: insert 4 bits, leave 1 bit
            bytes[offset++] = (byte)(nextByte | (digit >> 1));
            nextByte = (byte)((digit & 1) << 7);
            if (i >= len)
            {
                if (nextByte != 0)
                {
                    throw new ArgumentException(errorCanonicalEnd);
                }
                break; // discard the remaining 1 bit
            }
            // Read the 6th char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 1: leave 6 bits
            nextByte |= (byte)(digit << 2);
            // Assert(i < base32.length) // tested before loop
            // Read the 7th char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 6: insert 2 bits, leave 3 bits
            bytes[offset++] = (byte)(nextByte | (digit >> 3));
            nextByte = (byte)((digit & 7) << 5);
            if (i >= len)
            {
                if (nextByte != 0)
                {
                    throw new ArgumentException(errorCanonicalEnd);
                }
                break; // discard the remaining 3 bits
            }
            // Read the 8th char in a 8-chars subblock
            // Check that chars are not outside the lookup table and valid
            lookup = base32[i++] - '2';
            if (lookup < 0 || lookup >= base32LookupLength)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            digit = base32Lookup[lookup];
            if (digit == XX)
            {
                throw new ArgumentException(errorInvalidChar);
            }
            //// STEP n = 3: insert 5 bits, leave 0 bit
            bytes[offset++] = (byte)(nextByte | digit);
            //// possible end of string here with no trailing bits
        }
        // On loop exit, discard trialing n bits.
        return bytes;
    }
}