// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
	/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/SummaryDocumentation/*' />
	public interface IDependencyResolver
	{
		//Resolve
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve1"]/*' />
		TType Resolve<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve2"]/*' />
		TType Resolve<TType>(string name) where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve3"]/*' />
		object Resolve(Type type);
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve4"]/*' />
		object Resolve(string name, Type type);

		//ResolveAll
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="ResolveAll1"]/*' />
		IEnumerable<TType> ResolveAll<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="ResolveAll2"]/*' />
		IEnumerable<object> ResolveAll(Type type);

		//Lazy Resolve
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve1"]/*' />
		Func<TType> LazyResolve<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve2"]/*' />
		Func<TType> LazyResolve<TType>(string name) where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve3"]/*' />
		Func<object> LazyResolve(Type type);
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve4"]/*' />
		Func<object> LazyResolve(string name, Type type);

		//CanResolve
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve1"]/*' />
		bool CanResolve<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve2"]/*' />
		bool CanResolve<TType>(string name) where TType : class;
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve3"]/*' />
		bool CanResolve(Type type);
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve4"]/*' />
		bool CanResolve(string name, Type type);
	}
}
