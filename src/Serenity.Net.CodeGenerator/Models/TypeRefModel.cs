namespace Serenity.CodeGenerator
{
    public class TypeRefModel
    {
        public string TypeName { get; set; }
        public string Arguments { get; set; }

        public string FullName
        {
            get => (TypeName ?? "") + ArgumentPart;
        }

        public string ArgumentPart {
            get => string.IsNullOrEmpty(Arguments) ? "" : $"({Arguments})";
        }
    }
}