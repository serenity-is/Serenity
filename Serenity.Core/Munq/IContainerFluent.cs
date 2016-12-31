#if !COREFX
// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

namespace Munq
{
    public interface IContainerFluent
    {
        IContainerFluent UsesDefaultLifetimeManagerOf(ILifetimeManager lifetimeManager);
    }
}
#endif