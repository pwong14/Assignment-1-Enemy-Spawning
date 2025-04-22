using System;
using System.Collections.Generic;

public static class RPNEvaluator
{
    /// <summary>
    /// Evaluates an RPN (Reverse Polish Notation) expression.
    /// Supports +, -, *, /, %, and variables like "wave" and "base".
    /// </summary>
    /// <param name="expression">The RPN expression as a string, e.g. "5 wave +"</param>
    /// <param name="variables">Dictionary of variable names to float values</param>
    /// <returns>The result of the evaluated expression as a float</returns>
    public static float Evaluate(string expression, Dictionary<string, float> variables)
    {
        Stack<float> stack = new Stack<float>();
        string[] tokens = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (variables.ContainsKey(token))
            {
                stack.Push(variables[token]);
            }
            else if (float.TryParse(token, out float number))
            {
                stack.Push(number);
            }
            else
            {
                if (stack.Count < 2)
                    throw new InvalidOperationException($"Not enough operands for operator '{token}'");

                float b = stack.Pop();
                float a = stack.Pop();

                switch (token)
                {
                    case "+": stack.Push(a + b); break;
                    case "-": stack.Push(a - b); break;
                    case "*": stack.Push(a * b); break;
                    case "/": stack.Push(a / b); break;
                    case "%": stack.Push(a % b); break;
                    default: throw new InvalidOperationException($"Unknown operator '{token}'");
                }
            }
        }

        if (stack.Count != 1)
            throw new InvalidOperationException("Invalid RPN expression: stack did not end with exactly one value.");

        return stack.Pop();
    }
}