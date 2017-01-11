using System;
using System.Globalization;
using System.Text;
using System.IO;

namespace RazorGenerator.Templating
{
    public class RazorTemplateBase
    {
        private string _content;
        public RazorTemplateBase Layout { get; set; }

        private StringBuilder _generatingEnvironment = new System.Text.StringBuilder();
        private TextWriter _output;

        private TextWriter Output { get { return _output ?? (_output = new StringWriter(_generatingEnvironment)); } }

        public virtual void Execute()
        {
        }

        public void WriteLiteral(string textToAppend)
        {
            if (String.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            _generatingEnvironment.Append(textToAppend); ;
        }

        public void Write(object value)
        {
            if ((value == null))
            {
                return;
            }

            WriteLiteral(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public void Write(bool value) { WriteLiteral(value.ToString()); }
        public void Write(int value) { WriteLiteral(value.ToString(CultureInfo.InvariantCulture)); }
        public void Write(long value) { WriteLiteral(value.ToString(CultureInfo.InvariantCulture)); }
        public void Write(float value) { WriteLiteral(value.ToString(CultureInfo.InvariantCulture)); }
        public void Write(double value) { WriteLiteral(value.ToString(CultureInfo.InvariantCulture)); }
        public void Write(decimal value) { WriteLiteral(value.ToString(CultureInfo.InvariantCulture)); }

        public string RenderBody()
        {
            return _content;
        }

        public string TransformText()
        {
            Execute();
            if (Layout != null)
            {
                Layout._content = _generatingEnvironment.ToString();
                return Layout.TransformText();
            }
            else
            {
                return _generatingEnvironment.ToString();
            }
        }

        public void Clear()
        {
            _generatingEnvironment.Clear();

            if (Layout != null)
            {
                Layout._content = "";
            }
        }

        public void WriteLiteralTo(TextWriter writer, string text)
        {
            writer.Write(text);
        }

        public void WriteTo(TextWriter writer, object value)
        {
            writer.Write(Convert.ToString(value, CultureInfo.InvariantCulture));
        }

        public void WriteTo(TextWriter writer, bool value) { writer.Write(value.ToString()); }
        public void WriteTo(TextWriter writer, int value) { writer.Write(value.ToString(CultureInfo.InvariantCulture)); }
        public void WriteTo(TextWriter writer, long value) { writer.Write(value.ToString(CultureInfo.InvariantCulture)); }
        public void WriteTo(TextWriter writer, float value) { writer.Write(value.ToString(CultureInfo.InvariantCulture)); }
        public void WriteTo(TextWriter writer, double value) { writer.Write(value.ToString(CultureInfo.InvariantCulture)); }
        public void WriteTo(TextWriter writer, decimal value) { writer.Write(value.ToString(CultureInfo.InvariantCulture)); }

        // WriteAttribute is used by Razor runtime v2 and v3.

        public void WriteAttribute(string name,
                                   Tuple<string, int> prefix,
                                   Tuple<string, int> suffix,
                                   params object[] fragments)
        {
            WriteAttributeTo(Output, name, prefix, suffix, fragments);
        }

        public void WriteAttributeTo(TextWriter writer, string name,
                                     Tuple<string, int> prefix,
                                     Tuple<string, int> suffix,
                                     params object[] fragments)
        {
            // For sake of compatibility, this implementation is adapted from
            // System.Web.WebPages.WebPageExecutingBase as found in ASP.NET
            // web stack release 3.2.2:
            // https://github.com/ASP-NET-MVC/aspnetwebstack/releases/tag/v3.2.2

            #region Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

            // Licensed under the Apache License, Version 2.0 (the "License")
            // may not use this file except in compliance with the License. You may
            // obtain a copy of the License at
            //
            // http://www.apache.org/licenses/LICENSE-2.0
            //
            // Unless required by applicable law or agreed to in writing, software
            // distributed under the License is distributed on an "AS IS" BASIS,
            // WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or
            // implied. See the License for the specific language governing permissions
            // and limitations under the License.

            #endregion

            if (fragments.Length == 0)
            {
                WriteLiteralTo(writer, prefix.Item1);
                WriteLiteralTo(writer, suffix.Item1);
            }
            else
            {
                var first = true;
                var wroteSomething = false;
                foreach (var fragment in fragments)
                {
                    var sf = fragment as Tuple<Tuple<string, int>, Tuple<string, int>, bool>;
                    var of = sf == null ? (Tuple<Tuple<string, int>, Tuple<object, int>, bool>)fragment : null;

                    var ws = sf != null ? sf.Item1.Item1 : of.Item1.Item1;
                    var literal = sf != null ? sf.Item3 : of.Item3;
                    var val = sf != null ? sf.Item2.Item1 : of.Item2.Item1;

                    if (val == null)
                        continue; // nothing to write

                    // The special cases here are that the value we're writing might already be a string, or that the
                    // value might be a bool. If the value is the bool 'true' we want to write the attribute name instead
                    // of the string 'true'. If the value is the bool 'false' we don't want to write anything.
                    //
                    // Otherwise the value is another object (perhaps an IHtmlString), and we'll ask it to format itself.
                    string str;
                    var flag = val as bool?;

                    switch (flag)
                    {
                        case true: str = name; break;
                        case false: continue;
                        default: str = val as string; break;
                    }

                    if (first)
                    {
                        WriteLiteralTo(writer, prefix.Item1);
                        first = false;
                    }
                    else
                    {
                        WriteLiteralTo(writer, ws);
                    }

                    // The extra branching here is to ensure that we call the Write*To(string) overload when
                    // possible.
                    if (literal && str != null)
                        WriteLiteralTo(writer, str);
                    else if (literal)
                        WriteLiteralTo(writer, (string)val);
                    else if (str != null)
                        WriteTo(writer, str);
                    else
                        WriteTo(writer, val);

                    wroteSomething = true;
                }

                if (wroteSomething)
                    WriteLiteralTo(writer, suffix.Item1);
            }
        }
    }
}