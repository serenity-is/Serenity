using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using RazorGenerator.Templating;

namespace Serenity.CodeGenerator
{
    public static class Templates
    {
        public static string Render<T>(RazorTemplateBase template, T model)
        {
            ((dynamic)template).Model = model;
            return template.TransformText();
        }

        public static string Render<T>(RazorTemplateBase template, T model, GeneratorConfig config)
        {
            ((dynamic)template).Model = model;
            ((dynamic)template).Config = config;
            return template.TransformText();
        }
    }
}
