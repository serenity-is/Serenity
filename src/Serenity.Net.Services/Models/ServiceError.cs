
namespace Serenity.Services;

/// <summary>
/// The error object model returned from a service
/// </summary>
public class ServiceError
{
    /// <summary>
    /// Error code if any
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// Custom arguments info for the error. In some cases, 
    /// this might be the field name the error is related to.
    /// </summary>
    public string Arguments { get; set; }

    /// <summary>
    /// The error message. In non-development mode the message 
    /// might be something generic like "some error occured" if the
    /// error itself is not a <see cref="ValidationError"/>.
    /// The detailed error can be seen in exception log.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Error details, like stack trace etc. Normally, 
    /// this is only returned in development mode.
    /// </summary>
    public string Details { get; set; }

    /// <summary>
    /// When provided, this might be the related error ID 
    /// stored in the exception log.
    /// </summary>
    public string ErrorId { get; set; }
}
