using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Testing;
using System;
using System.Globalization;
using System.Threading;
using Xunit;

namespace Serenity.Test
{
    [Collection("AvoidParallel")]
    public class LocalTextTests
    {
        [Fact]
        public void LocalText_InvariantLanguageID_IsEmptyString()
        {
            Assert.Equal(String.Empty, LocalText.InvariantLanguageID);
        }

        [Fact]
        public void LocalText_Empty_IsALocalTextInstanceWithEmptyKey()
        {
            Assert.NotNull(LocalText.Empty);
            Assert.Equal(String.Empty, LocalText.Empty.Key);
        }

        [Fact]
        public void LocalText_Empty_ToStringReturnsEmptyString()
        {
            Assert.NotNull(LocalText.Empty);
            Assert.Equal(String.Empty, LocalText.Empty.ToString());
        }

        [Fact]
        public void LocalText_Constructor_AcceptsNullAndEmptyString()
        {
            new LocalText(null);
            new LocalText(String.Empty);
        }

        [Fact]
        public void LocalText_Key_ReturnsKeySetInConstructorAsIs()
        {
            Assert.Null(new LocalText(null).Key);
            Assert.Equal(String.Empty, new LocalText(String.Empty).Key);
            Assert.Equal("ABC", new LocalText("ABC").Key);
            Assert.Equal("  dEf ", new LocalText("  dEf ").Key);
            Assert.Equal("  dEf ", new LocalText("  dEf ").Key);
        }

        [Fact]
        public void LocalText_ImplicitConversionFromString_ReturnsLocalTextInstanceWithKey()
        {
            LocalText actual1 = "ABC";
            Assert.Equal("ABC", actual1.Key);

            LocalText actual2 = "";
            Assert.Equal("", actual2.Key);

            LocalText actual3 = "  dEf ";
            Assert.Equal("  dEf ", actual3.Key);
        }

        [Fact]
        public void LocalText_ImplicitConversionFromString_ReturnsEmptyForNullOrEmptyString()
        {
            LocalText actual1 = "";
            Assert.Equal(LocalText.Empty, actual1);

            string a = null;
            LocalText actual2 = a;
            Assert.Equal(LocalText.Empty, actual2);
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsNullIfInstanceIsNull()
        {
            LocalText text = null;
            string actual = text;
            Assert.Null(actual);
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsNullIfKeyIsNull()
        {
            LocalText text1 = new LocalText(null);
            string actual1 = text1;
            Assert.Null(actual1);
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsEmptyIfKeyIsEmpty()
        {
            LocalText text2 = new LocalText(String.Empty);
            string actual2 = text2;
            Assert.Equal(String.Empty, actual2);
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_DoesntThrowIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                new LocalText("Dummy");
            }
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsKeyAsIsIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                string actual1 = new LocalText("Dummy");
                Assert.Equal("Dummy", actual1);

                string actual2 = new LocalText(null);
                Assert.Null(actual2);

                string actual3 = new LocalText(String.Empty);
                Assert.Equal(String.Empty, actual3);
            }
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var text = new LocalText("Dummy");

                string translation = text;

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_UsesCurrentUICulture()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var text = new LocalText("Dummy");

                var oldCulture = Thread.CurrentThread.CurrentUICulture;
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    string translation = text;

                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                    translation = text;
                }
                finally
                {
                    Thread.CurrentThread.CurrentUICulture = oldCulture;
                }

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet("tr-TR", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet("en-GB", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsKeyIfNoTranslationIsFound()
        {
            using (new MunqContext())
            {
                const string key = "Db.MissingTable.MissingField";

                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .Returns(null);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var text = new LocalText(key);

                string translation = text;
                Assert.Equal(key, translation);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, key))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ImplicitConversionToString_ReturnsTranslationFromRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .ReturnsLazily((string l, string k) => l + ":" + k);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                string translation1 = new LocalText("Translation1");
                string translation2 = new LocalText("Translation2");
                string uiCulture = Thread.CurrentThread.CurrentUICulture.Name;

                Assert.Equal(uiCulture + ":Translation1", translation1);
                Assert.Equal(uiCulture + ":Translation2", translation2);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation1"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation2"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ToString_ReturnsNullIfKeyIsNull()
        {
            Assert.Null(new LocalText(null).ToString());
        }

        [Fact]
        public void LocalText_ToString_ReturnsEmptyIfKeyIsEmpty()
        {
            Assert.Equal(String.Empty, new LocalText(String.Empty).ToString());
        }

        [Fact]
        public void LocalText_ToString_DoesntThrowIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                new LocalText("Dummy").ToString();
            }
        }

        [Fact]
        public void LocalText_ToString_ReturnsKeyAsIsIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                Assert.Equal("Dummy", new LocalText("Dummy").ToString());

                Assert.Null(new LocalText(null).ToString());

                Assert.Equal(String.Empty, new LocalText(String.Empty).ToString());
            }
        }

