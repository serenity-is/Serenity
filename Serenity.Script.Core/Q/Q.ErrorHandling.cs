using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    public static partial class Q
    {
        public static class ErrorHandling
        {
            public static void ShowServiceError(ServiceError error)
            {
                if (error == null)
                    throw new Exception("error is null!");

                Q.Alert(error == null ? "??ERROR??" : (error.Message ?? error.Code));
            }
        }

        public static Action TryCatch(this Action<object> fail, Action callback)
        {
            if (fail == null)
            {
                return callback;
            }

            return delegate()
            {
                try
                {
                    callback();
                }
                catch (Exception ex)
                {
                    fail(ex);
                }
            };
        }
    }
}