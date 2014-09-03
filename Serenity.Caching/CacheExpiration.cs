
namespace Serenity
{
    using System;

    /// <summary>
    /// Bu sınıf içerisinde bazı standart cache'leme süreleri bulunur. Bu süreler TwoLevelCache gibi diğer
    /// nesnelere geçirilen parametreler için kullanılabilir. Normalde bu tip fonksiyonlar çağrılırken
    /// expire süresi TimeSpan.FromMinutes gibi de manuel tanımlanıp kullanılabilir. Ancak bu sınıftaki 
    /// öntanımlı değerler kullanılırsa hem her cache'lemede farklı değerler kullanılmamış olur 
    /// hem de find reference ile cache sürelerinin kullanıldığı noktalar tespit edilebilir. 
    /// Aksi taktirde tüm TimeSpan referanslarının incelenmesi gerekebilir.
    /// </summary>
    public static class CacheExpiration
    {
        public static readonly TimeSpan FifteenSeconds = TimeSpan.FromSeconds(15);
        public static readonly TimeSpan OneMinute = TimeSpan.FromMinutes(1);
        public static readonly TimeSpan FiveMinutes = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan ThirtyMinutes = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan OneHour = TimeSpan.FromMinutes(60);
        public static readonly TimeSpan OneDay = TimeSpan.FromDays(1);

        public static readonly TimeSpan Never = TimeSpan.Zero;
    }
}
