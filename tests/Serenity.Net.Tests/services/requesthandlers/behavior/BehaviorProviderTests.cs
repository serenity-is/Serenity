using System.Collections;

namespace Serenity.Services;

public class BehaviorProviderTests
{
    public class NonFieldBehavior : IImplicitBehavior
    {
        public bool ShouldActivate { get; set; }
        public bool ActivateFor(IRow row) => ShouldActivate;
    }

    public class FieldBehavior : IImplicitBehavior, IFieldBehavior
    {
        public Field? Target { get; set; }
        public bool ShouldActivate { get; set; } = true;
        public bool ActivateFor(IRow row) => ShouldActivate;
    }

    public class PlainBehavior
    {
    }

    public abstract class AbstractImplicitBehavior : IImplicitBehavior
    {
        public abstract bool ActivateFor(IRow row);
    }

    public class ClassAttachedBehavior : IImplicitBehavior
    {
        public bool ActivateFor(IRow row) => true;
    }

    public class FieldAttachedBehavior : IImplicitBehavior, IFieldBehavior
    {
        public Field? Target { get; set; }
        public bool ActivateFor(IRow row) => true;
    }

    [AddBehavior(typeof(ClassAttachedBehavior))]
    private class BehaviorRow : Row<BehaviorRow.RowFields>, IRow
    {
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }

