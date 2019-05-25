using System;
#if COREFX
using System.Reflection;
#else
using System.Reflection.Emit;
#endif

namespace Serenity.Reflection
{
    /// <summary>
    /// Fast reflection utils (not very fast in .NET4+)
    /// </summary>
    public static class FastReflection
    {
        /// <summary>
        /// Creates a delegate for constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>Delegate for constructor</returns>
        public static Func<object> DelegateForConstructor(Type type)
        {
            return DelegateForConstructor<object>(type);
        }

        /// <summary>
        /// Creates a delegate for constructor.
        /// </summary>
        /// <typeparam name="TReturn">The type of the return.</typeparam>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException">No constructor</exception>
        public static Func<TReturn> DelegateForConstructor<TReturn>(Type type)
        {
#if COREFX
            var constructor = type.GetTypeInfo().GetConstructor(Type.EmptyTypes);
            if (constructor == null)
                throw new ArgumentOutOfRangeException("type");

            return () => (TReturn)Activator.CreateInstance(type);
#else

            var ctor = type.GetConstructor(Type.EmptyTypes);

            if (ctor == null)
                throw new MissingMethodException(String.Format(
                    "There is no parameterless constructor for type {0}", type));

            var dm = new DynamicMethod("ctor0", type, Type.EmptyTypes, true);
            ILGenerator il = dm.GetILGenerator();
            il.DeclareLocal(type);
            il.Emit(OpCodes.Newobj, ctor);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            return (Func<TReturn>)dm.CreateDelegate(typeof(Func<TReturn>));
#endif
        }
    }
}
