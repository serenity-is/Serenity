namespace Serenity.CodeGenerator;

/// <summary>
/// Original code from https://github.com/fjunqueira/hinter
/// </summary>
public class Hinter
{
    public static string ReadHintedLine(IEnumerable<string> hintSource, 
        string inputRegex = ".", string userInput = null)
    {
        Console.OutputEncoding = Encoding.UTF8;
        ConsoleKeyInfo input;

        userInput ??= string.Empty;

        if (userInput.Length > 0)
        {
            Console.Write(userInput);
        }

        string lastUserInput = null;
        string lastSuggestion = null;

        while (ConsoleKey.Enter != (input = Console.ReadKey(true)).Key)
        {
            var oldUserInput = userInput;

            if (input.Key == ConsoleKey.Backspace)
            {
                userInput = userInput.Any() ? userInput.Remove(userInput.Length - 1, 1) : string.Empty;
                lastSuggestion = null;
                lastUserInput = null;
            }
            else if (input.Key == ConsoleKey.Tab)
            {
                lastUserInput ??= userInput;
                var suggestions = hintSource
                    .Where(item => item.Length >= lastUserInput.Length &&
                        string.Compare(item.Substring(0, lastUserInput.Length), lastUserInput, StringComparison.OrdinalIgnoreCase) == 0)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();

                if (suggestions.Any())
                {
                    var idx = lastSuggestion == null ? -1 : suggestions.IndexOf(lastSuggestion);

                    if (idx < 0 || idx == suggestions.Count - 1)
                    {
                        lastSuggestion = suggestions[0];
                    }
                    else
                    {
                        lastSuggestion = suggestions[idx + 1];
                    }

                    
                    userInput = lastSuggestion;                       
                }
                else
                {
                    lastSuggestion = null;
                    lastUserInput = null;
                }
            }
            else if (input.KeyChar != '\0' && Regex.IsMatch(input.KeyChar.ToString(), inputRegex))
                userInput += input.KeyChar;

            var same = 0;
            while (same < userInput.Length && same < oldUserInput.Length && userInput[same] == oldUserInput[same])
                same++;

            for (var i = same; i < oldUserInput.Length; i++)
            {
                Console.Write('\b');
                Console.Write(' ');
                Console.Write('\b');
            }

            for (var i = same; i < userInput.Length; i++)
                Console.Write(userInput[i]);
        }

        Console.WriteLine();

        return userInput;
    }
}