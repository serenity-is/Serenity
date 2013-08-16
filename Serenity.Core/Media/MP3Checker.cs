using System;
using System.IO;

namespace Serenity.Media
{
    /// <summary>
    ///   Checks an input data to see if it contains valid MP3 data and complies with constraints.</summary>
    public class Mp3Checker
    {
        /// <summary>Constant frame header size</summary>
        private const int HeaderSize = 4;
        /// <summary>Minimum mp3 data size that can be validated</summary>
        private const int MinBytesToScan = 255;
        /// <summary>Minimum number of frames that should exist for a valid MP3 file</summary>
        private const int MinValidFrameCount = 10;
        /// <summary>
        ///   Maximum amount of garbage allowed before or after file data</summary>
        private const int MaxGarbageAmount = 4096;

        /// <summary>Position of garbage at file start if exist</summary>
        private int garbagePositionStart;
        /// <summary>Amount of garbage at file start if exist</summary>
        private int garbageAmountStart;
        /// <summary>Poisiton of garbage at file end if exist</summary>
        private int garbagePositionEnd;
        /// <summary>Amount of garbage at file end if exist</summary>
        private int garbageAmountEnd;

        /// <summary>Temporary variable that will hold frame headers</summary>
        private ulong header;
        /// <summary>Temporary variable that will hold frame version</summary>
        private int versionIndex;
        /// <summary>Temporary variable that will hold layer index</summary>
        private int layerIndex;
        /// <summary>Temporary variable that will hold bitrate index</summary>
        private int bitrateIndex;
        /// <summary>Temporary variable that will hold frequency index</summary>
        private int frequencyIndex;
        /// <summary>Array that contains input data</summary>
        private byte[] data;
        /// <summary>Length of the input data</summary>
        private int length;
        /// <summary>Current position inside input data</summary>
        private int position;

        private int minFileSize;
        private int maxFileSize;
        private int minBitrate;
        private int maxBitrate;
        private int minDuration;
        private int maxDuration;

        private int fileSize;
        private int duration;
        private int bitrate;

