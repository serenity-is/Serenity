using System;

namespace Build
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Shared.DetermineRoot();

            var target = Shared.GetTarget(new(args));
            switch (target) 
            {
                case "vsix":
                    Shared.Targets.PrepareVSIX();
                    break;

                case "patchpackagejson":
                    Shared.Targets.PatchPackageJsonCopy();
                    break;

                default:
                    Console.Error.WriteLine("Unknown target!");
                    break;
            }
        }
    }
}