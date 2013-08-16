namespace Serenity.Media
{
    /// <summary>
    ///   <see cref="Mp3Checker"/> results enumeration.</summary>
	public enum Mp3CheckResult
	{
        /// <summary>
        ///   Success, MP3 is valid</summary>
		OK,
        /// <summary>
        ///   Error on file access.</summary>
		FileAccessError,
        /// <summary>
        ///   File is too small to validate, not an MP3 probably</summary>
		TooSmallFile,
        /// <summary>
        ///   No MP3 headers found in the file</summary>
		CantLocateFrameHeader,
        /// <summary>
        ///   ID3v2 tag found, but its invalid</summary>
		ID3v2TagError,
        /// <summary>
        ///   A frame header is invalid</summary>
		InvalidFrameHeader,
        /// <summary>
        ///   A frame size is invalid</summary>
		InvalidFrameSize,
        /// <summary>
        ///   Not enough valid frame in MP3 (less than 10)</summary>
		NotEnoughFrames,
        /// <summary>
        ///   Too much garbage at start or at end</summary>
		TooMuchGarbageOrDamaged,
        /// <summary>
        ///   Frame headers mismatch, for example sampling</summary>
		FrameHeadersMismatch,
        /// <summary>
        ///   Frame bitrates mismatch (variable bit rate)</summary>
		FrameBitratesMismatch,
        /// <summary>
        ///   Mp3 file is a XING variable bit rate (XING header found)</summary>
		XingVariableBitrate,
        /// <summary>
        ///   Unexpected error during validation</summary>
		UnexpectedError,
        /// <summary>
        ///   File size mismatch</summary>
        FileSizeMismatch,
        /// <summary>
        ///   File size too high</summary>
        FileSizeTooHigh,
        /// <summary>
        ///   File size too low</summary>
        FileSizeTooLow,
        /// <summary>
        ///   Bitrate mismatch (e.g. min max is 128)</summary>
        BitrateMismatch,
        /// <summary>
        ///   Bitrate too high</summary>
        BitrateTooHigh,
        /// <summary>
        ///   Bitrate too low</summary>
        BitrateTooLow,
        /// <summary>
        ///   Duration mismatch (e.g. if min max is 60)</summary>
        DurationMismatch,
        /// <summary>
        ///   Duration too high</summary>
        DurationTooHigh,
        /// <summary>
        ///   Duration too low</summary>
        DurationTooLow
	}
}
