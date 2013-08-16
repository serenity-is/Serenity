// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

namespace Munq
{
	/// <summary>
	/// Defines the functionality for Lifetime Managers.  Implementation should instantiate an 
	/// instance store and use the Registration's Key property to index the data in the store.
	/// This allows one lifetime manager to service multiple Registrations.
	/// </summary>
	public interface ILifetimeManager
	{
		/// <summary>
		/// Get an instance for the registration, using the lifetime manager to cache instance
		/// as required by the scope of the lifetime manager.
		/// </summary>
		/// <param name="registration">
		/// The registration which is used to supply the storage key and create a new instance if
		/// required.
		/// </param>
		/// <returns>The cached or new instance.</returns>
		object GetInstance(IRegistration registration);

		/// <summary>
		/// Invalidate the instance in whatever storage is used by the lifetime manager.
		/// </summary>
		/// <param name="registration">
		/// The registration which is used to supply the storage key and create a new instance if
		/// required.
		/// </param>
		void InvalidateInstanceCache(IRegistration registration);
	}
}
