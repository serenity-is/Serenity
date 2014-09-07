using Serenity.Abstractions;
using Serenity.ComponentModel;
using Serenity.Services;
using System;

namespace Serenity.Configuration
{
    [SettingScope("Application"), SettingKey("AdminUserSettings")]
    public class AdminUserSettings
    {
        public long? UserId { get; set; }
        public string Username { get; set; }
        public string MasterPassword { get; set; }
    }
}