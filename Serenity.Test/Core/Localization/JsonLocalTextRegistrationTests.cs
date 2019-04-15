using FakeItEasy;
using Newtonsoft.Json.Linq;
using Serenity.Abstractions;
using Serenity.Localization;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public class JsonLocalTextRegistrationTests
    {
        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_ThrowsArgumentNull_IfNestedIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                JsonLocalTextRegistration.AddFromNestedDictionary(null, "x", "en");
            });
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_ThrowsKeyNotFound_IfNoLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() =>
                {
                    var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'5'}");
                    JsonLocalTextRegistration.AddFromNestedDictionary(dict, "pre", "en");
                });

                Assert.Contains(typeof(ILocalTextRegistry).Name, exception.Message);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'5'}");
                JsonLocalTextRegistration.AddFromNestedDictionary(dict, "pre", "en");

                A.CallTo(() => registry.Add("en", "prex", "5"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_HandlesSimpleDictionary()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'5', 'y.z': 'a.b.c'}");
                JsonLocalTextRegistration.AddFromNestedDictionary(dict, "pre.", "es");

                A.CallTo(() => registry.Add("es", "pre.x", "5"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add("es", "pre.y.z", "a.b.c"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_HandlesHierarchicalDictionary()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'x',y:{z:{u:{l:'l',m:'m'},t:'t'}}}");
                JsonLocalTextRegistration.AddFromNestedDictionary(dict, "Db.", "jp");

                A.CallTo(() => registry.Add("jp", "Db.x", "x"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add("jp", "Db.y.z.u.l", "l"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add("jp", "Db.y.z.u.m", "m"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add("jp", "Db.y.z.t", "t"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(4, Times.Exactly);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromNestedDictionary_SkipsNullValues()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'x',y:null}");
                JsonLocalTextRegistration.AddFromNestedDictionary(dict, "Db.", "jp");

                A.CallTo(() => registry.Add("jp", "Db.x", "x"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_ProcessNestedDictionary_ThrowsArgumentNull_IfNestedIsNull()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                JsonLocalTextRegistration.ProcessNestedDictionary(null, "x", new Dictionary<string,string>());
            });
        }

        [Fact]
        public void JsonLocalTextRegistration_ProcessNestedDictionary_HandlesSimpleDictionary()
        {
            var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'5', 'y.z': 'a.b.c'}");
            var target = new Dictionary<string, string>();
            JsonLocalTextRegistration.ProcessNestedDictionary(dict, "pre.", target);

            Assert.Equal(2, target.Count);

            Assert.True(target.ContainsKey("pre.x"));
            Assert.Equal("5", target["pre.x"]);

            Assert.True(target.ContainsKey("pre.y.z"));
            Assert.Equal("a.b.c", target["pre.y.z"]);
        }

        [Fact]
        public void JsonLocalTextRegistration_ProcessNestedDictionary_HandlesHierarchicalDictionary()
        {
            var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'x',y:{z:{u:{l:'l',m:'m'},t:'t'}}}");
            var target = new Dictionary<string, string>();
            JsonLocalTextRegistration.ProcessNestedDictionary(dict, "Db.", target);

            Assert.Equal(4, target.Count);

            Assert.True(target.ContainsKey("Db.x"));
            Assert.Equal("x", target["Db.x"]);

            Assert.True(target.ContainsKey("Db.y.z.u.l"));
            Assert.Equal("l", target["Db.y.z.u.l"]);

            Assert.True(target.ContainsKey("Db.y.z.u.m"));
            Assert.Equal("m", target["Db.y.z.u.m"]);

            Assert.True(target.ContainsKey("Db.y.z.t"));
            Assert.Equal("t", target["Db.y.z.t"]);
        }

        [Fact]
        public void JsonLocalTextRegistration_ProcessNestedDictionary_SkipsNullValues()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var dict = JSON.Parse<Dictionary<string, JToken>>("{x:'x',y:null}");
                var target = new Dictionary<string, string>();
                JsonLocalTextRegistration.ProcessNestedDictionary(dict, "Db.", target);

                Assert.StrictEqual(1, target.Count);

                Assert.True(target.ContainsKey("Db.x"));
                Assert.Equal("x", target["Db.x"]);
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_ThrowsArgumentNull_IfPathIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => JsonLocalTextRegistration.AddFromFilesInFolder(null));
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_Ignores_IfDirectoryDoesntExist()
        {
            JsonLocalTextRegistration.AddFromFilesInFolder(@"c:\s_o_m_e_f_o_l_d_e_r");
        }

        private string CreateTempFolder()
        {
            var temporary = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(temporary);
            return temporary;
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_ThrowsKeyNotFound_IfNoLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "texts.en.json"), 
                        "{x:'5'}");

                    var exception = Assert.Throws<KeyNotFoundException>(() =>
                    {
                        JsonLocalTextRegistration.AddFromFilesInFolder(temporary);
                    });

                    Assert.Contains(typeof(ILocalTextRegistry).Name, exception.Message);
                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "texts.en.json"),
                        "{x:'5'}");

                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    A.CallTo(() => registry.Add("en", "x", "5"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .MustHaveHappened(1, Times.Exactly);
                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_HandlesSimpleDictionary()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "my.es.json"),
                        "{x:'5', 'y.z': 'a.b.c'}");

                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    A.CallTo(() => registry.Add("es", "x", "5"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("es", "y.z", "a.b.c"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .MustHaveHappened(2, Times.Exactly);
                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_HandlesHierarchicalDictionary()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "jp.json"),
                        "{x:'x',y:{z:{u:{l:'l',m:'m'},t:'t'}}}");

                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    A.CallTo(() => registry.Add("jp", "x", "x"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("jp", "y.z.u.l", "l"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("jp", "y.z.u.m", "m"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("jp", "y.z.t", "t"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .MustHaveHappened(4, Times.Exactly);

                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_SkipsNullValues()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "jp.json"),
                        "{x:'x',y:null}");

                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    A.CallTo(() => registry.Add("jp", "x", "x"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .MustHaveHappened(1, Times.Exactly);

                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_DeterminesLanguageIDsProperly()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "jp.json"), "{x:'1'}");
                    File.WriteAllText(Path.Combine(temporary, "en-US.json"), "{x:'2'}");
                    File.WriteAllText(Path.Combine(temporary, "texts.en-GB.json"), "{x:'3'}");
                    File.WriteAllText(Path.Combine(temporary, "texts.en-US.en.json"), "{x:'4'}");
                    File.WriteAllText(Path.Combine(temporary, "my.some.long.prefix.tr-TR.json"), "{x:'5'}");

                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    A.CallTo(() => registry.Add("jp", "x", "1"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("en-US", "x", "2"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("en-GB", "x", "3"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("en", "x", "4"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add("tr-TR", "x", "5"))
                        .MustHaveHappened(1, Times.Exactly);

                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .MustHaveHappened(5, Times.Exactly);
                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

        [Fact]
        public void JsonLocalTextRegistration_AddFromFilesInFolder_SortsFilesByFilename()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                var temporary = CreateTempFolder();
                try
                {
                    File.WriteAllText(Path.Combine(temporary, "z.json"), "{x:'1'}");
                    File.WriteAllText(Path.Combine(temporary, "a.json"), "{x:'2'}");
                    File.WriteAllText(Path.Combine(temporary, "t.json"), "{x:'3'}");
                    File.WriteAllText(Path.Combine(temporary, "b.json"), "{x:'4'}");
                    File.WriteAllText(Path.Combine(temporary, "0.json"), "{x:'5'}");

                    var list = new List<string>();
                    A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                        .Invokes((string lang, string key, string text) => list.Add(text));
                    
                    JsonLocalTextRegistration.AddFromFilesInFolder(temporary);

                    Assert.Equal(5, list.Count);

                    Assert.Equal("5", list[0]);
                    Assert.Equal("2", list[1]);
                    Assert.Equal("4", list[2]);
                    Assert.Equal("3", list[3]);
                    Assert.Equal("1", list[4]);
                }
                finally
                {
                    Directory.Delete(temporary, true);
                }
            }
        }

    }
}