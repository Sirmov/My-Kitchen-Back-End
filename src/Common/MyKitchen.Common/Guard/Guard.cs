namespace MyKitchen.Common.Guard
{
    using System.Text.RegularExpressions;

    public class Guard : IGuard
    {
        public TOut? AgainstNull<TOut>(object? variable, string formatString, params object?[] args)
        {
            if (variable == null)
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstTrue<TOut>(bool boolean, string formatString, params object?[] args)
        {
            if (boolean == true)
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstFalse<TOut>(bool boolean, string formatString, params object?[] args)
        {
            if (boolean == false)
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstNullOrEmpty<TOut>(string text, string formatString, params object?[] args)
        {
            if (string.IsNullOrEmpty(text))
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstNullOrWhiteSpace<TOut>(string text, string formatString, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstRegex<TOut>(string text, Regex regex, string formatString, params object?[] args)
        {
            if (regex.IsMatch(text) == false)
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        public TOut? AgainstRegex<TOut>(string text, string pattern, string formatString, params object?[] args)
        {
            if (Regex.IsMatch(text, pattern) == false)
            {
                return this.GenereteReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        private TOut GenereteReturnObject<TOut>(string message)
        {
            var constructor = typeof(TOut).GetConstructor([typeof(string)]);

            return constructor == null
                ? throw new InvalidOperationException($"Type {typeof(TOut)} does not have a constructor that takes a string.")
                : (TOut)constructor.Invoke([message]);
        }
    }
}
