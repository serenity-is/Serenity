using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Collections;

namespace Serenity.Web
{
    /// <summary>
    ///		<disclaimer>Copyright (c)2006 by Koistya `Navin - http://navin.biz </disclaimer>
    ///		<meta-data>
    ///			<version>1.0.0</version>
    ///			<author>
    ///				<name>Koistya `Navin</name>
    ///				<e-mail>koistya@gmail.com</e-mail>
    ///			</author>
    ///		</meta-data>
    ///		<description>
    ///			<p>This is a small utility class which is used                   
    ///         to determine the basic data from an SWF file header.</p>
    ///		</description>
    /// </summary>
    public class SwfInfo
    {
        private int frameCount;
        private double frameRate;
        private string magicBytes;
        private bool isCompressed;
        private int size;
        private int height;
        private int width;
        private short version;

        /// <summary>
        ///   Flash movie native frame-rate</summary>
        public double FrameRate
        {
            get { return frameRate; }
        }

        /// <summary>
        ///   Flash movie total frames</summary>
        public int FrameCount
        {
            get { return frameCount; }
        }

        /// <summary>
        ///   Flag to indicate a compressed file (CWS)</summary>
        public bool IsCompressed
        {
            get { return isCompressed; }
        }

        /// <summary>
        /// Magic bytes in a SWF file (FWS or CWS)
        /// </summary>
        public string MagicBytes
        {
            get { return magicBytes; }
        }

        /// <summary>
        ///   Flash major version</summary>
        public short Version
        {
            get { return version; }
        }

        /// <summary>
        ///   Uncompressed file size (in bytes)</summary>
        public int Size
        {
            get { return size; }
        }

        /// <summary>
        ///   Flash movie native width</summary>
        public int Width
        {
            get { return width; }
        }

        /// <summary>
        ///   Flash movie native height</summary>
        public int Height
        {
            get { return height; }
        }

        /// <summary>
        ///   Creates a SwfInfo object for specified file.</summary>
        /// <param name="filename">
        ///   Filename (required).</param>
        public SwfInfo(string filename)
        {
            LoadSwf(File.Open(filename, FileMode.Open));
        }

        /// <summary>
        ///   Creates a SwfInfo object for specified stream.</summary>
        /// <param name="stream">
        ///   Stream (required). This stream is closed by SwfInfo object!</param>
        public SwfInfo(Stream stream)
        {
            LoadSwf(stream);
        }

        /// <summary>
        ///   Loads SWF file from a stream and checks it. Warning: As it uses a BinaryReader, it
        ///   closes the stream afterwards.</summary>
        /// <param name="stream">
        ///   Stream.</param>
        public void LoadSwf(Stream stream)
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                // Read MAGIC FIELD
                magicBytes = new String(reader.ReadChars(3));

                if (magicBytes != "FWS" && magicBytes != "CWS")
                    throw new Exception(" is not a valid/supported SWF file.");

                // Compression
                isCompressed = magicBytes.StartsWith("C") ? true : false;

                // Version
                version = Convert.ToInt16(reader.ReadByte());

                // Size
                size = 0;

                // 4 LSB-MSB
                for (int i = 0; i < 4; i++)
                {
                    byte t = reader.ReadByte();
                    size += t << (8 * i);
                }

                // RECT... we will "simulate" a stream from now on... read remaining file
                byte[] buffer = reader.ReadBytes((int)size);

                // First decompress GZ stream
                if (isCompressed)
                {
                    // Let's set GZip magic bytes which GZipStream can process
                    Array.Resize(ref buffer, buffer.Length + 8);
                    for (int i = buffer.Length - 1; i > 9; i--)
                    {
                        buffer[i] = buffer[i - 8];
                    }
                    ((Array)(new byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0 })).
                        CopyTo(buffer, 0);

                    MemoryStream ms = new MemoryStream(buffer);

                    GZipStream gzip = new GZipStream(ms, CompressionMode.Decompress);

                    byte[] decompressedBuffer = new byte[buffer.Length + 1000000];

                    int gzipLength = ReadAllBytesFromStream(gzip, decompressedBuffer);

                    gzip.Close();
                    ms.Close();

                    Array.Resize(ref buffer, gzipLength);
                    Array.Resize(ref decompressedBuffer, gzipLength);
                    decompressedBuffer.CopyTo(buffer, 0);

                    Array.Clear(decompressedBuffer, 0, decompressedBuffer.Length);
                }

                byte cbyte = buffer[0];
                int bits = (int)cbyte >> 3;

                Array.Reverse(buffer);
                Array.Resize(ref buffer, buffer.Length - 1);
                Array.Reverse(buffer);

                BitArray cval = new BitArray(bits, false);

                // Current byte
                cbyte &= 7;
                cbyte <<= 5;

                // Current bit (first byte starts off already shifted)
                int cbit = 2;

                // Must get all 4 values in the RECT
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < cval.Count; j++)
                    {
                        if ((cbyte & 128) > 0)
                        {
                            cval[j] = true;
                        }

                        cbyte <<= 1;
                        cbyte &= 255;
                        cbit--;

                        // We will be needing a new byte if we run out of bits
                        if (cbit < 0)
                        {
                            cbyte = buffer[0];

                            Array.Reverse(buffer);
                            Array.Resize(ref buffer, buffer.Length - 1);
                            Array.Reverse(buffer);

                            cbit = 7;
                        }
                    }

                    // O.k. full value stored... calculate
                    int c = 1;
                    int val = 0;

                    for (int j = cval.Count - 1; j >= 0; j--)
                    {
                        if (cval[j])
                        {
                            val += c;
                        }
                        c *= 2;
                    }

                    val /= 20;

                    switch (i)
                    {
                        case 0:
                            // tmp value
                            width = val;
                            break;
                        case 1:
                            width = val - width;
                            break;
                        case 2:
                            // tmp value
                            height = val;
                            break;
                        case 3:
                            height = val - height;
                            break;
                    }

                    cval.SetAll(false);
                }

                // Frame rate
                frameRate += buffer[1];
                frameRate += Convert.ToSingle(buffer[0] / 100);

                // Frames
                frameCount += BitConverter.ToInt16(buffer, 2);
            }
        }

        /// <summary>
        ///   Reads all bytes from a stream and returns number of bytes read.</summary>
        /// <param name="stream">
        ///   Stream (required).</param>
        /// <param name="buffer">
        ///   Buffer to read bytes into (required).</param>
        /// <returns>
        ///   Number of bytes read.</returns>
        private static int ReadAllBytesFromStream(Stream stream, byte[] buffer)
        {
            // Use this method is used to read all bytes from a stream.
            int offset = 0;
            int totalCount = 0;
            while (true)
            {
                int bytesRead = stream.Read(buffer, offset, 100);
                if (bytesRead == 0)
                    break;
                offset += bytesRead;
                totalCount += bytesRead;
            }
            return totalCount;
        }
    }
}