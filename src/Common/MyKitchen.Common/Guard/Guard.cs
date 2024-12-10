// |-----------------------------------------------------------------------------------------------------|
// <copyright file="Guard.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Guard
{
    using System.Text.RegularExpressions;

    using MyKitchen.Common.Constants;

    /// <summary>
    /// This class implements the <see cref="IGuard"/> interface.
    /// </summary>
    public class Guard : IGuard
    {
        /// <inheritdoc/>
        public TOut? AgainstNull<TOut>(object? variable, string formatString, params object?[] args)
        {
            if (variable == null)
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstTrue<TOut>(bool boolean, string formatString, params object?[] args)
        {
            if (boolean == true)
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstFalse<TOut>(bool boolean, string formatString, params object?[] args)
        {
            if (boolean == false)
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstNullOrEmpty<TOut>(string text, string formatString, params object?[] args)
        {
            if (string.IsNullOrEmpty(text))
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstNullOrWhiteSpace<TOut>(string text, string formatString, params object?[] args)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstRegex<TOut>(string text, Regex regex, string formatString, params object?[] args)
        {
            if (regex.IsMatch(text) == false)
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        /// <inheritdoc/>
        public TOut? AgainstRegex<TOut>(string text, string pattern, string formatString, params object?[] args)
        {
            if (Regex.IsMatch(text, pattern) == false)
            {
                return this.GenerateReturnObject<TOut>(string.Format(formatString, args));
            }

            return default;
        }

        private TOut GenerateReturnObject<TOut>(string message)
        {
            var constructor = typeof(TOut).GetConstructor([typeof(string)]);

            return constructor == null
                ? throw new InvalidOperationException(string.Format(ExceptionMessages.TypeDoesNotHaveStringConstructor, typeof(TOut).Name))
                : (TOut)constructor.Invoke([message]);
        }
    }
}
