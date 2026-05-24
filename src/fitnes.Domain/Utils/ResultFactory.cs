using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using System.Reflection;

namespace fitnes.Domain.Utils;

public static class ResultFactory
{
    public static TResponse Failure<TResponse>(Errors error, IEnumerable<string>? errorsMessages = null)
    {
        var method = typeof(TResponse).GetMethod(nameof(Result.Failure), BindingFlags.Public | BindingFlags.Static, [typeof(Errors), typeof(IEnumerable<string>)]);
        
        if (method == null)
        {
             throw new InvalidOperationException($"Type {typeof(TResponse).Name} must implement static {nameof(Result.Failure)}(Errors, string[]) method.");
        }

        return (TResponse)method.Invoke(null, [error, errorsMessages])!;
    }
}
