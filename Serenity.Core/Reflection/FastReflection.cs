using System;
#if COREFX
using System.Reflection;
#else
using System.Reflection.Emit;
#endif

namespace Serenity.Reflection
{
    public static class FastReflection
    {
        public static Func<object> DelegateForConstructor(Type type)
        {
            return DelegateForConstructor<object>(type);
        }

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

            var dm = new DynamicMethod("ctor0", type, Type.EmptyTypes);
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
