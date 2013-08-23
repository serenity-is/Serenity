using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using RazorEngine;
using System.Windows;
using RazorEngine.Templating;

namespace Serenity.CodeGenerator
{
    public static class Templates
    {
        public static string LocationRoot;
        private static HashSet<string> CompiledTemplates = new HashSet<string>();

        private static void CompileTemplate(string templateName, Type modelType)
        {
            bool loaded = typeof(Microsoft.CSharp.RuntimeBinder.Binder).Assembly != null;
            if (!loaded)
                throw new InvalidOperationException("Microsoft.CSharp assembly not loaded");

            var templatePath = Path.Combine(LocationRoot, templateName) + ".cshtml";
            try
            {
                using (var sr = new StreamReader(templatePath))
                {
                    var s = sr.ReadToEnd();
                    Razor.Compile(s, modelType, templateName);
                    CompiledTemplates.Add(templateName);
                }
            }
            catch (TemplateCompilationException te)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(te.ToString());
                sb.AppendLine();
                foreach (var error in te.Errors)
                {
                    sb.Append(Path.GetFileName(error.FileName));
                    sb.Append("[");
                    sb.Append(error.Line);
                    sb.Append(",");
                    sb.Append(error.Column);
                    sb.Append("]: ");
                    sb.Append(error.ErrorNumber);
                    sb.Append(" - ");
                    sb.Append(error.ErrorText);
                    sb.AppendLine();
                }

                MessageBox.Show(sb.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static string Render<T>(string templateName, T model)
        {
            if (!Templates.CompiledTemplates.Contains(templateName))
                CompileTemplate(templateName, typeof(T));
            return Razor.Run(templateName, model);
        }
    }
}
