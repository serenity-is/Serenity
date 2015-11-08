using System;
using System.Reflection.Emit;

namespace Serenity.Reflection
{
    public static class FastReflection
    {
        public static Func<object> DelegateForConstructor(Type type)
        {
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

            return (Func<object>)dm.CreateDelegate(typeof(Func<object>));
        }
    }
}
