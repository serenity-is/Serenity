using Serenity.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Serenity.Web
{
    /// <summary>
    /// Adds impersonation support to any IAuthorizationService implementation
    /// </summary>
    public class ImpersonatingAuthorizationService : IAuthorizationService, IImpersonator
    {
        private IAuthorizationService authorizationService;
        private IRequestContext requestContext;
        private ThreadLocal<Stack<string>> impersonationStack = new ThreadLocal<Stack<string>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ImpersonatingAuthorizationService"/> class
        /// that wraps passed authorization service and adds impersonation support.
        /// </summary>
        /// <param name="authorizationService">The authorization service to wrap with impersonation support.</param>
        /// <param name="requestContext">Request context</param>
        public ImpersonatingAuthorizationService(IAuthorizationService authorizationService, IRequestContext requestContext = null)
        {
            this.authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
#if NET45
            this.requestContext = requestContext;
#else
            this.requestContext = requestContext ?? throw new ArgumentNullException(nameof(requestContext));
#endif
        }

        private Stack<string> GetImpersonationStack(bool createIfNull)
        {
            Stack<string> stack;

#if NET45
            var requestItems = (requestContext ?? Dependency.Resolve<IRequestContext>()).Items;
#else
            var requestItems = requestContext.Items;
#endif

            if (requestItems != null)
            {
                stack = requestItems["ImpersonationStack"] as Stack<string>;
                if (stack == null && createIfNull)
                    requestItems["ImpersonationStack"] = stack = new Stack<string>();
            }
            else
            {
                stack = impersonationStack.Value;
                if (stack == null && createIfNull)
                    impersonationStack.Value = stack = new Stack<string>();
            }

            return stack;
        }

        /// <summary>
        /// True if there is a currenty logged user
        /// </summary>
        public bool IsLoggedIn 
        {
            get
            {
                return !string.IsNullOrEmpty(Username);
            }
        }

        /// <summary>
        /// Return currently logged user name
        /// </summary>
        public string Username
        { 
            get
            {
                var impersonationStack = GetImpersonationStack(false);

                if (impersonationStack != null && impersonationStack.Count > 0)
                    return impersonationStack.Peek();

                return authorizationService.Username;
            }
        }

        /// <summary>
        /// Temporarily impersonates as a user
        /// </summary>
        /// <param name="username">Username to impersonate as</param>
        public void Impersonate(string username)
        {
            Check.NotNullOrEmpty(username, "username");
            var impersonationStack = GetImpersonationStack(true);
            impersonationStack.Push(username);
        }

        /// <summary>
        /// Undoes impersonation
        /// </summary>
        /// <exception cref="InvalidOperationException">UndoImpersonate() is called while impersonation stack is empty!</exception>
        public void UndoImpersonate()
        {
            var impersonationStack = GetImpersonationStack(false);
            if (impersonationStack == null || impersonationStack.Count == 0)
                throw new InvalidOperationException("UndoImpersonate() is called while impersonation stack is empty!");

            impersonationStack.Pop();
        }
    }
}