        /// <summary>
        ///   Validates an MP3 data inside an array.</summary>
        /// <param name="data">
        ///   Data.</param>
        /// <returns>
        ///   One of <see cref="Mp3CheckResult"/> values. If valid, Mp3CheckResult.OK</returns>
        public Mp3CheckResult CheckBytes(byte[] data)
        {
            this.data = data;
            position = 0;
            length = data.Length;

            fileSize = length;

            // check maximum file size
            if (MaxFileSize != 0 && length > MaxFileSize)
                return Mp3CheckResult.FileSizeTooHigh;

            // check minimum file size (a constant value, not parameter)
            if (length < MinFileSize)
                return Mp3CheckResult.FileSizeTooLow;

            garbageAmountEnd = 0;
            garbageAmountStart = 0;

            // search for ID3v1 tag
            // if it exists should be at the end of the file and its length is 128 bytes
            // so its position should be file length - 128 and at that position we should
            // find "TAG" special value
            if (length >= 128 &&
                data[length - 128] == 0x54 && // T
                data[length - 127] == 0x41 && // A
                data[length - 126] == 0x47)   // G
            {
                // assume file length is 128 shorter
                length -= 128;
            }

            // search for ID3v2 tag
            // if it exists, should be at start and its length is variable and
            // this length is specified inside its header
            int ID3v2DataSize = 0;

            // check if first three characters are ID3 
            // this tag ensures ID3v2 exists at start
            if (length >= 4 && 
                data[0] == 0x49 && // I
                data[1] == 0x44 && // D
                data[2] == 0x33)   // 3
            {
                // need at least 10 bytes to detect ID3v2 length, if it actually exists
                if (length < 10)
                    return Mp3CheckResult.ID3v2TagError;

                // data size field is a 32 bits synchsafe integer, so MSB is zero for each 8 bit part of it,
                // thus its calculated like below
                ID3v2DataSize = data[9];
                ID3v2DataSize += 128 * data[8];
                ID3v2DataSize += 16384 * data[7];
                ID3v2DataSize += 2097152 * data[6];


                if ((data[5] & 0x10) != 0)
                    ID3v2DataSize += 20;
                else
                    ID3v2DataSize += 10;

                // ID3v2 exists but data size is lower than file is not valid
                if (length < ID3v2DataSize)
                    return Mp3CheckResult.ID3v2TagError;
            }

            // An MP3 that is shorter than 255 bytes is not likely
            if (length - ID3v2DataSize < MinBytesToScan)
                return Mp3CheckResult.TooSmallFile;

            position = ID3v2DataSize;

            // scan file to locate first frame header
            do
            {
                // try to load as if there is a header at this location
                // if no more data, LoadFrameHeader returns false
                if (!LoadFrameHeader())
                    return Mp3CheckResult.CantLocateFrameHeader;
                // if we found a valid header, proceed as many bytes as a header size and break the loop
                else if (IsValidFrameHeader())
                {
                    if (LoadXingVBRHeader(position))
                        return Mp3CheckResult.XingVariableBitrate;

                    break;
                }

                position++;
            }
            while (true);

            // if our posiiton is not 0 or there exists a ID3v2 and we are not at the end of it, than 
            // there is some garbage at the start of MP3
            garbageAmountStart = (position - ID3v2DataSize);
            if (garbageAmountStart > 0)
                garbagePositionStart = position;

            // count of frame headers located so far
            int countFrameHeaders = 0;

            // read first frame data, so we can compare them with other frames
            versionIndex = GetVersionIndex();
            layerIndex = GetLayerIndex();
            bitrateIndex = GetBitrateIndex();
            frequencyIndex = GetFrequencyIndex();

            // calculate some constants that will be used to find frame size here
            bitrate = MpegBitrates[versionIndex & 1, layerIndex - 1, bitrateIndex];
            int totalFrameBytes = 0;
            int samplingRate = MpegSamplingRates[versionIndex, frequencyIndex];

            // if bitrate or samplingRate is -1, then it is an unsupported format
            if (bitrate <= 0 || samplingRate <= 0)
                return Mp3CheckResult.InvalidFrameHeader;

            do
            {
                countFrameHeaders++;

                int padding = GetPaddingBit();
                int frameSize;
                if (layerIndex == 3) 
                    frameSize = (12 * bitrate * 1000 / samplingRate + padding) * 4;
                else if (layerIndex == 2 || (layerIndex == 1 && versionIndex == 3)) 
                    frameSize = 144 * bitrate * 1000 / samplingRate + padding;
                else 
                    frameSize = 72 * bitrate * 1000 / samplingRate + padding;

                if (frameSize < HeaderSize)
                    return Mp3CheckResult.InvalidFrameSize;

                position += frameSize;

                totalFrameBytes += frameSize;

                if ((!LoadFrameHeader() && IsValidFrameHeader()))
                {
                    // if a frame header canditate is not loaded, we reached end or this is not a valid frame
                    // (garbage or invalid). if so, ensure that a minimum number of valid frames are detected
                    if (countFrameHeaders < MinValidFrameCount)
                        return Mp3CheckResult.NotEnoughFrames;

                    // rest of the file is garbage
                    garbageAmountEnd = (length - position);
                    if (garbageAmountEnd > 0)
                        garbagePositionEnd = position;

                    if (garbageAmountStart + garbageAmountEnd > MaxGarbageAmount)
                        return Mp3CheckResult.TooMuchGarbageOrDamaged;

                    duration = (int)((double)totalFrameBytes / (double)bitrate / (double)(125));

                    return Mp3CheckResult.OK;
                }

                // ensure that rest of the frames has same structure with first frame, otherwise this
                // file is invalid or an Variable Bit Rate MP3, which we don't support (as it is not
                // supported by Windows Media Server)
                if (versionIndex != GetVersionIndex() ||
                    layerIndex != GetLayerIndex() ||
                    frequencyIndex != GetFrequencyIndex())
                    return Mp3CheckResult.FrameHeadersMismatch;

                if (bitrateIndex != GetBitrateIndex())
                    return Mp3CheckResult.FrameBitratesMismatch;
            } 
            while (true);
        }

        /// <summary>
        ///   Validates a file to see if it contains valid MP3 data.</summary>
        /// <param name="fileName">
        ///   Full path of the file to validate.</param>
        /// <returns>
        ///   One of <see cref="Mp3CheckResult"/> values. If successfull, Mp3CheckResult.OK</returns>
        public Mp3CheckResult CheckFile(string fileName)
        {
            byte[] data;
            FileStream fs = null;
            try
            {
                try
                {
                    fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                    data = new byte[(int)fs.Length];
                    fs.Read(data, 0, data.Length);
                }
                catch
                {
                    return Mp3CheckResult.FileAccessError;
                }
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                    fs = null;
                }
            }

            return CheckBytes(data);
        }

