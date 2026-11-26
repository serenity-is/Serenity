namespace Serenity.Services;

/// <summary>
/// Parses and evaluates logical permission expressions containing <c>!</c>, <c>&amp;</c>, <c>|</c>, and parentheses.
/// </summary>
public static class PermissionExpressionParser
{
    enum TokenType
    {
        Permission,
        Parenthesis,
        Operator,
        WhiteSpace
    };

    private static TokenType DetermineType(char ch)
    {
        if (ch == '!' || ch == '&' || ch == '|')
            return TokenType.Operator;

        if (ch == '(' || ch == ')')
            return TokenType.Parenthesis;

        if (char.IsWhiteSpace(ch))
            return TokenType.WhiteSpace;

        return TokenType.Permission;
    }

    private static readonly char[] expressionChars = ['|', '&', '!', '(', ')'];
    private static readonly ConcurrentDictionary<string, string[]> rpnCache = new();

    /// <summary>
    /// Returns true if the permission is a logical expression, e.g. contains <c>!</c>, <c>&amp;</c>,
    /// <c>|</c> or parentheses, rather than a single permission key.
    /// </summary>
    /// <param name="permission">Permission or permission expression</param>
    public static bool IsExpression(string? permission)
    {
        return !string.IsNullOrEmpty(permission) && permission.IndexOfAny(expressionChars) >= 0;
    }

    /// <summary>
    /// Checks a permission that may be a logical expression. A single permission key is passed to
    /// <paramref name="hasPermissionKey"/> as is, while for an expression, each permission key in it
    /// is passed separately and the results are combined with the operators.
    /// </summary>
    /// <param name="permission">Permission or permission expression</param>
    /// <param name="hasPermissionKey">Checks a single permission key</param>
    /// <exception cref="InvalidOperationException">The expression is malformed, e.g. "A|" or "(A",
    /// which is usually a typo in an attribute or setting.</exception>
    public static bool HasPermission(string permission, Func<string, bool> hasPermissionKey)
    {
        if (hasPermissionKey is null)
            throw new ArgumentNullException(nameof(hasPermissionKey));

        if (!IsExpression(permission))
            return hasPermissionKey(permission);

        try
        {
            if (!rpnCache.TryGetValue(permission, out var rpnTokens))
                rpnCache[permission] = rpnTokens = [.. ShuntingYard(Tokenize(permission))];

            return Evaluate(rpnTokens, hasPermissionKey);
        }
        catch (InvalidOperationException ex)
        {
            // e.g. a missing operand pops from an empty stack, which says nothing about the expression
            throw new InvalidOperationException($"Invalid permission expression: '{permission}'", ex);
        }
    }

    /// <summary>
    /// Tokenizes a permission expression into individual operators, parentheses, and permission keys.
    /// </summary>
    /// <param name="expression">The permission expression to tokenize.</param>
    /// <returns>An enumerable of tokens.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <c>null</c>.</exception>
    public static IEnumerable<string> Tokenize(string expression)
    {
        if (expression is null)
            throw new ArgumentNullException(nameof(expression));

        var token = new StringBuilder();

        for (var index = 0; index < expression.Length; index++)
        {
            var ch = expression[index];

            var type = DetermineType(ch);
            if (type == TokenType.WhiteSpace)
                continue;

            token.Append(ch);

            if (index >= expression.Length - 1 ||
                type != TokenType.Permission ||
                DetermineType(expression[index + 1]) != TokenType.Permission)
            {
                yield return token.ToString();
                token.Clear();
            }
        }
    }

    static readonly string[] operators = ["!", "&", "|"];
    const string openParen = "(";
    const string closeParen = ")";

    /// <summary>
    /// Converts tokens to Reverse Polish Notation using the shunting-yard algorithm.
    /// </summary>
    /// <param name="tokens">The tokens produced by <see cref="Tokenize"/>.</param>
    /// <returns>Tokens in Reverse Polish Notation.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="tokens"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">The expression contains mismatched parentheses.</exception>
    public static IEnumerable<string> ShuntingYard(IEnumerable<string> tokens)
    {
        if (tokens == null)
            throw new ArgumentNullException(nameof(tokens));

        var stack = new Stack<string>();
        foreach (var token in tokens)
        {
            if (token.Length == 1)
            {
                var precedence = Array.IndexOf(operators, token);

                if (precedence >= 0)
                {
                    while (stack.Count > 0)
                    {
                        var prevPrecedence = Array.IndexOf(operators, stack.Peek());
                        if (prevPrecedence < 0 || prevPrecedence > precedence)
                            break;

                        yield return stack.Pop();
                    }

                    stack.Push(token);
                }
                else if (token == openParen)
                {
                    stack.Push(token);
                }
                else if (token == closeParen)
                {
                    while (stack.Peek() != "(")
                        yield return stack.Pop();

                    stack.Pop();
                }
                else
                    yield return token;
            }
            else
                yield return token;
        }

        while (stack.Count > 0)
        {
            var tok = stack.Pop();

            if (tok == openParen || tok == closeParen)
                throw new InvalidOperationException("Mismatched parentheses in permission expression!");

            yield return tok;
        }
    }

    /// <summary>
    /// Evaluates tokens in Reverse Polish Notation produced by <see cref="ShuntingYard"/>.
    /// </summary>
    /// <param name="rpnTokens">The tokens in Reverse Polish Notation.</param>
    /// <param name="hasPermission">A function that returns <c>true</c> if the user has the specified permission key.</param>
    /// <returns><c>true</c> if the expression evaluates to granted; otherwise <c>false</c>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="rpnTokens"/> or <paramref name="hasPermission"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">The expression is malformed and cannot be evaluated.</exception>
    public static bool Evaluate(IEnumerable<string> rpnTokens, Func<string, bool> hasPermission)
    {
        if (rpnTokens is null)
            throw new ArgumentNullException(nameof(rpnTokens));

        if (hasPermission == null)
            throw new ArgumentNullException(nameof(hasPermission));

        var stack = new Stack<bool>();

        foreach (var token in rpnTokens)
        {
            switch (token)
            {
                case "!":
                    {
                        stack.Push(!stack.Pop());
                        break;
                    }
                case "&":
                    {
                        stack.Push(stack.Pop() & stack.Pop());
                        break;
                    }
                case "|":
                    {
                        stack.Push(stack.Pop() | stack.Pop());
                        break;
                    }
                default:
                    stack.Push(hasPermission(token));
                    break;
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Error evaluating permission expression!");

        return stack.Pop();
    }
}