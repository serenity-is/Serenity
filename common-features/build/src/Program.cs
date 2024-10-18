namespace Build
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Shared.DetermineRoot();
            
            var target = Shared.GetTarget(new(args));
            if (target == "pack")
                Shared.Targets.Pack();
            else if (target == "push")
                Targets.Push();
        }
    }
}