        private bool LoadFrameHeader()
        {
            if (position > length - HeaderSize)
                return false;

            header = (ulong)(((data[position] & 255) << 24) | 
                             ((data[position + 1] & 255) << 16) | 
                             ((data[position + 2] & 255) << 8) | 
                             ((data[position + 3] & 255)));

            return true;
        }

        private bool LoadXingVBRHeader(int position)
        {
            if (GetVersionIndex() == 3) // MPEG Version 1   
            {
                if (GetModeIndex() == 3) // Single Channel   
                {
                    position += 21;
                }
                else
                {
                    position += 36;
                }
            }
            else // MPEG Version 2.0 or 2.5                       
            {
                if (GetModeIndex() == 3)    // Single Channel
                {
                    position += 13;
                }
                else
                {
                    position += 21;
                }
            }

            if (position > length - 12)
                return false;

            // if variable bit rate, first 4 bytes should be 'Xing'
            if (data[position] == 88 && 
                data[position + 1] == 105 && 
                data[position + 2] == 110 && 
                data[position + 3] == 103)
                return true;

            return false;
        }

        /// <summary>
        ///   Checks if frame header seems like a valid one.</summary>
        /// <returns>
        ///   True if valid.</returns>
        /// <remarks>
        ///   Garbage can also seem to to be a valid frame header, because most of 4 byte random values can look
        ///   like a valid frame header, but if we calculate the frame size from the header, and find another valid
        ///   header next to the end of this frame, it lowers chance of it being random garbage.</remarks>
        private bool IsValidFrameHeader()
        {
            return (((GetFrameSync()      & 2047) == 2047) &&
                    ((GetVersionIndex()   &    3) !=    1) &&
                    ((GetLayerIndex()     &    3) !=    0) && 
                    ((GetBitrateIndex()   &   15) !=    0) &&
                    ((GetBitrateIndex()   &   15) !=   15) &&
                    ((GetFrequencyIndex() &    3) !=    3) &&
                    ((GetEmphasisIndex()  &    3) !=    2)    );
        }

        /// <summary>
        ///   Gets FrameSync from header.</summary>
        /// <returns>
        ///   FrameSync.</returns>
        private int GetFrameSync()
        {
            return (int)((header >> 21) & 2047); 
        }

        /// <summary>
        ///   Gets VersionIndex from header.</summary>
        /// <returns>
        ///   VersionIndex.</returns>
        /// <remarks>
        ///   <p>0 - MPEG v2.5</p>
        ///   <p>1 - Reserved</p>
        ///   <p>2 - MPEG v2.0</p>
        ///   <p>3 - MPEG v1.0</p></remarks>
        private int GetVersionIndex()  
        {
            return (int)((header >> 19) & 3);  
        }

        /// <summary>
        ///   Gets LayerIndex from header.</summary>
        /// <returns>
        ///   LayerIndex.</returns>
        /// <remarks>
        ///   <p>0 - Invalid</p>
        ///   <p>1 - MPEG Layer 3</p>
        ///   <p>2 - MPEG Layer 2</p>
        ///   <p>3 - MPEG Layer 1</p></remarks>
        private int GetLayerIndex()    
        { 
            return (int)((header >> 17) & 3);  
        }

        /// <summary>
        ///   Gets ProtectionBit from header.</summary>
        /// <returns>
        ///   ProtectionBit.</returns>
        private int GetProtectionBit() 
        { 
            return (int)((header >> 16) & 1);  
        }

        /// <summary>
        ///   Gets BitrateIndex from header.</summary>
        /// <returns>
        ///   BitrateIndex.</returns>       
        private int GetBitrateIndex()  
        { 
            return (int)((header >> 12) & 15); 
        }

        /// <summary>
        ///   Gets FrequencyIndex from header.</summary>
        /// <returns>
        ///   FrequencyIndex.</returns>       
        private int GetFrequencyIndex()
        { 
            return (int)((header >> 10) & 3);  
        }

        /// <summary>
        ///   Gets PaddingBit from header.</summary>
        /// <returns>
        ///   PaddingBit.</returns>              
        private int GetPaddingBit()    
        { 
            return (int)((header >> 9) & 1);  
        }

        /// <summary>
        ///   Gets PrivateBit from header.</summary>
        /// <returns>
        ///   PrivateBit.</returns>       
        private int GetPrivateBit()    
        { 
            return (int)((header >> 8) & 1);  
        }

        /// <summary>
        ///   Gets ModeIndex from header.</summary>
        /// <returns>
        ///   ModeIndex.</returns>       
        private int GetModeIndex()     
        { 
            return (int)((header >> 6) & 3);  
        }

