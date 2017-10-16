#if !ASPNETCORE
using System;

namespace Serenity.Services
{
    public class IgnoreAntiforgeryTokenAttribute : Attribute
    {
    }
}
#endif