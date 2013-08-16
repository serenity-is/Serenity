// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;

namespace Munq
{
	/// <include file='XmlDocumentation/IContainerFluent.xml' path='IContainerFluent/SummaryDocumentation/*' />
	public interface IContainerFluent
	{
		/// <include file='XmlDocumentation/IContainerFluent.xml'
		/// path='IContainerFluent/Members[@name="UsesDefaultLifetimeManagerOf"]/*' />
		IContainerFluent UsesDefaultLifetimeManagerOf(ILifetimeManager lifetimeManager);
	}
}
