// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
	/// <summary>
	/// This interface is used internally to identify registrations in the type registry.
	/// </summary>
	internal interface IRegistrationKey
	{
		/// <summary>
		/// Gets the type that this key identifies.
		/// </summary>
		/// <returns>Returns the type of the registration.</returns>
	    Type GetInstanceType();
		bool Equals(object obj);
		int  GetHashCode();
	}
}