        [AddBehavior(typeof(FieldAttachedBehavior))]
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }

        public string? Extra { get => fields.Extra[this]; set => fields.Extra[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public StringField Extra;
#pragma warning restore CS0649
        }
    }

    private class PlainRow : Row<PlainRow.RowFields>, IRow
    {
        public int? ID { get => fields.ID[this]; set => fields.ID[this] = value; }
        public string? Name { get => fields.Name[this]; set => fields.Name[this] = value; }
        public string? Extra { get => fields.Extra[this]; set => fields.Extra[this] = value; }

        public class RowFields : RowFieldsBase
        {
#pragma warning disable CS0649
            public Int32Field ID;
            public StringField Name;
            public StringField Extra;
#pragma warning restore CS0649
        }
    }

    private class TestImplicitBehaviorRegistry(IEnumerable<Type> types) : IImplicitBehaviorRegistry
    {
        public IEnumerable<Type> GetTypes() => types;
    }

    private class TestBehaviorFactory(Func<Type, object?> factory) : IBehaviorFactory
    {
        public object CreateInstance(Type behaviorType) => factory(behaviorType);
    }

    private class TestBehaviorProvider : IBehaviorProvider
    {
        public IEnumerable Resolve(Type handlerType, Type rowType, Type behaviorType) => new[] { new NonFieldBehavior() };
    }

    public class AddBehaviorAttributeTests
    {
        [Fact]
        public void Constructor_Sets_Value()
        {
            var attr = new AddBehaviorAttribute(typeof(NonFieldBehavior));
            Assert.Equal(typeof(NonFieldBehavior), attr.Value);
        }

        [Fact]
        public void Constructor_Throws_For_Abstract_Type()
        {
            Assert.Throws<ArgumentException>(() => new AddBehaviorAttribute(typeof(AbstractImplicitBehavior)));
        }

        [Fact]
        public void Constructor_Throws_For_Interface_Type()
        {
            Assert.Throws<ArgumentException>(() => new AddBehaviorAttribute(typeof(IImplicitBehavior)));
        }
    }

    public class DefaultBehaviorFactoryTests
    {
        [Fact]
        public void Constructor_Throws_For_Null_Provider()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultBehaviorFactory(null));
        }

        [Fact]
        public void CreateInstance_Creates_Behavior()
        {
            var factory = new DefaultBehaviorFactory(new ServiceCollection().BuildServiceProvider());
            Assert.IsType<NonFieldBehavior>(factory.CreateInstance(typeof(NonFieldBehavior)));
        }
    }

    public class DefaultImplicitBehaviorRegistryTests
    {
        [Fact]
        public void Constructor_Throws_For_Null_TypeSource()
        {
            Assert.Throws<ArgumentNullException>(() => new DefaultImplicitBehaviorRegistry(null));
        }

        [Fact]
        public void GetTypes_Returns_Concrete_Implicit_Behaviors_Only()
        {
            var registry = new DefaultImplicitBehaviorRegistry(new MockTypeSource(
                typeof(NonFieldBehavior),
                typeof(AbstractImplicitBehavior),
                typeof(PlainBehavior)));

            var types = registry.GetTypes().ToList();
            Assert.Contains(typeof(NonFieldBehavior), types);
            Assert.DoesNotContain(typeof(AbstractImplicitBehavior), types);
            Assert.DoesNotContain(typeof(PlainBehavior), types);
        }
    }

    public class BehaviorProviderExtensionsTests
    {
        [Fact]
        public void Resolve_Generic_Casts_Behaviors()
        {
            var behaviors = new TestBehaviorProvider()
                .Resolve<BehaviorRow, NonFieldBehavior>(typeof(object))
                .ToList();

            Assert.Single(behaviors);
        }

        private interface IBase
        {
        }

        private interface ISource : IBase
        {
        }

        private interface INative : IBase
        {
        }

        private class Native : INative
        {
        }

        private class Source : ISource
        {
        }

        private class Invalid : IBase
        {
        }

        [Fact]
        public void AutoWrapBehaviors_Throws_For_Null_Input()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BehaviorProviderExtensions.AutoWrapBehaviors<IBase, ISource, INative>(null, x => new Native()).ToList());
        }

        [Fact]
        public void AutoWrapBehaviors_Throws_For_Null_Wrap()
        {
            Assert.Throws<ArgumentNullException>(() =>
                BehaviorProviderExtensions.AutoWrapBehaviors<IBase, ISource, INative>([new Native()], null).ToList());
        }

        [Fact]
        public void AutoWrapBehaviors_Returns_Native_As_Is()
        {
            var native = new Native();
            var result = BehaviorProviderExtensions
                .AutoWrapBehaviors<IBase, ISource, INative>([native], _ => new Native())
                .ToList();

            Assert.Same(native, Assert.Single(result));
        }

        [Fact]
        public void AutoWrapBehaviors_Wraps_Source()
        {
            var result = BehaviorProviderExtensions
                .AutoWrapBehaviors<IBase, ISource, INative>([new Source()], _ => new Native())
                .ToList();

            Assert.IsType<Native>(Assert.Single(result));
        }

        [Fact]
        public void AutoWrapBehaviors_Throws_For_Invalid_Behavior()
        {
            Assert.Throws<InvalidOperationException>(() =>
                BehaviorProviderExtensions
                    .AutoWrapBehaviors<IBase, ISource, INative>([new Invalid()], _ => new Native())
                    .ToList());
        }

        [Fact]
        public void AutoWrapBehaviors_Throws_For_Null_Behavior()
        {
            var exception = Assert.Throws<InvalidOperationException>(() =>
                BehaviorProviderExtensions
                    .AutoWrapBehaviors<IBase, ISource, INative>([null], _ => new Native())
                    .ToList());

            Assert.Contains("<null>", exception.Message);
        }
    }

    public class DefaultBehaviorProviderTests
    {
        [Fact]
        public void Constructor_Throws_For_Null_Registry()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultBehaviorProvider(null, new TestBehaviorFactory(t => Activator.CreateInstance(t))));
        }

        [Fact]
        public void Constructor_Throws_For_Null_Factory()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new DefaultBehaviorProvider(new TestImplicitBehaviorRegistry([]), null));
        }

        [Fact]
        public void Resolve_Returns_Empty_When_No_Behaviors()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([]),
                new TestBehaviorFactory(t => Activator.CreateInstance(t)));

            Assert.Empty(provider.Resolve(typeof(object), typeof(PlainRow), typeof(IImplicitBehavior)));
        }

        [Fact]
        public void Resolve_Activates_NonField_Behavior()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(NonFieldBehavior)]),
                new TestBehaviorFactory(t => new NonFieldBehavior { ShouldActivate = true }));

            var behaviors = provider.Resolve(typeof(object), typeof(PlainRow), typeof(IImplicitBehavior)).Cast<object>().ToList();

            Assert.Single(behaviors);
            Assert.IsType<NonFieldBehavior>(behaviors[0]);
        }

        [Fact]
        public void Resolve_Skips_NonActivated_Behavior()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(NonFieldBehavior)]),
                new TestBehaviorFactory(t => new NonFieldBehavior { ShouldActivate = false }));

            Assert.Empty(provider.Resolve(typeof(object), typeof(PlainRow), typeof(IImplicitBehavior)));
        }

        [Fact]
        public void Resolve_Skips_NonAssignable_Type()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(PlainBehavior)]),
                new TestBehaviorFactory(t => Activator.CreateInstance(t)));

            Assert.Empty(provider.Resolve(typeof(object), typeof(PlainRow), typeof(IImplicitBehavior)));
        }

        [Fact]
        public void Resolve_Skips_Null_Factory_Result()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(NonFieldBehavior)]),
                new TestBehaviorFactory(t => null));

            Assert.Empty(provider.Resolve(typeof(object), typeof(PlainRow), typeof(IImplicitBehavior)));
        }

        [Fact]
        public void Resolve_Skips_Behavior_Not_Implementing_IImplicitBehavior()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(PlainBehavior)]),
                new TestBehaviorFactory(t => new PlainBehavior()));

            Assert.Empty(provider.Resolve(typeof(object), typeof(PlainRow), typeof(PlainBehavior)));
        }

        [Fact]
        public void Resolve_Activates_Field_Behavior_For_Each_Field()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([typeof(FieldBehavior)]),
                new TestBehaviorFactory(t => new FieldBehavior { ShouldActivate = true }));

            var behaviors = provider.Resolve(typeof(object), typeof(PlainRow), typeof(IFieldBehavior))
                .OfType<FieldBehavior>().ToList();

            Assert.Equal(3, behaviors.Count);
            Assert.All(behaviors, b => Assert.NotNull(b.Target));
        }

        [Fact]
        public void Resolve_Adds_Class_Attached_Behavior()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([]),
                new TestBehaviorFactory(t => Activator.CreateInstance(t)));

            var behaviors = provider.Resolve(typeof(object), typeof(BehaviorRow), typeof(IImplicitBehavior))
                .Cast<object>().ToList();

            Assert.Contains(behaviors, b => b is ClassAttachedBehavior);
        }

        [Fact]
        public void Resolve_Adds_Field_Attached_Behavior()
        {
            var provider = new DefaultBehaviorProvider(
                new TestImplicitBehaviorRegistry([]),
                new TestBehaviorFactory(t => Activator.CreateInstance(t)));

            var behaviors = provider.Resolve(typeof(object), typeof(BehaviorRow), typeof(IFieldBehavior))
                .Cast<IFieldBehavior>().ToList();

            var attached = Assert.Single(behaviors, b => b is FieldAttachedBehavior);
            Assert.NotNull(attached.Target);
        }
    }
}
