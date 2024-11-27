namespace Build
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Shared.DetermineRoot();
            
            var target = Shared.GetTarget(new(args));
            if (target == "pack" || target == "packonly")
                Shared.Targets.Pack(packOnly: target == "packonly");
            else if (target == "push" || target == "pushonly")
                Targets.Push(pushOnly: target == "pushonly");
        }
    }
}