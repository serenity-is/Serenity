using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

namespace Serenity.CodeGenerator
{
    /// <summary>
    /// Original code from https://github.com/fjunqueira/hinter
    /// </summary>
    public class Hinter
    {
        public static string ReadHintedLine<T, TResult>(IEnumerable<T> hintSource, 
            Func<T, TResult> hintField, string inputRegex = ".", ConsoleColor hintColor = ConsoleColor.DarkGray,
            string userInput = null)
        {
            Console.OutputEncoding = Encoding.UTF8;
            ConsoleKeyInfo input;

            var suggestion = string.Empty;
            var readLine = string.Empty;
            userInput = userInput ?? String.Empty;
            if (userInput.Length > 0)
            {
                readLine = userInput;
                ClearCurrentConsoleLine();
                Console.Write(userInput);
            }

            while (ConsoleKey.Enter != (input = Console.ReadKey()).Key)
            {
                if (input.Key == ConsoleKey.Backspace)
                    userInput = userInput.Any() ? userInput.Remove(userInput.Length - 1, 1) : string.Empty;

                else if (input.Key == ConsoleKey.Tab)
                    userInput = suggestion ?? userInput;

                else if (input != null && input.KeyChar != '\0' && Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                    userInput += input.KeyChar;

                suggestion = hintSource.Select(item => hintField(item).ToString())
                    .FirstOrDefault(item => item.Length > userInput.Length && item.Substring(0, userInput.Length) == userInput);

                if (suggestion == null)
                    suggestion = hintSource.Select(item => hintField(item).ToString())
                        .FirstOrDefault(item => item.Length > userInput.Length && 
                            String.Compare(item.Substring(0, userInput.Length), userInput, StringComparison.OrdinalIgnoreCase) == 0);

                readLine = suggestion == null ? userInput : suggestion;

                ClearCurrentConsoleLine();

                Console.Write(userInput);

                var originalColor = Console.ForegroundColor;

                Console.ForegroundColor = hintColor;

                if (userInput.Any())
                    Console.Write(readLine.Substring(userInput.Length, readLine.Length - userInput.Length));

                Console.SetCursorPosition(userInput.Length, Console.CursorTop);
                Console.ForegroundColor = originalColor;
            }

            Console.WriteLine(readLine);

            return readLine;
        }

        private static void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            int consoleWidth;
            try
            {
                consoleWidth = Console.WindowWidth;
            }
            catch (Exception)
            {
                consoleWidth = 80;
            }
            Console.Write(new string(' ', consoleWidth));
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}