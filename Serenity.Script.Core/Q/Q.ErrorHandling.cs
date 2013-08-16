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
    }
}