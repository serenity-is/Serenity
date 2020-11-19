using Microsoft.AspNetCore.WebUtilities;
using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace Serenity.Web
{
    public partial class DynamicScriptManager
    {
        private class Item
        {
            private readonly DynamicScriptManager scriptManager;

            public Item(DynamicScriptManager scriptManager)
            {
                this.scriptManager = scriptManager;
            }

            public string Name;
            public IDynamicScript Generator;

            private Script content;

            public Script Content
            {
                get
                {
                    if (content.UncompressedBytes == null)
                        return content;

                    if (content.Expiration < DateTime.Now)
                    {
                        this.Reset();
                        return content;
                    }

                    if (Generator.GroupKey == null)
                        return content;

                    scriptManager.cache.GetLocalStoreOnly("DynamicScriptCheck:" + Name, TimeSpan.Zero,
                        Generator.GroupKey, () =>
                        {
                            Reset();
                            return new object();
                        });

                    return content;
                }
            }
           
            public Item(string name, IDynamicScript generator)
            {
                Name = name;
                Generator = generator;
                content = new Script
                {
                    Time = DateTime.UtcNow,
                    Hash = null,
                    UncompressedBytes = null,
                    CompressedBytes = null
                };

                generator.ScriptChanged += ScriptChanged;
            }

            private void ScriptChanged(object sender, EventArgs e)
            {
                Reset();
                scriptManager.RaiseScriptChanged(Name);
            }

            private static string GetMD5HashString(byte[] bytes)
            {
                var md5 = MD5.Create();
                byte[] result = md5.ComputeHash(bytes);
                return WebEncoders.Base64UrlEncode(result);
            }

            private Script GenerateContent()
            {
                byte[] ub = null;
                byte[] cb = null;

                string scriptText = Generator.GetScript();
                using (var ms = new MemoryStream(scriptText.Length))
                {
                    using (var sw = new StreamWriter(ms, new UTF8Encoding(true)))
                    {
                        sw.Write(scriptText);
                        sw.Flush();

                        ub = ms.ToArray();
                        ms.Seek(0, SeekOrigin.Begin);

                        if (ms.Length > 4096)
                        {
                            using (var cs = new MemoryStream((int)ms.Length))
                            {
                                using (var gz = new GZipStream(cs, CompressionMode.Compress))
                                {
                                    ms.CopyTo(gz);
                                    gz.Flush();
                                }

                                cb = cs.ToArray();
                            }
                        }
                    }

                    var script = new Script
                    {
                        Hash = GetMD5HashString(ub),
                        Time = DateTime.UtcNow,
                        ScriptText = scriptText,
                        CompressedBytes = cb,
                        UncompressedBytes = ub,
                        Expiration = Generator.Expiration == TimeSpan.Zero ? DateTime.MaxValue :
                            DateTime.Now.Add(Generator.Expiration)
                    };

                    content = script;

                    if (Generator.GroupKey == null)
                        return script;

                    scriptManager.cache.GetLocalStoreOnly("DynamicScriptCheck:" + Name, Generator.Expiration,
                        Generator.GroupKey, () =>
                        {
                            return new object();
                        });

                    return script;
                }
            }

            internal Script EnsureContentBytes()
            {
                var currentScript = Content;
                
                if (currentScript.UncompressedBytes == null)
                    return GenerateContent();

                return currentScript;
            }

            private void Reset()
            {
                content = new Script
                {
                    Time = DateTime.UtcNow,
                    Hash = null,
                    UncompressedBytes = null,
                    CompressedBytes = null
                };
            }
        }
    }
}