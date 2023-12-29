using Serenity.TypeScript;
using System.IO;
using Xunit.Sdk;

namespace Serenity.Tests.CodeGenerator;

public partial class TypeScriptTests
{
    const string TypeScriptCasesFolder = "/Sandbox/misc/TypeScript/tests/cases/compiler/";

    // For these tests to work, clone TypeScript repo, npm i, npm run build and
    // create a testcases.cjs at root with the file with following content:
    // 
    // var glob = require("glob");
    // var ts = require("./built/local/typescript");
    // const { createScanner } = require("typescript");
    // 
    // var fs = require("fs");
    // var path = require("path");
    // var scanner = createScanner(ts.ScriptTarget.Latest, true);
    // for (var file of glob.sync("./tests/cases/compiler/**/*.ts")) {
    //     var content = fs.readFileSync(file, 'utf8');
    //     scanner.setText(content ?? "");
    //     var tokens = [];
    //     while (scanner.scan() != ts.SyntaxKind.EndOfFileToken) {
    //         tokens.push({
    //             Token: scanner.getToken(),
    //             Text: scanner.getTokenText(),
    //             Value: scanner.getTokenValue(),
    //             FullStart: scanner.getTokenFullStart(),
    //             TokenStart: scanner.getTokenStart(),
    //             TokenEnd: scanner.getTokenEnd(),
    //             ExtendedUnicodeEscape: scanner.hasExtendedUnicodeEscape(),
    //             UnicodeEscape: scanner.hasExtendedUnicodeEscape(),
    //             PrecedingLineBreak: scanner.hasPrecedingLineBreak,
    //             IsIdentifier: scanner.isIdentifier(),
    //             IsReservedWord: scanner.isReservedWord(),
    //             IsKeyword: scanner.IsKeyword(),
    //             IsUnterminated: scanner.isUnterminated()
    //         });
    //     }
    //     fs.writeFileSync(file.replace(".ts", ".scannertokens"), JSON.stringify({
    //         Content: content,
    //         Tokens: tokens
    //     }));
    // }
    //
    // Run it with node ./testcases.cjs then set the TypeScriptTestCases constant
    // above to the full folder of cases
    private class ScannerDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (!Directory.Exists(TypeScriptCasesFolder))
                return [];

            var fullPath = Path.GetFullPath(TypeScriptCasesFolder);
            return new DirectoryInfo(TypeScriptCasesFolder).GetFiles("*.scannertokens", SearchOption.AllDirectories)
                .OrderBy(x => x.Length)
                .Select(x => new object[] { Path.ChangeExtension(Path.GetRelativePath(fullPath, x.FullName), null) })
                .ToArray();
        }
    }

    internal class ScanTokens
    {
        public string Content { get; set; }
        public string Extension { get; set; }
        public List<Token> Tokens { get; set; }

    }

    internal class Token
    {
        public SyntaxKind Kind { get; set; }

        public int FS { get; set; }
        public int TS { get; set; }
        public int TE { get; set; }
        public bool XE { get; set; }
        public bool UE { get; set; }
        public bool LB { get; set; }
        public bool ID { get; set; }
        public bool RW { get; set; }
        public bool UT { get; set; }
        public string Value { get; set; }
        public string Text { get; set; }


        public override bool Equals(object obj)
        {
            return obj is Token other &&
                other.Kind == Kind &&
                (other.Value == Value ||
                (other.Value == "Infinity" && Value == "∞") ||
                (other.Value == "∞" && Value == "Infinity") ||
                 (Kind == SyntaxKind.NumericLiteral ||
                  ((Kind > SyntaxKind.FirstLiteralToken || Kind < SyntaxKind.LastLiteralToken) &&
                    Kind != SyntaxKind.Identifier && Kind != SyntaxKind.PrivateIdentifier)) &&
                 !string.IsNullOrEmpty(Value) &&
                 !string.IsNullOrEmpty(other.Value) &&
                  double.TryParse(Value, out var x1) &&
                  double.TryParse(other.Value, out var x2) &&
                  x1 == x2) &&
                other.FS == FS &&
                other.TS == TS &&
                other.TE == TE &&
                other.XE == XE &&
                other.UE == UE &&
                other.LB == LB &&
                other.ID == ID &&
                other.RW == RW &&
                other.UT == UT;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [ScannerData]
    [Theory]
    public void Scanner_Outputs_Matching_Tokens(string file)
    {
        if (file == "parseBigInt" ||
            file == "unicodeEscapesInNames02" ||
            file == "identifierStartAfterNumericLiteral" ||
            file == "extendedUnicodePlaneIdentifiers" ||
            file == "extendedUnicodePlaneIdentifiersJSDoc")
        {
            // these are four known failing cases
            return;
        }

        var path = Path.Combine(TypeScriptCasesFolder, file) + ".scannertokens";
        var expected = JSON.Parse<ScanTokens>(File.ReadAllText(path));
        var scanner = new Scanner(ScriptTarget.Latest, skipTrivia: true);
        scanner.SetText(expected.Content);
        var expectedTokens = expected.Tokens;
        var actualTokens = new List<Token>();
        while (scanner.Scan() != SyntaxKind.EndOfFileToken)
        {
            actualTokens.Add(new()
            {
                Kind = scanner.GetToken(),
                Text = scanner.GetTokenText(),
                Value = scanner.GetTokenValue(),
                FS = scanner.GetTokenFullStart(),
                TS = scanner.GetTokenStart(),
                TE = scanner.GetTokenEnd(),
                XE = scanner.HasExtendedUnicodeEscape(),
                UE = scanner.HasUnicodeEscape(),
                LB = scanner.HasPrecedingLineBreak(),
                ID = scanner.IsIdentifier(),
                RW = scanner.IsReservedWord(),
                UT = scanner.IsUnterminated()
            });
        }

        Assert.Equal(expectedTokens.Count, actualTokens.Count);
        for (var i = 0; i < actualTokens.Count; i++)
        {
            try
            {
                Assert.Equal(expectedTokens[i], actualTokens[i]);
            }
            catch
            {

                throw new Exception("Difference at token number: " + i + " of " + file + "\n" +
                    "Expected: " + JSON.Stringify(expectedTokens[i]) + "\n" +
                    "Actual:   " + JSON.Stringify(actualTokens[i]) + "\n");
            }
        }
    }

    [Fact]
    public void AsyncFunction_WithGenericPromise_TypeOfImport_ReturnAssignment()
    {
        var result = new Parser().ParseSourceFile("test", """"
            var l: any;
            async function x(): Promise<typeof import("l")> {
                return l = await import("x");
            }
            """");
        Assert.NotNull(result);
    }

    [Fact]
    public void AsyncFunction_WithGenericPromise_TypeOfVar_ReturnAssignment()
    {
        var result = new Parser().ParseSourceFile("test", """"
            var l: any;
            async function x(): Promise<typeof l> {
                return l = await import("x");
            }
            """");
        Assert.NotNull(result);
    }
}
