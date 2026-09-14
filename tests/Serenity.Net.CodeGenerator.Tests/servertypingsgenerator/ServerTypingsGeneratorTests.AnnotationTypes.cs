using ServerTypingsTest.Annotations;

namespace Serenity.CodeGeneration
{
    public partial class ServerTypingsGeneratorTests
    {
        [Fact]
        public void Form_Uses_Attributes_From_Annotation_Types()
        {
            var generator = CreateGenerator(typeof(FormWithAnnotations),
                typeof(AnnotatedByInterface), typeof(AnnotatedByAttribute),
                typeof(AnnotatedWrongNamespace), typeof(AnnotatedMissingProperty));
            generator.AddTSType(EditorTypeFor("InterfaceEditor", "Forms.InterfaceEditor"));
            generator.AddTSType(EditorTypeFor("AttributeEditor", "Forms.AttributeEditor"));

            var result = generator.Run();
            var code = Assert.Single(result, x => x.Filename == "Annotations/FormWithAnnotations.ts").Text;

            Assert.Contains("Name: InterfaceEditor;", code);
            Assert.Contains("Other: AttributeEditor;", code);
            Assert.DoesNotContain("WrongNamespace", code);
            Assert.DoesNotContain("MissingProperty", code);
        }

        private static ExternalType EditorTypeFor(string name, string key)
        {
            return new ExternalType
            {
                Name = name,
                Module = "MyModule",
                Attributes =
                [
                    new ExternalAttribute
                    {
                        Type = "registerEditor",
                        Arguments = [new ExternalArgument { Value = key }]
                    }
                ]
            };
        }
    }
}

namespace ServerTypingsTest.Annotations
{
    public interface IAnnotated
    {
    }

    public class MarkerAttribute : Attribute
    {
    }

    [AnnotationType(typeof(IAnnotated))]
    public class AnnotatedByInterface
    {
        [EditorType("Forms.InterfaceEditor")]
        public string Name { get; set; }
    }

    [AnnotationType(typeof(MarkerAttribute))]
    public class AnnotatedByAttribute
    {
        [EditorType("Forms.AttributeEditor")]
        public string Other { get; set; }
    }

    [AnnotationType(typeof(IAnnotated), Namespaces = ["ServerTypingsTest.Other.*"])]
    public class AnnotatedWrongNamespace
    {
        [EditorType("Forms.WrongNamespaceEditor")]
        public string Name { get; set; }
    }

    [AnnotationType(typeof(IAnnotated), Properties = ["DoesNotExist"])]
    public class AnnotatedMissingProperty
    {
        [EditorType("Forms.MissingPropertyEditor")]
        public string Name { get; set; }
    }

    [Marker]
    public class AnnotatedRow : IAnnotated
    {
        public string Name { get; set; }
        public string Other { get; set; }
    }

    [FormScript("Forms.Annotated")]
    [BasedOnRow(typeof(AnnotatedRow))]
    public class FormWithAnnotations
    {
        public string Name { get; set; }
        public string Other { get; set; }
    }
}