        /// <summary>
        ///   Gets ModeExtIndex from header.</summary>
        /// <returns>
        ///   ModeExtIndex.</returns>       
        private int GetModeExtIndex()  
        { 
            return (int)((header >> 4) & 3);  
        }

        /// <summary>
        ///   Gets CopyrightBit from header.</summary>
        /// <returns>
        ///   CopyrightBit.</returns>       
        private int GetCopyrightBit()   
        { 
            return (int)((header >> 3) & 1);  
        }

        /// <summary>
        ///   Gets OriginalBit from header.</summary>
        /// <returns>
        ///   OriginalBit.</returns>       
        private int GetOrginalBit()    
        { 
            return (int)((header >> 2) & 1);  
        }

        /// <summary>
        ///   Gets EmphasisIndex from header.</summary>
        /// <returns>
        ///   EmphasisIndex.</returns>       
        private int GetEmphasisIndex() 
        { 
            return (int)(header & 3);  
        }

        /// <summary>
        ///   MPEG sampling rates. First dimension is VersionIndex, second is FrequencyIndex</summary>
        private static readonly int[,] MpegSamplingRates =    {    
            {32000, 16000,  8000}, // MPEG v2.5
            {   -1,    -1,    -1}, // reserved
            {22050, 24000, 16000}, // MPEG 2
            {44100, 48000, 32000}  // MPEG 1
        };

        /// <summary>
        ///   Mpeg bitrates. First dimension is (VersionIndex &amp; 1), second is (LayerIndex - 1)</summary>
        private static readonly int[, ,] MpegBitrates = {
            { // MPEG v2 & v2.5
                {0,  8, 16, 24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}, // Layer III
                {0,  8, 16, 24, 32, 40, 48, 56, 64, 80, 96,112,128,144,160,0}, // Layer II
                {0, 32, 48, 56, 64, 80, 96,112,128,144,160,176,192,224,256,0}  // Layer I
            },
            { // MPEG v1
                {0, 32, 40, 48, 56, 64, 80, 96,112,128,160,192,224,256,320,0}, // Layer III
                {0, 32, 48, 56, 64, 80, 96,112,128,160,192,224,256,320,384,0}, // Layer II
                {0, 32, 64, 96,128,160,192,224,256,288,320,352,384,416,448,0}  // Layer I
            }
        };

        /// <summary>
        ///   Writes data to file eliminating garbage data detected during scanning.</summary>
        /// <param name="data">
        ///   Data with garbage.</param>
        /// <param name="fileName">
        ///   Target file (required).</param>
        public void WriteBytesWithoutGarbage(byte[] data, string fileName)
        {
            if (fileName == null)
                throw new ArgumentNullException("fileName");

            FileStream newFile = new FileStream(fileName, FileMode.Create);
            try
            {
                int dataPosition = 0;

                if (garbageAmountStart > 0)
                {
                    if (garbagePositionStart > 0)
                        newFile.Write(data, 0, garbagePositionStart);

                    dataPosition = garbagePositionStart + garbageAmountStart;
                }

                if (garbageAmountEnd > 0 && garbagePositionEnd > dataPosition)
                {
                    newFile.Write(data, dataPosition, garbagePositionEnd - dataPosition);
                    dataPosition = garbagePositionEnd + garbageAmountEnd;
                }

                newFile.Write(data, dataPosition, data.Length - dataPosition);
            }
            finally
            {
                newFile.Close();
            }
        }

        /// <summary>
        ///   Gets garbage amount at start, detected during scan</summary>
        public int GarbageAmountStart
        {
            get { return garbageAmountStart; }
        }

        /// <summary>
        ///   Gets garbage position at start, detected during scan</summary>
        public int GarbagePositionStart
        {
            get { return garbagePositionStart; }
        }

        /// <summary>
        ///   Gets garbage amount at end, detected during scan</summary>
        public int GarbageAmountEnd
        {
            get { return garbageAmountEnd; }
        }

        /// <summary>
        ///   Gets garbage position at end, detected during scan</summary>
        public int GarbagePositionEnd
        {
            get { return garbagePositionEnd; }
        }

        /// <summary>
        ///   Gets file size.</summary>
        public int FileSize
        {
            get { return fileSize; }
        }

        /// <summary>
        ///   Gets mp3 duration, detected during scan.</summary>
        public int Duration
        {
            get { return duration; }
        }

