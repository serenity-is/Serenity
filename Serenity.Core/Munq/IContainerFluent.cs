#if NET45
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

namespace Munq
{
    /// <summary>
    /// Fluent container
    /// </summary>
    public interface IContainerFluent
    {
        /// <summary>
        /// Gets uses the default lifetime manager of.
        /// </summary>
        /// <param name="lifetimeManager">The lifetime manager.</param>
        /// <returns></returns>
        IContainerFluent UsesDefaultLifetimeManagerOf(ILifetimeManager lifetimeManager);
    }
}
#endif