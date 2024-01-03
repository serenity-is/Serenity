using Newtonsoft.Json.Converters;
using Serenity.JsonConverters;
using Serenity.TypeScript;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Xunit.Sdk;

namespace Serenity.Tests.CodeGenerator;

public partial class TypeScriptTests
{
    const string TestCaseExtension = ".testcase";
    const string TypeScriptCasesFolder = "/Sandbox/misc/TypeScript/tests/cases/compiler/";
    const string DummyCaseFile = "__dummyFile__";

    // For these tests to work, clone TypeScript repo, npm i, npm run build and
    // create a testcases.cjs at root with the following content:
    // 
    // var glob = require("glob");
    // var ts = require("./built/local/typescript");
    // const { createScanner, createProgram, ScriptTarget } = require("typescript");
    // 
    // var fs = require("fs");
    // var path = require("path");
    // var scanner = createScanner(ts.ScriptTarget.Latest, true);
    // var host = ts.createCompilerHost({});
    // for (var file of glob.sync("./tests/cases/compiler/**/*.{ts,tsx}")) {
    //     try {
    //         // stack overflow for forEachChild
    //         if (/binderBinaryExpressionStress(Js)?.ts$/.test(file))
    //             continue;
    //         var content = host.readFile(file);
    // 
    //         scanner.setText(content ?? "");
    //         var tokens = [];
    //         while (scanner.scan() != ts.SyntaxKind.EndOfFileToken) {
    //             tokens.push({
    //                 Kind: scanner.getToken(),
    //                 Text: scanner.getTokenText(),
    //                 Value: scanner.getTokenValue(),
    //                 FS: scanner.getTokenFullStart(),
    //                 TS: scanner.getTokenStart(),
    //                 TE: scanner.getTokenEnd(),
    //                 XE: scanner.hasExtendedUnicodeEscape(),
    //                 UE: scanner.hasUnicodeEscape(),
    //                 LB: scanner.hasPrecedingLineBreak(),
    //                 ID: scanner.isIdentifier(),
    //                 RW: scanner.isReservedWord(),
    //                 UT: scanner.isUnterminated()
    //             });
    //         }
    //         
    //         let program = createProgram([file], {
    //             target: ScriptTarget.Latest,
    //         }, host);
    //         var sourceFile = program.getSourceFile(file);
    //         var nodes = [];
    //         var visitor = node => {
    //             if (nodes.length > 100000) {
    //                 throw "Possible infinite recursion!";
    //             }
    //             nodes.push({
    //                 Kind: node.kind
    //             });
    //             return node.forEachChild(visitor);
    //         }
    //         
    //         sourceFile.forEachChild(visitor);
    // 
    //     } catch(e) {
    //         console.log('error parsing: ' + file + ': ' + e.toString());
    //         continue;
    //     }
    //     fs.writeFileSync(file.replace(/.tsx?$/, ".testcase"), JSON.stringify({
    //         Extension: path.extname(file),
    //         Content: content,
    //         Tokens: tokens,
    //         Nodes: nodes
    //     }));
    // }
    //
    // Run it with node ./testcases.cjs then set the TypeScriptTestCases constant
    // above to the full folder of cases
    private class TestCaseDataAttribute : DataAttribute
    {
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            if (!Directory.Exists(TypeScriptCasesFolder))
                return [[DummyCaseFile]];

            var fullPath = Path.GetFullPath(TypeScriptCasesFolder);
            var cases = new DirectoryInfo(TypeScriptCasesFolder).GetFiles("*" + TestCaseExtension, SearchOption.AllDirectories)
                .OrderBy(x => x.Length)
                .Select(x => new object[] { Path.ChangeExtension(Path.GetRelativePath(fullPath, x.FullName), null) })
                .Where(x => !TypeScriptCasesToSkip.Contains(x[0] as string))
                .ToArray();

