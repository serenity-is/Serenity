namespace Serenity.Services;

/// <summary>
/// Generic validation error mostly used by services.
/// </summary>
/// <seealso cref="Exception" />
public class ValidationError : Exception, IIsSensitiveMessage
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    public ValidationError() : base() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ValidationError(string message) : base(message) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public ValidationError(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationError(string errorCode, string errorMessage)
        : this(errorCode, null, errorMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="errorMessage">The error message.</param>
    public ValidationError(string errorCode, string? arguments, string errorMessage) : base(errorMessage)
    {
        ErrorCode = errorCode;
        Arguments = arguments;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> class.
    /// </summary>
    /// <param name="errorCode">The error code.</param>
    /// <param name="arguments">The arguments.</param>
    /// <param name="errorMessageFormat">The error message format.</param>
    /// <param name="formatArgs">The format arguments.</param>
    public ValidationError(string errorCode, string? arguments, string errorMessageFormat, params object[] formatArgs)
        : base(string.Format(errorMessageFormat, formatArgs))
    {
        ErrorCode = errorCode;
        Arguments = arguments;
    }

    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    /// <value>
    /// The error code.
    /// </value>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Gets or sets the arguments.
    /// </summary>
    /// <value>
    /// The arguments.
    /// </value>
    public string? Arguments { get; set; }

    /// <summary>
    /// By default all ValidationErrors are end user exceptions (e.g. message can be shown safely to the end user)
    /// </summary>
    public bool IsSensitiveMessage { get; set; }
}