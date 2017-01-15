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

            while (ConsoleKey.Enter != (input = Console.ReadKey()).Key)
            {
                if (input.Key == ConsoleKey.Backspace)
                    userInput = userInput.Any() ? userInput.Remove(userInput.Length - 1, 1) : string.Empty;

                else if (input.Key == ConsoleKey.Tab)
                    userInput = suggestion ?? userInput;

                else if (input != null && input.KeyChar != '\0' && Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                    userInput += input.KeyChar;

                var oldLine = readLine;
                readLine = getSuggestion();

                var originalColor = Console.ForegroundColor;

                Console.ForegroundColor = hintColor;
                Console.Write('\r');
                for (var i = 0; i < readLine.Length; i++)
                    Console.Write(readLine[i]);

                for (var i = readLine.Length; i < oldLine.Length; i++)
                    Console.Write(' ');

                Console.ForegroundColor = originalColor;
                Console.Write('\r');
                for (var i = 0; i < userInput.Length; i++)
                    Console.Write(userInput[i]);
            }

            Console.WriteLine(readLine);
            return readLine;
        }
    }
}