// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Munq
{
	/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/SummaryDocumentation/*' />
	public interface IDependecyRegistrar
	{
		// ---- Lifetime Manager -----------------------------------------------------------------

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="DefaultLifetimeManager"]/*' />
		ILifetimeManager DefaultLifetimeManager { get; set; }

		// ---- Register -------------------------------------------------------------------------

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="Register1"]/*' />
		IRegistration Register<TType>(Func<IDependencyResolver, TType> func) where TType : class;
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="Register2"]/*' />
		IRegistration Register<TType>(string name, Func<IDependencyResolver, TType> func) where TType : class;

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="Register3"]/*' />
		IRegistration Register(Type type, Func<IDependencyResolver, object> func);
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="Register4"]/*' />
		IRegistration Register(string name, Type type, Func<IDependencyResolver, object> func);

		// ---- Register Instance ----------------------------------------------------------------
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterInstance1"]/*' />
		IRegistration RegisterInstance<TType>(TType instance) where TType : class;
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterInstance2"]/*' />
		IRegistration RegisterInstance<TType>(string name, TType instance) where TType : class;

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterInstance3"]/*' />
		IRegistration RegisterInstance(Type type, object instance);
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterInstance4"]/*' />
		IRegistration RegisterInstance(string name, Type type, object instance);

		// ---- Register Type Implementation -----------------------------------------------------
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterType1"]/*' />
		IRegistration Register<TType, TImpl>() where TType: class where TImpl : class, TType;
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterType2"]/*' />
		IRegistration Register<TType, TImpl>(string name) where TType: class where TImpl : class, TType;

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterType3"]/*' />
		IRegistration Register(Type tType, Type tImpl);
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="RegisterType4"]/*' />
		IRegistration Register(string name, Type tType, Type tImpl);

		// ---- Remove Registration --------------------------------------------------------------
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="Remove"]/*' />
		void Remove(IRegistration ireg);

		// ---- Get Registration -----------------------------------------------------------
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistration1"]/*' />
		IRegistration GetRegistration<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistration2"]/*' />
		IRegistration GetRegistration<TType>(string name) where TType : class;

		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistration3"]/*' />
		IRegistration GetRegistration(Type type);
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistration4"]/*' />
		IRegistration GetRegistration(string name, Type type);

		// ---- Get Registrations ----------------------------------------------------------
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistrations1"]/*' />
		IEnumerable<IRegistration> GetRegistrations<TType>() where TType : class;
		/// <include file='XmlDocumentation/IDependencyRegistrar.xml' path='IDependencyRegistrar/Members[@name="GetRegistrations2"]/*' />
		IEnumerable<IRegistration> GetRegistrations(Type type);
	}
}