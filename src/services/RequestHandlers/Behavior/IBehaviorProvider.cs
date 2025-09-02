using System.Collections;

namespace Serenity.Services;

/// <summary>
/// Interface abstraction for behavior provider which resolves
/// list of behavior instances of a specified type, like <see cref="ISaveBehavior"/> etc.
/// </summary>
public interface IBehaviorProvider
{
    /// <summary>
    /// Resolves a list of behavior instances targeted for a handler, row and behaivor type.
    /// </summary>
    /// <param name="handlerType">The handler type requesting list of behaviors</param>
    /// <param name="rowType">Target row type</param>
    /// <param name="behaviorType">Type of the behaviors</param>
    IEnumerable Resolve(Type handlerType, Type rowType, Type behaviorType);
}