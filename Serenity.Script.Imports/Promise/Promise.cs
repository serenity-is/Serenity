using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace System
{
    [Imported, IgnoreNamespace, ScriptName("Promise")]
    public class Promise : IPromise
    {
        [InlineCode("new Promise({constructor})")]
        public Promise(Action<Delegate, Delegate> constructor)
        {
        }

        void IPromise.Then(Delegate onFulfilled)
        {
        }

        void IPromise.Then(Delegate onFulfilled, Delegate onRejected)
        {
        }

        void IPromise.Then(Delegate onFulfilled, Delegate onRejected, Delegate progressHandler)
        {
        }

        [PreserveName, ScriptName("then")]
        public Promise Then(Action onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise<TValue> ThenSelect<TValue>(Func<TValue> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise<TValue> ThenAwait<TValue>(Func<Promise<TValue>> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise ThenAwait(Func<Promise> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [InlineCode("{this}.catch({onRejected})")]
        public Promise Catch(Callback onRejected)
        {
            return null;
        }

        public static Promise All(params IPromise[] promises)
        {
            return null;
        }

        public static Promise All(ICollection<IPromise> promises)
        {
            return null;
        }

        public static Promise Race(params IPromise[] promises)
        {
            return null;
        }

        public static Promise Race(ICollection<IPromise> promises)
        {
            return null;
        }

        public static Promise Resolve()
        {
            return null;
        }

        [ScriptName("Promise.resolve({value})"), IncludeGenericArguments(false)]
        public static Promise<TValue> FromValue<TValue>(TValue value)
        {
            return null;
        }

        public static Promise Void
        {
            [InlineCode("Promise.resolve()")]
            get { return null; }
        }
    }


    [Imported, IgnoreNamespace, ScriptName("Promise")]
    public class Promise<TValue> : Promise
    {
        [InlineCode("new Promise({constructor})")]
        public Promise(Action<Action<TValue>, Callback> constructor)
            : base((Action<Delegate, Delegate>)null)
        {
        }

        [PreserveName, IncludeGenericArguments(false)]
        public Promise Then(Action<TValue> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise<TOutput> ThenSelect<TOutput>(Func<TValue, TOutput> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise<TOutput> ThenAwait<TOutput>(Func<TValue, Promise<TOutput>> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [IncludeGenericArguments(false), ScriptName("then")]
        public Promise ThenAwait(Func<TValue, Promise> onFulfilled, Callback onRejected = null)
        {
            return null;
        }

        [InlineCode("{this}['catch']({onRejected})")]
        public new Promise<TValue> Catch(Callback onRejected)
        {
            return null;
        }
    }
}