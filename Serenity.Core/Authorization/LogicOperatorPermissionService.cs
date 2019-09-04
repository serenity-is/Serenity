using Serenity.Abstractions;
using Serenity.Services;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Serenity.Web
{
    /// <summary>
    /// Adds AND OR operator support to any IPermissionService implementation
    /// </summary>
    /// <remarks>
    /// Register this class in your application start, to allow !, |, &amp;, () operators
    /// in your permission services, e.g.
    /// <code>
    /// registrar.RegisterInstance&lt;IPermissionService&gt;(new LogicOperatorPermissionService(new MyPermissionService()))
    /// </code>
    /// </remarks>
    public partial class LogicOperatorPermissionService : IPermissionService
    {
        private static readonly char[] chars = new Char[] { '|', '&', '!', '(', ')' };
        private IPermissionService permissionService;
        private ConcurrentDictionary<string, string[]> cache = new ConcurrentDictionary<string, string[]>();

        /// <summary>
        /// Creates a new LogicOperatorPermissionService wrapping passed IPermissionService
        /// </summary>
        /// <param name="permissionService">Permission service to wrap with AND/OR functionality</param>
        public LogicOperatorPermissionService(IPermissionService permissionService)
        {
            Check.NotNull(permissionService, "permissionService");

            this.permissionService = permissionService;
        }

        /// <summary>
        /// Returns true if user has specified permission
        /// </summary>
        /// <param name="permission">Permission to check</param>
        /// <returns>True if user has specified permission</returns>
        public bool HasPermission(string permission)
        {
            if (string.IsNullOrEmpty(permission) ||
                permission.IndexOfAny(chars) < 0)
                return permissionService.HasPermission(permission);

            string[] rpnTokens;
            if (!cache.TryGetValue(permission, out rpnTokens))
            {
                var tokens = PermissionExpressionParser.Tokenize(permission);
                cache[permission] = rpnTokens = PermissionExpressionParser.ShuntingYard(tokens).ToArray();
            }

            return PermissionExpressionParser.Evaluate(rpnTokens, permissionService.HasPermission);
        }
    }
}