        [Fact]
        public void LocalText_ToString_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                new LocalText("Dummy").ToString();

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ToString_UsesCurrentUICulture()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var oldCulture = Thread.CurrentThread.CurrentUICulture;
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    new LocalText("Dummy").ToString();

                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                    new LocalText("Dummy").ToString();
                }
                finally
                {
                    Thread.CurrentThread.CurrentUICulture = oldCulture;
                }

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet("tr-TR", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet("en-GB", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ToString_ReturnsKeyIfNoTranslationIsFound()
        {
            using (new MunqContext())
            {
                const string key = "Db.MissingTable.MissingField";

                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .Returns(null);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var translation = new LocalText(key).ToString();

                Assert.Equal(key, translation);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, key))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_ToString_ReturnsTranslationFromRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .ReturnsLazily((string l, string k) => l + ":" + k);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                string translation1 = new LocalText("Translation1").ToString();
                string translation2 = new LocalText("Translation2").ToString();
                string uiCulture = Thread.CurrentThread.CurrentUICulture.Name;

                Assert.Equal(uiCulture + ":Translation1", translation1);
                Assert.Equal(uiCulture + ":Translation2", translation2);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation1"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation2"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_Get_ReturnsNullIfKeyIsNull()
        {
            Assert.Null(LocalText.Get(null));
        }

        [Fact]
        public void LocalText_Get_ReturnsEmptyIfKeyIsEmpty()
        {
            Assert.Equal(String.Empty, LocalText.Get(String.Empty));
        }

        [Fact]
        public void LocalText_Get_DoesntThrowIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                LocalText.Get("Dummy");
            }
        }

        [Fact]
        public void LocalText_Get_ReturnsKeyAsIsIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                Assert.Equal("Dummy", LocalText.Get("Dummy"));

                Assert.Null(LocalText.Get(null));

                Assert.Equal(String.Empty, LocalText.Get(String.Empty));
            }
        }

        [Fact]
        public void LocalText_Get_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                LocalText.Get("Dummy");

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_Get_UsesCurrentUICulture()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var oldCulture = Thread.CurrentThread.CurrentUICulture;
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    LocalText.Get("Dummy");

                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                    LocalText.Get("Dummy");
                }
                finally
                {
                    Thread.CurrentThread.CurrentUICulture = oldCulture;
                }

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet("tr-TR", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet("en-GB", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_Get_ReturnsKeyIfNoTranslationIsFound()
        {
            using (new MunqContext())
            {
                const string key = "Db.MissingTable.MissingField";

                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .Returns(null);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var translation = LocalText.Get(key);

                Assert.Equal(key, translation);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, key))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_Get_ReturnsTranslationFromRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .ReturnsLazily((string l, string k) => l + ":" + k);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                string translation1 = LocalText.Get("Translation1").ToString();
                string translation2 = LocalText.Get("Translation2").ToString();
                string uiCulture = Thread.CurrentThread.CurrentUICulture.Name;

                Assert.Equal(uiCulture + ":Translation1", translation1);
                Assert.Equal(uiCulture + ":Translation2", translation2);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation1"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation2"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_TryGet_ReturnsNullIfKeyIsNull()
        {
            Assert.Null(LocalText.TryGet(null));
        }

        [Fact]
        public void LocalText_TryGet_ReturnsNullIfKeyIsEmpty()
        {
            Assert.Null(LocalText.TryGet(String.Empty));
        }

        [Fact]
        public void LocalText_TryGet_DoesntThrowIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                LocalText.TryGet("Dummy");
            }
        }

        [Fact]
        public void LocalText_TryGet_ReturnsNullIfNoLocalTextProvider()
        {
            using (new MunqContext())
            {
                Assert.Null(LocalText.TryGet("Dummy"));

                Assert.Null(LocalText.TryGet(null));

                Assert.Null(LocalText.TryGet(String.Empty));
            }
        }

        [Fact]
        public void LocalText_TryGet_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                LocalText.TryGet("Dummy");

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_TryGet_UsesCurrentUICulture()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var oldCulture = Thread.CurrentThread.CurrentUICulture;
                try
                {
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("tr-TR");
                    LocalText.TryGet("Dummy");

                    Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
                    LocalText.TryGet("Dummy");
                }
                finally
                {
                    Thread.CurrentThread.CurrentUICulture = oldCulture;
                }

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet("tr-TR", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet("en-GB", "Dummy"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_TryGet_ReturnsNullIfNoTranslationIsFound()
        {
            using (new MunqContext())
            {
                const string key = "Db.MissingTable.MissingField";

                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .Returns(null);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                var translation = LocalText.TryGet(key);

                Assert.Null(translation);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(A<string>._, key))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

        [Fact]
        public void LocalText_TryGet_ReturnsTranslationFromRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .ReturnsLazily((string l, string k) => l + ":" + k);

                Dependency.Resolve<IDependencyRegistrar>().RegisterInstance(registry);

                string translation1 = LocalText.TryGet("Translation1").ToString();
                string translation2 = LocalText.TryGet("Translation2").ToString();
                string uiCulture = Thread.CurrentThread.CurrentUICulture.Name;

                Assert.Equal(uiCulture + ":Translation1", translation1);
                Assert.Equal(uiCulture + ":Translation2", translation2);

                A.CallTo(() => registry.TryGet(A<string>._, A<string>._))
                    .MustHaveHappened(2, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation1"))
                    .MustHaveHappened(1, Times.Exactly);

                A.CallTo(() => registry.TryGet(uiCulture, "Translation2"))
                    .MustHaveHappened(1, Times.Exactly);
            }
        }

    }
}