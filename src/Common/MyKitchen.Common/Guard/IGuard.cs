namespace MyKitchen.Common.Guard
{
    using System.Text.RegularExpressions;

    public interface IGuard
    {
        public TOut? AgainstNull<TOut>(object? variable, string formatString, params object?[] args);

        public TOut? AgainstTrue<TOut>(bool boolean, string formatString, params object?[] args);

        public TOut? AgainstFalse<TOut>(bool boolean, string formatString, params object?[] args);

        public TOut? AgainstRegex<TOut>(string text, Regex regex, string formatString, params object?[] args);

        public TOut? AgainstRegex<TOut>(string text, string pattern, string formatString, params object?[] args);

        public TOut? AgainstNullOrEmpty<TOut>(string text, string formatString, params object?[] args);

        public TOut? AgainstNullOrWhiteSpace<TOut>(string text, string formatString, params object?[] args);
    }
}
