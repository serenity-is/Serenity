// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
	/// <summary>
	/// This is the result of registering a type in the container.
	/// </summary>
	public interface IRegistration
	{
		/// <summary>
		/// Gets the name of the registration.
		/// </summary>
		string Name         { get; }

		/// <summary>
		/// Gets the key that is used to identify cached values.
		/// </summary>
		string Key          { get; }

		/// <summary>
		/// Gets the type the contain will Resolve to when this Registration is used.
		/// </summary>
		Type   ResolvesTo   { get; }

		/// <summary>
		/// Sets the lifetime manager to be used by this Registration.
		/// </summary>
		/// <param name="manager">The ILifetimeManager to use.</param>
		/// <returns>'this', or the Registration.</returns>
		IRegistration WithLifetimeManager(ILifetimeManager manager);

		/// <summary>
		/// Invalidates any cached value so that a new instance will be created on
		/// the next Resolve call.
		/// </summary>
		void          InvalidateInstanceCache();

		/// <summary>
		/// Creates an instance of the type using the registered function.
		/// </summary>
		/// <returns>The new instance.</returns>
		object        CreateInstance();
	}
}