            return cases.Length == 0 ? [[DummyCaseFile]] : cases;
        }
    }

    // There are known failing cases
    static readonly HashSet<string> TypeScriptCasesToSkip =
    [
        "bom-utf16be", // unicode issues
        "bom-utf16le", // unicode issues
        "dynamicImportsDeclaration", // top level await reparse issue
        "extendedUnicodePlaneIdentifiers",  // unicode issues
        "extendedUnicodePlaneIdentifiersJSDoc",  // unicode issues
        "identifierStartAfterNumericLiteral", // invalid syntax
        "parseBigInt",  // bigint parse issues
        "unicodeEscapesInNames02"  // unicode issues
    ];

    internal class TestCase
    {
        public string Content { get; set; }
        public string Extension { get; set; }
        public List<NodeData> Nodes { get; set; }
        public List<TokenData> Tokens { get; set; }

        internal class NodeData
        {
            public SyntaxKind Kind { get; set; }

            public override bool Equals(object obj)
            {
                return obj is NodeData other &&
                    other.Kind == Kind;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        internal class TokenData
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
                return obj is TokenData other &&
                    other.Kind == Kind &&
                    (other.Value == Value ||
                    (other.Value == "Infinity" && Value == "∞") ||
                    (other.Value == "∞" && Value == "Infinity") ||
                     (Kind == SyntaxKind.NumericLiteral ||
                      ((Kind > SyntaxKindMarker.FirstLiteralToken || Kind < SyntaxKindMarker.LastLiteralToken) &&
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
    }

    static readonly JsonSerializerOptions withEnumString;

    static TypeScriptTests()
    {
        withEnumString = new JsonSerializerOptions(JSON.Defaults.Strict);
        withEnumString.Converters.Remove(withEnumString.Converters.First(x => x is EnumJsonConverter));
        withEnumString.Converters.Insert(0, new JsonStringEnumConverter());
    }

    [TestCaseData]
    [Theory]
    public void Scanner_Outputs_Matching_Tokens(string file)
    {
        if (file == DummyCaseFile)
            return;

        var path = Path.Combine(TypeScriptCasesFolder, file) + TestCaseExtension;
        var testCase = JSON.Parse<TestCase>(File.ReadAllText(path));
        var scanner = new Scanner(ScriptTarget.Latest, skipTrivia: true);
        scanner.SetText(testCase.Content);
        var expectedTokens = testCase.Tokens;
        var actualTokens = new List<TestCase.TokenData>();
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
                    "Expected: " + JSON.Stringify(expectedTokens[i], withEnumString) + "\n" +
                    "Actual:   " + JSON.Stringify(actualTokens[i], withEnumString) + "\n");
            }
        }
    }

    [TestCaseData]
    [Theory]
    public void Parser_Outputs_Matching_Nodes(string file)
    {
        if (file == DummyCaseFile)
            return;

        var path = Path.Combine(TypeScriptCasesFolder, file) + TestCaseExtension;
        var testCase = JSON.Parse<TestCase>(File.ReadAllText(path));
        var sourceFile = new Parser().ParseSourceFile(Path.ChangeExtension(file, 
            testCase.Extension), testCase.Content);
        var expectedNodes = testCase.Nodes;
        var actualNodes = new List<TestCase.NodeData>();
        sourceFile.ForEachChild(node =>
        {
            actualNodes.Add(new()
            {
                Kind = node.Kind
            });

            return null;
        }, recursively: true);

        Assert.Equal(expectedNodes.Count, actualNodes.Count);
        for (var i = 0; i < actualNodes.Count; i++)
        {
            try
            {
                Assert.Equal(expectedNodes[i], actualNodes[i]);
            }
            catch
            {

                throw new Exception("Difference at node number: " + i + " of " + file + "\n" +
                    "Expected: " + JSON.Stringify(expectedNodes[i], withEnumString) + "\n" +
                    "Actual:   " + JSON.Stringify(actualNodes[i], withEnumString) + "\n");
            }
        }
    }
}
