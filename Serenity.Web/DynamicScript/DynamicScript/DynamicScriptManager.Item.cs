using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
#if ASPNETCORE
using Microsoft.AspNetCore.WebUtilities;
#else
using System.Web;
#endif

namespace Serenity.Web
{
    public static partial class DynamicScriptManager
    {
        private class Item
        {
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

                    TwoLevelCache.GetLocalStoreOnly("DynamicScriptCheck:" + this.Name, TimeSpan.Zero,
                        Generator.GroupKey, () =>
                        {
                            this.Reset();
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
            }

            private static string GetMD5HashString(byte[] bytes)
            {
#if COREFX
                var md5 = MD5.Create();
                byte[] result = md5.ComputeHash(bytes);
                return WebEncoders.Base64UrlEncode(result);
#else
                var md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(bytes);
                return HttpServerUtility.UrlTokenEncode(result);
#endif
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
                        CompressedBytes = cb,
                        UncompressedBytes = ub,
                        Expiration = Generator.Expiration == TimeSpan.Zero ? DateTime.MaxValue :
                            DateTime.Now.Add(Generator.Expiration)
                    };

                    this.content = script;

                    if (Generator.GroupKey == null)
                        return script;

                    TwoLevelCache.GetLocalStoreOnly("DynamicScriptCheck:" + this.Name, Generator.Expiration,
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
                this.content = new Script
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