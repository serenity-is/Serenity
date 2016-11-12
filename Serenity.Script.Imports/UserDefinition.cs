using System.Html;
using jQueryApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Serenity
{
    /// <summary>
    /// User definition interface
    /// </summary>
    [Imported, Serializable, ScriptName("UserDefinition"), PreserveMemberCase]
    public class UserDefinition
    {
        [IntrinsicProperty]
        public string Username { get { return null; } }
        [IntrinsicProperty]
        public string DisplayName { get { return null; } }
        [IntrinsicProperty]
        public bool IsAdmin { get { return false; } }
        [IntrinsicProperty]
        public JsDictionary<string, bool> Permissions { get { return null; } }
    }
}