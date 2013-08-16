// --------------------------------------------------------------------------------------------------
// © Copyright 2011 by Matthew Dennis.
// Released under the Microsoft Public License (Ms-PL) http://www.opensource.org/licenses/ms-pl.html
// --------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

namespace Munq
{
	public partial class IocContainer : IDependencyResolver
	{
		#region Resolve Members
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve1"]/*' />
		public TType Resolve<TType>() where TType : class
		{
			return Resolve(null, typeof(TType)) as TType;
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve2"]/*' />
		public TType Resolve<TType>(string name) where TType : class
		{
			return Resolve(name, typeof(TType)) as TType;
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve3"]/*' />
		public object Resolve(Type type)
		{
			return Resolve(null, type);
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="Resolve4"]/*' />
		public object Resolve(string name, Type type)
		{
			try
			{
				return typeRegistry.Get(name, type).GetInstance();
			}
			catch (KeyNotFoundException knfe)
			{
				return HandleUnResolved(knfe, name, type);
			}

		}

		private object HandleUnResolved(Exception knfe, string name, Type type)
		{
			if (type.IsGenericType)
			{
				object result = ResolveUsingOpenType(knfe, name, type);
				if (result!=null)
					return result;
			}

			if (type.IsClass)
			{
				try
				{
					var func = CreateInstanceDelegateFactory.Create(type);
					Register(name, type, func);
					// Thanks to dfullerton for catching this.
					return typeRegistry.Get(name, type).GetInstance();
				}
				catch
				{
					throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
				}
			}

			if (type.IsInterface)
			{
				var regs = typeRegistry.GetDerived(name, type);
				var reg = regs.FirstOrDefault();
				if (reg != null)
				{
					object instance = reg.GetInstance();
					Register(name, type, (c) => c.Resolve(name, instance.GetType()));
					return instance;
				}
				else
					throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
			}
			throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
		}

		private object ResolveUsingOpenType(Exception knfe, string name, Type type)
		{
			if (type.ContainsGenericParameters)
				throw new KeyNotFoundException(ResolveFailureMessage(type), knfe);
			else
			{
				// Look for an Open Type Definition registration
				// create a type using the registered Open Type
				// Try and resolve this type
				var definition = type.GetGenericTypeDefinition();
				var arguments  = type.GetGenericArguments();
				if (opentypeRegistry.ContainsKey(name, definition))
				{
					var reg      = opentypeRegistry.Get(name, definition);
					var implType = reg.ImplType;
					var newType  = implType.MakeGenericType(arguments);
					try
					{
						if (CanResolve(name, newType))
							return Resolve(name, newType);

						Register(name, type, newType).WithLifetimeManager(reg.LifetimeManager);
						return typeRegistry.Get(name, type).GetInstance();
					}
					catch
					{
						return null;
					}
				}
			}
			return null;
		}
		#endregion

		#region CanResolve Members

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve1"]/*' />
		public bool CanResolve<TType>()
				where TType : class
		{
			return CanResolve(null, typeof(TType));
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve2"]/*' />
		public bool CanResolve<TType>(string name)
				where TType : class
		{
			return CanResolve(name, typeof(TType));
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve3"]/*' />
		public bool CanResolve(Type type)
		{
			return CanResolve(null, type);
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="CanResolve4"]/*' />
		public bool CanResolve(string name, Type type)
		{
			return typeRegistry.ContainsKey(name, type);
		}
		#endregion

		#region Resolve All Methods
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="ResolveAll1"]/*' />
		public IEnumerable<TType> ResolveAll<TType>() where TType : class
		{
			return ResolveAll(typeof(TType)).Cast<TType>();
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="ResolveAll2"]/*' />
		public IEnumerable<object> ResolveAll(Type type)
		{
			var registrations = typeRegistry.GetDerived(type);
			var instances = new List<object>();
			foreach (var reg in registrations)
			{
				instances.Add(reg.GetInstance());
			}
			return instances;
		}
		#endregion

		#region LazyResolve Members
		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve1"]/*' />
		public Func<TType> LazyResolve<TType>() where TType : class
		{
			return LazyResolve<TType>(null);
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve2"]/*' />
		public Func<TType> LazyResolve<TType>(string name) where TType : class
		{
			return () => Resolve<TType>(name);
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve3"]/*' />
		public Func<Object> LazyResolve(Type type)
		{
			return LazyResolve(null, type);
		}

		/// <include file='XmlDocumentation/IDependencyResolver.xml' path='IDependencyResolver/Members[@name="LazyResolve4"]/*' />
		/// <inheritdoc />
		public Func<Object> LazyResolve(string name, Type type)
		{
			return () => Resolve(name, type);
		}

		private static string ResolveFailureMessage(Type type)
		{
			return String.Format("Munq IocContainer failed to resolve {0}", type);
		}
		#endregion
	}
}
