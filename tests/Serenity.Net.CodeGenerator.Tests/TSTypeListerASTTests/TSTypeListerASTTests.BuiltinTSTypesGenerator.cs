using Serenity.CodeGenerator;
using Serenity.Reflection;

namespace Serenity.Tests.CodeGenerator;

public partial class TSTypeListerASTTests
{
    //[Fact]
    protected void BuiltinTSTypesGenerator()
    {
        var path = Environment.CurrentDirectory.Replace('\\', '/');
        Assert.Contains("/Serenity/", path);
        var serenityRoot = path[0..(path.IndexOf("/Serenity/", StringComparison.Ordinal) + 10)];
        var packages = System.IO.Path.Combine(serenityRoot, "packages");

        var fileSystem = new MockFileSystem();
        var assembly = typeof(TSTypeListerASTTests).Assembly;
        var projectDirectory = fileSystem.Directory.GetCurrentDirectory();
        var typeLister = new TSTypeListerAST(fileSystem,
            tsConfigDir: projectDirectory, tsConfig: new TSConfig
            {
                CompilerOptions = new TSConfig.CompilerConfig
                {
                    Module = "ESNext"
                }
            });

        foreach (var package in new string[] { "corelib", "sleekgrid" })
        {
            var content = System.IO.File.ReadAllText(System.IO.Path.Combine(packages, package, "dist", "index.d.ts"));
            var targetPath = fileSystem.Combine(projectDirectory, "node_modules", "@serenity-is", package, "dist", "index.d.ts");
            fileSystem.Directory.CreateDirectory(fileSystem.GetDirectoryName(targetPath));
            fileSystem.File.WriteAllText(targetPath, content);
            typeLister.AddInputFile(targetPath);
        }

        var types = typeLister.ExtractTypes();

        var cb = new CodeWriter();
        cb.AppendLine("// Auto generated by TSTypeListerASTTests.BuiltinTSTypesGenerator, don't modify!");
        cb.AppendLine("namespace Serenity.CodeGeneration;");
        cb.AppendLine();
        cb.AppendLine("public class BuiltinTSTypes");
        cb.InBrace(() =>
        {
            var groupIndex = 0;
            cb.Indented("public static readonly ExternalType[] All = ");
            types.GroupBy(x => x.Module).OrderBy(x => x.Key).ToList().ForEach(module =>
            {
                if (groupIndex > 0)
                    cb.IndentedLine(".Concat(new ExternalType[]");
                else
                    cb.AppendLine("new ExternalType[]");
                cb.InBrace(() =>
                {
                    foreach (var type in module.Where(x => !x.Name.StartsWith('"')).OrderBy(x => x.Name))
                    {
                        cb.Indented("new() { Name = ");
                        cb.Append(type.Name.ToDoubleQuoted());

                        if (!string.IsNullOrEmpty(type.BaseType))
                        {
                            cb.Append(", BaseType = ");
                            cb.Append(type.BaseType.ToDoubleQuoted());
                        }

                        if (type.IsAbstract == true)
                        {
                            cb.Append(", IsAbstract = true");
                        }

                        if (type.IsDeclaration != true)
                        {
                            cb.Append(", IsDeclaration = false");
                        }

                        if (type.IsInterface != null)
                        {
                            cb.Append($", IsInterface = {(type.IsInterface == true ? "true" : false)}");
                        }

                        if (type.IsIntersectionType == true)
                        {
                            cb.Append(", IsIntersectionType = true");
                        }

                        if (type.Interfaces != null && type.Interfaces.Count > 0)
                        {
                            cb.Append(", Interfaces = [ ");
                            var intfIdx = 0;
                            foreach (var intf in type.Interfaces)
                            {
                                if (intfIdx++ > 0)
                                    cb.Append(", ");
                                cb.Append(intf.ToDoubleQuoted());
                            }
                            cb.Append(" ]");
                        }

                        if (type.Fields != null && type.Fields.Count > 0)
                        {
                            cb.Append(", Fields = [ ");
                            var fieldIdx = 0;
                            foreach (var field in type.Fields)
                            {
                                if (fieldIdx++ > 0)
                                    cb.Append(", ");
                                cb.Append("new() { Name = ");
                                cb.Append(field.Name.ToDoubleQuoted());
                                if (!string.IsNullOrEmpty(field.Type))
                                {
                                    cb.Append(", Type = ");
                                    cb.Append(field.Type.ToDoubleQuoted());
                                }
                                if (field.IsStatic == true)
                                {
                                    cb.Append(", IsStatic = true");
                                }
                                if (field.Value is bool b)
                                {
                                    cb.Append($", Value = {(b == true ? "true" : "false")}");
                                }
                                else if (field.Value is string s)
                                {
                                    cb.Append($", Value = {s.ToDoubleQuoted()}");
                                }
                                cb.Append(" }");
                            }
                            cb.Append(" ]");
                        }

                        if (type.GenericParameters != null && type.GenericParameters.Count > 0)
                        {
                            cb.Append(", GenericParameters = [ ");
                            var paramIdx = 0;
                            foreach (var gp in type.GenericParameters)
                            {
                                if (paramIdx++ > 0)
                                    cb.Append(", ");
                                cb.Append("new() { Name = ");
                                cb.Append(gp.Name.ToDoubleQuoted());
                                if (!string.IsNullOrEmpty(gp.Extends))
                                {
                                    cb.Append(", Extends = ");
                                    cb.Append(gp.Extends.ToDoubleQuoted());
                                }
                                if (!string.IsNullOrEmpty(gp.Default))
                                {
                                    cb.Append(", Default = ");
                                    cb.Append(gp.Default.ToDoubleQuoted());
                                }
                                cb.Append(" }");
                            }
                            cb.Append(" ]");
                        }

                        cb.AppendLine(" },");
                    }
                });
                cb.IndentedLine(".Select(x =>");
                cb.InBrace(() =>
                {
                    cb.Indented("x.Module = ");
                    cb.Append(module.Key.ToDoubleQuoted());
                    cb.AppendLine(";");
                    cb.Indented("x.SourceFile = ");
                    cb.Append(("builtin:/node_modules/" + module.Key + "/dist/index.d.ts").ToDoubleQuoted());
                    cb.AppendLine(";");
                    cb.IndentedLine("x.IsDeclaration ??= true;");
                    cb.IndentedLine("return x;");
                }, endLine: false);
                cb.AppendLine(groupIndex == 0 ? ")" : "))");
                groupIndex++;
            });
            cb.IndentedLine(".ToArray();");
        });

        System.IO.File.WriteAllText(System.IO.Path.Combine(serenityRoot, "src", "Serenity.Net.CodeGenerator", "CodeGeneration", "Model", "BuiltinTSTypes.gen.cs"), cb.ToString());
    }
}