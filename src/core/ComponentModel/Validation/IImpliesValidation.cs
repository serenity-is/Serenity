using System.ComponentModel.DataAnnotations;

namespace Serenity.ComponentModel;

/// <summary>
/// An attribute, e.g. of an editor, that implies validation rules for the property it is on, like an email editor that
/// implies <see cref="ValidateEmailAttribute"/>. They are checked on the server and by the forms as if they were on the
/// property, see <see cref="ValidationRules.GetRules(IEnumerable{object})"/>.
/// </summary>
public interface IImpliesValidation
{
    /// <summary>
    /// Gets the rules the attribute implies.
    /// </summary>
    IEnumerable<ValidationAttribute> GetImpliedValidation();
}
