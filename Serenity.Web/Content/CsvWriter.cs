using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using Serenity.Data;

namespace Serenity.Web
{
    /// <summary>
    ///   Helper class to write CSV files to HTTP context or another stream</summary>
    public class CsvWriter : IDisposable
    {
        private HttpContext context;
        private TextWriter writer;
        private bool startOfLine;

        /// <summary>
        ///   Creates a CSVWriter for given context</summary>
        /// <param name="context">
        ///   HTTP context</param>
        public CsvWriter(HttpContext context)
        {
            InitializeResponse(context);
            this.writer = context.Response.Output;
            this.context = context;
            this.startOfLine = true;
        }

        /// <summary>
        ///   Closes the response</summary>
        public void Dispose()
        {
            if (context != null)
                context.Response.End();
        }

        /// <summary>
        ///   Writes a list of values, converting all to string</summary>
        /// <param name="values">
        ///   List of values</param>
        public void Write(params object[] values)
        {
            foreach (var value in values)
            {
                if (!startOfLine)
                    writer.Write(";");
                else
                    startOfLine = false;
                WriteItem(writer, value, false);
            }
        }

        /// <summary>
        ///   Writes a list of values as a line, converting all to string</summary>
        /// <param name="values">
        ///   List of values</param>
        public void WriteLine(params object[] values)
        {
            Write(values);
            writer.Write("\r\n");
            startOfLine = true;
        }

        /// <summary>
        ///   Initializes current HTTP response for CSV writing</summary>
        /// <param name="context">
        ///   HTTP context</param>
        private static void InitializeResponse(HttpContext context)
        {
            context.Response.Clear();
            context.Response.ContentType = "text/csv";
            context.Response.ContentEncoding = new UTF8Encoding(true);
            context.Response.AppendHeader("content-disposition", "attachment; filename=data.csv");
            context.Response.OutputStream.WriteByte(0xEF);
            context.Response.OutputStream.WriteByte(0xBB);
            context.Response.OutputStream.WriteByte(0xBF);
        }
 
        /// <summary>
        ///   Writes a datatable to response</summary>
        /// <param name="context">
        ///   HTTP context</param>
        /// <param name="table">
        ///   Table</param>
        /// <param name="header">
        ///   True if a header should be written</param>
        /// <param name="quoteall">
        ///    True to quote all variables</param>
        public static void WriteToResponse(HttpContext context, DataTable table, bool header, bool quoteall)
        {
            InitializeResponse(context);
            WriteToStream(context.Response.Output, table, header, quoteall);
            context.Response.End();
        }

        /// <summary>
        ///   Writes a datatable to a string As CSV</summary>
        /// <param name="table">
        ///   Table</param>
        /// <param name="header">
        ///   True if a header should be written</param>
        /// <param name="quoteall">
        ///   True to quote all variables</param>
        /// <returns>
        ///   String containing CSV data</returns>
        public static string WriteToString(DataTable table, bool header, bool quoteall)
        {
            StringWriter writer = new StringWriter();
            WriteToStream(writer, table, header, quoteall);
            return writer.ToString();
        }

        /// <summary>
        ///   Writes a datatable to a stream as CSV</summary>
        /// <param name="stream">
        ///   Stream</param>
        /// <param name="table">
        ///   Table</param>
        /// <param name="header">
        ///   True if a header should be written</param>
        /// <param name="quoteall">
        ///   True to quote all variables</param>
        public static void WriteToStream(TextWriter stream, DataTable table, bool header, bool quoteall)
        {
            if (header)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, table.Columns[i].Caption, quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(';');
                    else
                        stream.Write("\r\n");
                }
            }
            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    WriteItem(stream, row[i], quoteall);
                    if (i < table.Columns.Count - 1)
                        stream.Write(';');
                    else
                        stream.Write("\r\n");
                }
            }
        }

        /// <summary>
        ///   Writes a single item as CSV data</summary>
        /// <param name="stream">
        ///   Text stream to write into</param>
        /// <param name="item">
        ///   Item to be written</param>
        /// <param name="quoteall">
        ///   True to force quoting, even if value has no quotes or commas in it</param>
        public static void WriteItem(TextWriter stream, object item, bool quoteall)
        {
            if (item == null)
                return;
            string s = item.ToString();
            if (quoteall || s.IndexOfAny("\";\x0A\x0D".ToCharArray()) > -1)
                stream.Write("\"" + s.Replace("\"", "\"\"") + "\"");
            else
                stream.Write(s);
        }

        /// <summary>
        ///   Exports a list of dictionaries as CSV</summary>
        /// <param name="list">
        ///   List containing dictionaries</param>
        /// <param name="columns">
        ///   Columns</param>
        /// <param name="columnCaptions">
        ///   Dictionary<string, object>() with column captions (name -> caption)</param>
        /// <param name="handleValue">
        ///   Optional delegate that will be called for each (column, value) pair to format values</param>
        public void ExportDictionaryList(IEnumerable list, IEnumerable<string> columns, IDictionary<string, string> columnCaptions,
            Func<string, object, object> handleValue)
        {
            string c;
            foreach (string s in columns)
            {
                if (columnCaptions == null || !columnCaptions.TryGetValue(s, out c))
                    c = s;

                Write(c);
            }

            WriteLine();

            if (list != null)
                foreach (var o in list)
                {
                    var item = o as IDictionary;
                    if (item != null)
                    {
                        foreach (var col in columns)
                        {
                            if (!item.Contains(col))
                            {
                                Write("");
                                continue;
                            }

                            var v = item[col];

                            if (v == null || Convert.IsDBNull(v))
                            {
                                Write("");
                                continue;
                            }

                            if (handleValue != null)
                                v = handleValue(col, v);

                            if (v is string)
                            {
                                Write(v);
                            }
                            else if (v is DateTime)
                            {
                                var d = (DateTime)v;
                                Write(d.ToString(d.Date == d ? DateHelper.CurrentDateFormat : DateHelper.CurrentDateTimeFormat));
                            }
                            else if (v is Decimal || v is Double)
                            {
                                var m = Convert.ToDecimal(v);
                                Write(m.ToString(",0.00"));
                            }
                            else if (v is Int32 || v is Int64)
                            {
                                var i = Convert.ToInt64(v);
                                Write(i.ToString(",0"));
                            }
                            else
                                Write(v);
                        }

                        WriteLine();
                    }
                }
        }
    }
}
