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
        private static bool failedRedirect = false;

        public static string ReadHintedLine<T, TResult>(IEnumerable<T> hintSource, 
            Func<T, TResult> hintField, string inputRegex = ".", ConsoleColor hintColor = ConsoleColor.DarkGray,
            string userInput = null)
        {
            Console.OutputEncoding = Encoding.UTF8;
            ConsoleKeyInfo input;

            var suggestion = string.Empty;
            var readLine = string.Empty;
            userInput = userInput ?? String.Empty;
            CheckRedirect();

            if (userInput.Length > 0)
            {
                readLine = userInput;
                ClearCurrentConsoleLine();
                if (!failedRedirect)
                    Console.Write(userInput);
            }

            Func<string> getSuggestion = () =>
            {
                suggestion = hintSource.Select(item => hintField(item).ToString())
                    .FirstOrDefault(item => item.Length > userInput.Length && item.Substring(0, userInput.Length) == userInput);

                if (suggestion == null)
                    suggestion = hintSource.Select(item => hintField(item).ToString())
                        .FirstOrDefault(item => item.Length > userInput.Length &&
                            String.Compare(item.Substring(0, userInput.Length), userInput, StringComparison.OrdinalIgnoreCase) == 0);

                return suggestion == null ? userInput : suggestion;
            };

            if (failedRedirect)
            {
                userInput = Console.ReadLine();
                readLine = getSuggestion();
                Console.WriteLine(readLine);
                return readLine;
            }

            while (ConsoleKey.Enter != (input = Console.ReadKey()).Key)
            {
                if (input.Key == ConsoleKey.Backspace)
                    userInput = userInput.Any() ? userInput.Remove(userInput.Length - 1, 1) : string.Empty;

                else if (input.Key == ConsoleKey.Tab)
                    userInput = suggestion ?? userInput;

                else if (input != null && input.KeyChar != '\0' && Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                    userInput += input.KeyChar;

                readLine = getSuggestion();

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

        private static void CheckRedirect()
        {
            try
            {
                var y = Console.CursorTop;
                var x = Console.CursorLeft;
                var w = Console.WindowWidth;
                if (w >= 0)
                    Console.SetCursorPosition(x, y);
            }
            catch
            {
                failedRedirect = true;
            }
        }

        private static void ClearCurrentConsoleLine()
        {
            if (failedRedirect)
                return;

            try
            {
                int currentLineCursor = Console.CursorTop;
                Console.SetCursorPosition(0, Console.CursorTop);
                int consoleWidth;
                consoleWidth = Console.WindowWidth;
                Console.Write(new string(' ', consoleWidth));
                Console.SetCursorPosition(0, currentLineCursor);
            }
            catch (Exception)
            {
                failedRedirect = true;
            }
        }
    }
}