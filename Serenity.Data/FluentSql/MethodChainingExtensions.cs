using System;

namespace Serenity
{
    /// <summary>
    /// Contains method chaining extensions
    /// </summary>
    public static class MethodChainingExtensions
    {
        /// <summary>
        /// Allows to reference the call chain object itself without breaking a call chain.
        /// </summary>
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as parameter.</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain>(this TChain chain, Action<TChain> action)
            where TChain: IChainable
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
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1>(this TChain chain, TParam1 param1, Action<TChain, TParam1> action)
            where TChain : IChainable
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
        /// <param name="chain">Chaining object</param>
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
        /// <param name="chain">Chaining object</param> 
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, Action<TChain, TParam1, TParam2, TParam3> action)
            where TChain : IChainable
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
        /// <param name="chain">Chaining object</param>
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
            where TChain : IChainable
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
        /// <param name="chain">Chaining object</param>
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
            where TChain : IChainable
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// six inline objects without breaking call chain.
        /// </summary>
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <param name="param5">An object that will be passed to action as sixth parameter</param>
        /// <param name="param6">An object that will be passed to action as seventh parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6,
            Action<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6> action)
            where TChain : IChainable
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5, param6);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// seven inline objects without breaking call chain.
        /// </summary>
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <param name="param5">An object that will be passed to action as sixth parameter</param>
        /// <param name="param6">An object that will be passed to action as seventh parameter</param>
        /// <param name="param7">An object that will be passed to action as eight parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7,
            Action<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7> action)
            where TChain : IChainable
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5, param6, param7);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// seven inline objects without breaking call chain.
        /// </summary>
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <param name="param5">An object that will be passed to action as sixth parameter</param>
        /// <param name="param6">An object that will be passed to action as seventh parameter</param>
        /// <param name="param7">An object that will be passed to action as eight parameter</param>
        /// <param name="param8">An object that will be passed to action as ninth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8,
            Action<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8> action)
            where TChain : IChainable
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5, param6, param7, param8);

            return chain;
        }

        /// <summary>
        /// Allows to reference the call chain object itself, while declaring
        /// seven inline objects without breaking call chain.
        /// </summary>
        /// <param name="chain">Chaining object</param>
        /// <param name="action">An action that will be called with the chain object as first parameter.</param>
        /// <param name="param1">An object that will be passed to action as second parameter</param>
        /// <param name="param2">An object that will be passed to action as third parameter</param>
        /// <param name="param3">An object that will be passed to action as fourth parameter</param>
        /// <param name="param4">An object that will be passed to action as fifth parameter</param>
        /// <param name="param5">An object that will be passed to action as sixth parameter</param>
        /// <param name="param6">An object that will be passed to action as seventh parameter</param>
        /// <param name="param7">An object that will be passed to action as eight parameter</param>
        /// <param name="param8">An object that will be passed to action as ninth parameter</param>
        /// <param name="param9">An object that will be passed to action as tenth parameter</param>
        /// <returns>
        /// The query itself.</returns>
        public static TChain With<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9>(this TChain chain,
            TParam1 param1, TParam2 param2, TParam3 param3, TParam4 param4, TParam5 param5, TParam6 param6, TParam7 param7, TParam8 param8, TParam9 param9,
            Action<TChain, TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TParam8, TParam9> action)
            where TChain : IChainable
        {
            if (action == null)
                throw new ArgumentNullException("action");

            action(chain, param1, param2, param3, param4, param5, param6, param7, param8, param9);

            return chain;
        }

    }
}