using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Serenity.Services
{
    /// <summary>
    /// Extension methods for behavior provider
    /// </summary>
    public static class BehaviorProviderExtensions
    {
        /// <summary>
        /// Resolves behaviors for item type
        /// </summary>
        /// <typeparam name="TBehavior">Behavior type</typeparam>
        /// <param name="provider">Provider</param>
        /// <param name="forType">Item type</param>
        /// <returns>Behavior</returns>
        public static IEnumerable<TBehavior> Resolve<TBehavior>(this IBehaviorProvider provider, Type forType)
        {
            return provider.Resolve(forType, typeof(TBehavior), Type.EmptyTypes).Cast<TBehavior>();
        }

        /// <summary>
        /// Resolves behaviors for item type
        /// </summary>
        /// <typeparam name="TBehavior">Behavior type</typeparam>
        /// <param name="provider">Provider</param>
        /// <param name="forType">Item type</param>
        /// <param name="explicitTypes">Explict types</param>
        /// <returns>Behavior</returns>
        public static IEnumerable<TBehavior> Resolve<TBehavior>(this IBehaviorProvider provider, Type forType, Type[] explicitTypes)
        {
            return provider.Resolve(forType, typeof(TBehavior), explicitTypes).Cast<TBehavior>();
        }

        /// <summary>
        /// Resolves behaviors for item type
        /// </summary>
        /// <typeparam name="TBehavior">Behavior type</typeparam>
        /// <param name="provider">Provider</param>
        /// <param name="itemType">Item type</param>
        /// <returns>Behavior</returns>
        public static IEnumerable<TBehavior> Resolve<TForType, TBehavior>(this IBehaviorProvider provider)
        {
            return provider.Resolve(typeof(TForType), typeof(TBehavior), Type.EmptyTypes).Cast<TBehavior>();
        }


        /// <summary>
        /// Resolves behaviors for item type
        /// </summary>
        /// <typeparam name="TBehavior">Behavior type</typeparam>
        /// <param name="provider">Provider</param>
        /// <param name="itemType">Item type</param>
        /// <param name="explicitTypes">Explict types</param>
        /// <returns>Behavior</returns>
        public static IEnumerable<TBehavior> Resolve<TForType, TBehavior>(this IBehaviorProvider provider, Type[] explicitTypes)
        {
            return provider.Resolve(typeof(TForType), typeof(TBehavior), explicitTypes).Cast<TBehavior>();
        }
    }
}