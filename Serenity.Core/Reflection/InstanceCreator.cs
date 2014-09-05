using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Serenity.Reflection
{
    public static class InstanceCreator
    {
        /// <summary>
        /// Returns an instance of the <paramref name="type"/> on which the method is invoked.
        /// </summary>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <returns>An instance of the <paramref name="type"/>.</returns>
        public static object GetInstance(this Type type)
        {
            return GetInstance<TypeToIgnore>(type, null);
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg">The type of the argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument">The argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type"/>.</returns>
        public static object GetInstance<TArg>(this Type type, TArg argument)
        {
            return GetInstance<TArg, TypeToIgnore>(type, argument, null);
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument1">The first argument to pass to the constructor.</param>
        /// <param name="argument2">The second argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type"/>.</returns>
        public static object GetInstance<TArg1, TArg2>(this Type type, TArg1 argument1, TArg2 argument2)
        {
            return GetInstance<TArg1, TArg2, TypeToIgnore>(type, argument1, argument2, null);
        }

        /// <summary>
        /// Returns an instance of the <paramref name="type"/> on which the method is invoked.
        /// </summary>
        /// <typeparam name="TArg1">The type of the first argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg2">The type of the second argument to pass to the constructor.</typeparam>
        /// <typeparam name="TArg3">The type of the third argument to pass to the constructor.</typeparam>
        /// <param name="type">The type on which the method was invoked.</param>
        /// <param name="argument1">The first argument to pass to the constructor.</param>
        /// <param name="argument2">The second argument to pass to the constructor.</param>
        /// <param name="argument3">The third argument to pass to the constructor.</param>
        /// <returns>An instance of the given <paramref name="type"/>.</returns>
        public static object GetInstance<TArg1, TArg2, TArg3>(
            this Type type,
            TArg1 argument1,
            TArg2 argument2,
            TArg3 argument3)
        {
            return InstanceCreationFactory<TArg1, TArg2, TArg3>
                .CreateInstanceOf(type, argument1, argument2, argument3);
        }

        // To allow for overloads with differing numbers of arguments, we flag arguments which should be 
        // ignored by using this Type:
        private class TypeToIgnore
        {
        }

        private static class InstanceCreationFactory<TArg1, TArg2, TArg3>
        {
            // This dictionary will hold a cache of object-creation functions, keyed by the Type to create:
            private static readonly Dictionary<Type, Func<TArg1, TArg2, TArg3, object>> _instanceCreationMethods =
                new Dictionary<Type, Func<TArg1, TArg2, TArg3, object>>();

            public static object CreateInstanceOf(Type type, TArg1 arg1, TArg2 arg2, TArg3 arg3)
            {
                CacheInstanceCreationMethodIfRequired(type);

                return _instanceCreationMethods[type].Invoke(arg1, arg2, arg3);
            }

            private static void CacheInstanceCreationMethodIfRequired(Type type)
            {
                // Bail out if we've already cached the instance creation method:
                if (_instanceCreationMethods.ContainsKey(type))
                {
                    return;
                }

                var argumentTypes = new[] { typeof(TArg1), typeof(TArg2), typeof(TArg3) };

                // Get a collection of the constructor argument Types we've been given; ignore any 
                // arguments which are of the 'ignore this' Type:
                Type[] constructorArgumentTypes = argumentTypes.Where(t => t != typeof(TypeToIgnore)).ToArray();

                // Get the Constructor which matches the given argument Types:
                var constructor = type.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    CallingConventions.HasThis,
                    constructorArgumentTypes,
                    new ParameterModifier[0]);

                // Get a set of Expressions representing the parameters which will be passed to the Func:
                var lamdaParameterExpressions = new[]
            {
                Expression.Parameter(typeof(TArg1), "param1"),
                Expression.Parameter(typeof(TArg2), "param2"),
                Expression.Parameter(typeof(TArg3), "param3")
            };

                // Get a set of Expressions representing the parameters which will be passed to the constructor:
                var constructorParameterExpressions = lamdaParameterExpressions
                    .Take(constructorArgumentTypes.Length)
                    .ToArray();

                // Get an Expression representing the constructor call, passing in the constructor parameters:
                var constructorCallExpression = Expression.New(constructor, constructorParameterExpressions);

                // Compile the Expression into a Func which takes three arguments and returns the constructed object:
                var constructorCallingLambda = Expression
                    .Lambda<Func<TArg1, TArg2, TArg3, object>>(constructorCallExpression, lamdaParameterExpressions)
                    .Compile();

                _instanceCreationMethods[type] = constructorCallingLambda;
            }
        }
    }
}