        /// <summary>
        ///   Gets mp3 bitrate, detected during scan.</summary>
        public int Bitrate
        {
            get { return bitrate; }
        }

        /// <summary>
        ///   Gets/sets min file size.</summary>
        public int MinFileSize
        {
            get { return minFileSize; }
            set { minFileSize = value; }
        }

        /// <summary>
        ///   Gets/sets max file size.</summary>
        public int MaxFileSize
        {
            get { return maxFileSize; }
            set { maxFileSize = value; }
        }

        /// <summary>
        ///   Gets/sets min bitrate.</summary>
        public int MinBitrate
        {
            get { return minBitrate; }
            set { minBitrate = value; }
        }

        /// <summary>
        ///   Gets/sets max bitrate.</summary>
        public int MaxBitrate
        {
            get { return maxBitrate; }
            set { maxBitrate = value; }
        }

        /// <summary>
        ///   Gets/sets min duration in seconds.</summary>
        public int MinDuration
        {
            get { return minDuration; }
            set { minDuration = value; }
        }

        /// <summary>
        ///   Gets/sets max duration in seconds.</summary>
        public int MaxDuration
        {
            get { return maxDuration; }
            set { maxDuration = value; }
        }

        /// <summary>
        ///   Formats an error message returned from CheckBytes operation.
        ///   This method should be called just after CheckBytes, because it uses 
        ///   contstraints and validation results to format error message.</summary>
        /// <param name="error">
        ///   Mp3CheckResult.</param>
        /// <returns>
        ///   Formatted error message.</returns>
        public string FormatErrorMessage(Mp3CheckResult error)
        {
            return
                String.Format(
                    Mp3Checker.CheckErrorMessages[(int)error],
                    FileSize, // 0
                    Bitrate, // 1
                    Duration, // 2
                    MinFileSize, // 3
                    MaxFileSize, // 4
                    MinBitrate, // 5
                    MaxBitrate, // 6
                    MinDuration, // 7
                    MaxDuration); // 8
        }

        /// <summary>
        ///   Set of error messages used by FormatErrorMessage.</summary>
        private static readonly LocalText[] CheckErrorMessages = new LocalText[] 
        { 
            // OK
            "cms.mp3_check_result.ok", 
            // File access error!
            "cms.mp3_check_result.file_access_error",
            // File is too small ({0} bytes), Can't be MP3!
            "cms.mp3_check_result.too_small_file", 
            // MP3 header not found. File is not an MP3 or corrupted!
            "cms.mp3_check_result.cant_locate_frame_header", 
            // ID3v2 tag is corrupted. File is not an MP3 or corrupted!                    
            "cms.mp3_check_result.id3v2_tag_error", 
            // An invalid MP3 header detected. File is not an MP3 or corrupted!
            "cms.mp3_check_result.invalid_frame_header", 
            // An invalid MP3 header size detected. File is not an MP3 or corrupted!
            "cms.mp3_check_result.invalid_frame_size", 
            // Not enough MP3 headers in file. File is not an MP3 or corrupted!
            "cms.mp3_check_result.not_enough_frames", 
            // Too much garbage in file. File is not an MP3 or corrupted!
            "cms.mp3_check_result.too_much_garbage_or_damaged", 
            // MP3 headers in file doesn't match. File is not an MP3 or corrupted!
            "cms.mp3_check_result.frame_headers_mismatch", 
            // This is a variable bitrate MP3. This MP3 type is unsupported!
            "cms.mp3_check_result.frame_bitrates_mismatch", 
            // This is a variable bitrate MP3. This MP3 type is unsupported!
            "cms.mp3_check_result.xing_variable_bitrate", 
            // An unexpected error occured during validation!
            "cms.mp3_check_result.unexpected_error", 
            // MP3 file size mismatch
            "cms.mp3_check_result.file_size_mismatch", 
            // MP3 file size too high
            "cms.mp3_check_result.file_size_too_high", 
            // MP3 file size too low
            "cms.mp3_check_result.file_size_too_low", 
            // MP3 bitrate mismatch
            "cms.mp3_check_result.bitrate_mismatch", 
            // MP3 bitrate too high
            "cms.mp3_check_result.bitrate_too_high", 
            // MP3 bitrate too low
            "cms.mp3_check_result.bitrate_too_low", 
            // MP3 duration mismatch
            "cms.mp3_check_result.duration_mismatch", 
            // MP3 duration too high
            "cms.mp3_check_result.duration_too_high", 
            // MP3 duration too low
            "cms.mp3_check_result.duration_too_low"
        };
    }
}