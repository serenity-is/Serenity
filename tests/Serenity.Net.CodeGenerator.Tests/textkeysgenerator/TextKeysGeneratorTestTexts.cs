namespace Serenity.CodeGenerator;

/// <summary>
/// The texts that the test of the generator of the text keys reads from the assembly that it is in, as a project that
/// has texts of its own has them, see <see cref="TextKeysGeneratorTests"/>.
/// </summary>
[NestedLocalTexts(Prefix = "Test.Nested.")]
public static class TextKeysGeneratorTestTexts
{
    /// <summary>A text of the outermost class.</summary>
    public static readonly LocalText Hello = "Hello";

    /// <summary>A class in the outermost one, whose name is the same as the one of a key.</summary>
    public static class Sub_
    {
        /// <summary>A text of the nested class.</summary>
        public static readonly LocalText World = "World";
    }
}
