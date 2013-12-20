using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Web;

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
        ///   Creates a CSVWriter for given writer</summary>
        /// <param name="context">
        ///   HTTP context</param>
        public CsvWriter(TextWriter writer)
        {
            this.writer = writer;
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
        ///   Exports a table (usually generated by Tabularizer) as CSV</summary>
        /// <param name="captions">
        ///   List of column captions</param>
        /// <param name="table">
        ///   List containing data rows</param>
        public void ExportTabularized(ICollection<string> captions, ICollection<string[]> table)
        {
            if (captions != null)
            {
                foreach (var caption in captions)
                    Write(caption);

                WriteLine();
            }

            if (table != null)
                foreach (var line in table)
                {
                    for (var i = 0; i < line.Length; i++)
                        Write(line[i]);

                    WriteLine();
                }
        }
    }
}