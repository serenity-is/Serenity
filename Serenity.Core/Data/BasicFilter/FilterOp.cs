namespace Serenity.Data
{
    /// <summary>
    ///   Filter operator types for PI Filter Panel</summary>
    public enum FilterOp
    {
        /// <summary>true</summary>
        True,
        /// <summary>false</summary>
        False,
        /// <summary>contains</summary>
        Contains,
        /// <summary>not contains</summary>
        NotContains,
        /// <summary>starts with</summary>
        StartsWith,
        /// <summary>starts with</summary>
        EndsWith,
        /// <summary>like</summary>
        Like,
        /// <summary>not like</summary>
        NotLike,
        /// <summary>equal to</summary>
        EQ,
        /// <summary>not equal to</summary>
        NE,
        /// <summary>greater than</summary>
        GT,
        /// <summary>greater than or equal to</summary>
        GE,
        /// <summary>less than</summary>
        LT,
        /// <summary>less than or equal to</summary>
        LE,
        /// <summary>between</summary>
        BW,
        /// <summary>between</summary>
        NotBW,
        /// <summary>between</summary>
        IN,
        /// <summary>between</summary>
        NotIN,
        /// <summary>is null</summary>
        IsNull,
        /// <summary>is not null</summary>
        IsNotNull
    }
}