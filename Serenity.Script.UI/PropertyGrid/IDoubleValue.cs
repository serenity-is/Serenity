using System;
using System.Runtime.CompilerServices;

namespace Serenity
{
    [Imported(ObeysTypeSystem = true)]
    public interface IDoubleValue
    {
        Double? Value { get; set; }
    }
}