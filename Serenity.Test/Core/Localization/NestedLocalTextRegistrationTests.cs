using FakeItEasy;
using Serenity.Abstractions;
using Serenity.Extensibility;
using Serenity.Localization;
using Serenity.Testing;
using System;
using System.Collections.Generic;
using Xunit;

namespace Serenity.Test
{
    public class NestedLocalTextRegistrationTests
    {
        [Fact]
        public void NestedLocalTextRegistration_Initialize_ThrowsKeyNotFound_IfNoLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<KeyNotFoundException>(() =>
                {
                    NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });
                });

                Assert.Contains(typeof(ILocalTextRegistry).Name, exception.Message);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_UsesRegisteredLocalTextRegistry()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustHaveHappened();
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_ThrowsArgumentNull_IfAssembliesIsNull()
        {
            using (new MunqContext())
            {
                var exception = Assert.Throws<ArgumentNullException>(() =>
                {
                    NestedLocalTextRegistration.Initialize(null);
                });
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_OnlyRunsOnSpecifiedAssemblies()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { typeof(System.Object).Assembly });

                A.CallTo(() => registry.Add(A<string>._, A<string>._, A<string>._))
                    .MustNotHaveHappened();
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_SkipsClassesWithoutNestedLocalTextsAttribute()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "N.N1", "5"))
                    .MustNotHaveHappened();

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "Y.Y1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_UsesInvariantLanguageAndNoPrefixByDefault()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "Y.Y1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "Y.Y2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "Y.YI.Y3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "Y4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }


        [Fact]
        public void NestedLocalTextRegistration_Initialize_UsesLanguageIfSpecified()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add("es", "Z.Z1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("es", "Z.Z2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("es", "Z.ZI.Z3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("es", "Z4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_UsesPrefixIfSpecified()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "p.P.P1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "p.P.P2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "p.P.PI.P3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add(LocalText.InvariantLanguageID, "p.P4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_UsesLanguageAndPrefixIfSpecified()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry.Add("jp", "x.X.X1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("jp", "x.X.X2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("jp", "x.X.XI.X3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry.Add("jp", "x.X4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_ReplacesLocalTextObjectsWithInitializedLocalTextObjects()
        {
            using (new MunqContext())
            {
                var registry = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                Assert.IsType<InitializedLocalText>(Texts.Y.Y1);
                Assert.IsType<InitializedLocalText>(Texts.Y.Y2);
                Assert.IsType<InitializedLocalText>(Texts.Y.YI.Y3);
                Assert.IsType<InitializedLocalText>(Texts.Y4);

                Assert.Equal("Y.Y1", Texts.Y.Y1.Key);
                Assert.Equal("1", ((InitializedLocalText)Texts.Y.Y1).InitialText);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_CanBeCalledMoreThanOnce()
        {
            using (new MunqContext())
            {
                var registry1 = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry1);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y.Y1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y.Y2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y.YI.Y3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                var registry2 = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry2);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry2.Add(LocalText.InvariantLanguageID, "Y.Y1", "1"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry2.Add(LocalText.InvariantLanguageID, "Y.Y2", "2"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry2.Add(LocalText.InvariantLanguageID, "Y.YI.Y3", "3"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry2.Add(LocalText.InvariantLanguageID, "Y4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [Fact]
        public void NestedLocalTextRegistration_Initialize_RemovesLastUnderscoreFromClassName()
        {
            using (new MunqContext())
            {
                var registry1 = A.Fake<ILocalTextRegistry>();
                Dependency.Resolve<IDependencyRegistrar>()
                    .RegisterInstance(registry1);

                NestedLocalTextRegistration.Initialize(new[] { this.GetType().Assembly });

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y4", "4"))
                    .MustHaveHappened(Repeated.Exactly.Once);

                A.CallTo(() => registry1.Add(LocalText.InvariantLanguageID, "Y4.Y5", "5"))
                    .MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        public class NotNestedTexts
        {
            public class N
            {
                public static readonly LocalText N1 = "5";
            }
        }

        [NestedLocalTexts]
        public class Texts
        {
            public class Y
            {
                public static readonly LocalText Y1 = "1";
                public static readonly LocalText Y2 = "2";

                public class YI
                {
                    public static readonly LocalText Y3 = "3";
                }
            }

            public static readonly LocalText Y4 = "4";

            // To prevent clash with Y4, append a underscore that will be removed when determining keys
            public class Y4_
            {
                public static readonly LocalText Y5 = "5";
            }
        }

        [NestedLocalTexts(LanguageID = "es")]
        public class TextsWithLanguage
        {
            public class Z
            {
                public static readonly LocalText Z1 = "1";
                public static readonly LocalText Z2 = "2";

                public class ZI
                {
                    public static readonly LocalText Z3 = "3";
                }
            }

            public static readonly LocalText Z4 = "4";
        }

        [NestedLocalTexts(Prefix = "p.")]
        public class TextsWithPrefix
        {
            public class P
            {
                public static readonly LocalText P1 = "1";
                public static readonly LocalText P2 = "2";

                public class PI
                {
                    public static readonly LocalText P3 = "3";
                }
            }

            public static readonly LocalText P4 = "4";
        }

        [NestedLocalTexts(Prefix = "x.", LanguageID = "jp")]
        public class TextsWithPrefixAndLanguage
        {
            public class X
            {
                public static readonly LocalText X1 = "1";
                public static readonly LocalText X2 = "2";

                public class XI
                {
                    public static readonly LocalText X3 = "3";
                }
            }

            public static readonly LocalText X4 = "4";
        }
    }
}