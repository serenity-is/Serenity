using System.Threading;

namespace Serenity.Data
{
    public struct Parameter
    {
        private static long next;
        private string name;

        public Parameter(string name)
        {
            this.name = name;
        }

        public static long NextNumber()
        {
            var mine = Interlocked.Increment(ref next);
            if (mine > 9000000000000000000)
                Interlocked.CompareExchange(ref next, 1, mine);
            return mine;
        }

        public static string NextName()
        {
            var mine = Interlocked.Increment(ref next);
            if (mine > 9000000000000000000)
                Interlocked.CompareExchange(ref next, 1, mine);
            return "@p" + mine.ToInvariant();
        }

        public static Parameter Next()
        {
            return new Parameter(NextName());
        }

        public string Name
        {
            get { return name; }
        }
    }
}
