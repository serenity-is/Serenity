using System;
using System.Collections.Generic;
using System.Linq;

namespace Serenity
{
    public static class MethodChainingExtensions
    {
        /// <summary>
        /// Allows to reference the call chain object itself without breaking a call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as parameter.</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain>(this TChain chain, Action<TChain> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// an inline object without breaking a call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1>(this TChain chain, TParam1 param1, Action<TChain, TParam1> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// two inline objects without breaking call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2>(this TChain chain, 
            TParam1 param1, TParam2 param2, Action<TChain, TParam1, TParam2> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// three inline objects without breaking call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, Action<TChain, TParam1, TParam2, TParam3> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// four inline objects without breaking call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, 
            Action<TChain, TParam1, TParam2, TParam3, TParam4> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// five inline objects without breaking call chain.
        /// </summary>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <param name="param5">An object that will be passed to action as sixth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4, TParam5>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5,
            Action<TChain, TParam1, TParam2, TParam3, TParam4, TParam5> action)
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5);

            return chain;
        }
    }
}