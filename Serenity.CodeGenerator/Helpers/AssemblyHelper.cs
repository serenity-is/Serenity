using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Serenity.CodeGenerator
{
#if NET46
    /// <summary>
    ///     Helps to redirect assembly loading based on loaded assembly config file.
    /// </summary>
    internal class AssemblyHelper
    {
        public static void TryAppendRedirectionFromConfigFile(string fullName)
        {
            var fn = fullName + ".config";
            if (!File.Exists(fn))
                return;
            try
            {
                var cfg = XDocument.Load(fn);
                var els = cfg.Root.Element("runtime")
                    .Descendants(Ns + "assemblyBinding")
                    .Select(RedirectInfo.FromXElement).Where(a => a != null)
                    .ToArray();

                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    var an = new AssemblyName(args.Name);
                    var redirect = els.FirstOrDefault(a => a.CanRedirect(an));
                    if (redirect == null)
                        return null;
                    an = redirect.GetRedirectedAssemblyName();
                    return Assembly.Load(an);
                };
            }
            catch
            {
            }
        }

        private static Version GetVersion(string v, Version defaultValue)
        {
            v = v?.Trim();
            return string.IsNullOrEmpty(v) ? defaultValue : new Version(v);
        }

        private static readonly XNamespace Ns = "urn:schemas-microsoft-com:asm.v1";

        private class RedirectInfo
        {
            public static RedirectInfo FromXElement(XElement assemblyBinding)
            {
                try
                {
                    var dependentAssembly = assemblyBinding.Element(Ns + "dependentAssembly");
                    if (dependentAssembly == null)
                        return null;
                    var assemblyIdentity = dependentAssembly.Element(Ns + "assemblyIdentity");
                    var bindingRedirect = dependentAssembly.Element(Ns + "bindingRedirect");
                    if (assemblyIdentity == null || bindingRedirect == null)
                        return null;
                    var versions = ((string)bindingRedirect.Attribute("oldVersion")).Split('-');
                    var redirectInfo = new RedirectInfo
                    {
                        _name = (string)assemblyIdentity.Attribute("name"),
                        _oldVersionMin = GetVersion(versions.First(), new Version(0, 0, 0, 0)),
                        _oldVersionMax = GetVersion(versions.Last(),
                            new Version(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue)),
                        _newVersion = new Version((string)bindingRedirect.Attribute("newVersion"))
                    };
                    return redirectInfo;
                }
                catch
                {
                    return null;
                }
            }

            public bool CanRedirect(AssemblyName an)
            {
                if (an.Name != _name || an.Version < _oldVersionMin || an.Version > _oldVersionMax) return false;
                return an.Version != _newVersion;
            }

            public AssemblyName GetRedirectedAssemblyName()
            {
                var an = new AssemblyName
                {
                    Name = _name,
                    Version = _newVersion
                };
                return an;
            }

            private string _name;
            private Version _oldVersionMin;
            private Version _oldVersionMax;
            private Version _newVersion;
        }
    }
#endif
}