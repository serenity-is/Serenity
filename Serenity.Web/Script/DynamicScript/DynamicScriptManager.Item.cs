using System;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Serenity.Web
{
    public static partial class DynamicScriptManager
    {
        private class Item
        {
            public string Name;
            public IDynamicScript Generator;
            public bool NonCached;

            private Script content;

            public Script Content
            {
                get
                {
                    var twoLevel = Generator as ITwoLevelCached;
                    if (twoLevel == null || twoLevel.GlobalGenerationKey == null)
                        return content;

                    if (content.UncompressedBytes == null)
                        return content;

                    TwoLevelCache.GetLocalStoreOnly("DynamicScriptCheck:" + this.Name, TimeSpan.Zero,
                        twoLevel.GlobalGenerationKey, () =>
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
                    Hash = DateTime.Now.Ticks.ToString(),
                    UncompressedBytes = null,
                    CompressedBytes = null
                };
                NonCached = false;
                generator.ScriptChanged += ScriptChanged;
            }

            private void ScriptChanged(object sender, EventArgs e)
            {
                Reset();
            }

            private static string GetMD5HashString(byte[] bytes)
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] result = md5.ComputeHash(bytes);

                return HttpServerUtility.UrlTokenEncode(result);
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
                        UncompressedBytes = ub
                    };

                    this.content = script;

                    var twoLevel = Generator as ITwoLevelCached;
                    if (twoLevel == null || twoLevel.GlobalGenerationKey == null)
                        return script;

                    TwoLevelCache.GetLocalStoreOnly("DynamicScriptCheck:" + this.Name, TimeSpan.Zero,
                        twoLevel.GlobalGenerationKey, () =>
                        {
                            return new object();
                        });

                    return script;
                }
            }

            internal Script EnsureContentBytes()
            {
                var currentScript = Content;
                
                if (currentScript.UncompressedBytes == null || NonCached)
                    return GenerateContent();

                return currentScript;
            }

            private void Reset()
            {
                this.content = new Script
                {
                    Time = DateTime.UtcNow,
                    Hash = DateTime.Now.Ticks.ToString(),
                    UncompressedBytes = null,
                    CompressedBytes = null
                };
            }
        }
    }
}