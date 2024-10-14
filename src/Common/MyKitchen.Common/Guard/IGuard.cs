// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IGuard.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Guard
{
    using System.Text.RegularExpressions;

    /// <summary>
    /// This interface defines the functionality of a guard class.
    /// </summary>
    public interface IGuard
    {
        /// <summary>
        /// This method guards against <see langword="null"/>. When <paramref name="variable"/> is <see langword="null"/> a exception message
        /// is generated using the <paramref name="formatString"/> and the provided <paramref name="args"/>. A new
        /// instance of <typeparamref name="TOut"/> is created using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="variable">The variable to be checked.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstNull<TOut>(object? variable, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against <see langword="true"/>. When <paramref name="boolean"/> is <see langword="true"/> a exception message
        /// is generated using the <paramref name="formatString"/> and the provided <paramref name="args"/>. A new
        /// instance of <typeparamref name="TOut"/> is created using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="boolean">The boolean variable to be checked.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstTrue<TOut>(bool boolean, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against <see langword="false"/>. When <paramref name="boolean"/> is <see langword="false"/> a exception message
        /// is generated using the <paramref name="formatString"/> and the provided <paramref name="args"/>. A new
        /// instance of <typeparamref name="TOut"/> is created using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="boolean">The boolean variable to be checked.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstFalse<TOut>(bool boolean, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against <see cref="Regex"/>. When <paramref name="text"/> does not match
        /// the <paramref name="regex"/> a exception message is generated using the <paramref name="formatString"/>
        /// and the provided <paramref name="args"/>. A new instance of <typeparamref name="TOut"/> is created
        /// using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="text">The string variable to be checked.</param>
        /// <param name="regex">The regex to be used.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstRegex<TOut>(string text, Regex regex, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against a regex <paramref name="pattern"/>. When <paramref name="text"/> does not match
        /// the regex <paramref name="pattern"/> a exception message is generated using the <paramref name="formatString"/>
        /// and the provided <paramref name="args"/>. A new instance of <typeparamref name="TOut"/> is created
        /// using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="text">The string variable to be checked.</param>
        /// <param name="pattern">The regex pattern to be used.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstRegex<TOut>(string text, string pattern, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against a <see langword="null"/> or empty values. When <paramref name="text"/> is <see langword="null"/> or
        /// is empty a exception message is generated using the <paramref name="formatString"/>
        /// and the provided <paramref name="args"/>. A new instance of <typeparamref name="TOut"/> is created
        /// using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="text">The string variable to be checked.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstNullOrEmpty<TOut>(string text, string formatString, params object?[] args);

        /// <summary>
        /// This method guards against a <see langword="null"/> or white space values. When <paramref name="text"/> is <see langword="null"/> or
        /// contains only white space a exception message is generated using the <paramref name="formatString"/>
        /// and the provided <paramref name="args"/>. A new instance of <typeparamref name="TOut"/> is created
        /// using the exception message as a constructor argument.
        /// </summary>
        /// <typeparam name="TOut">
        /// The class containing the exception message to be returned.
        /// Must contain a constructor taking a <see cref="string"/> parameter.
        /// </typeparam>
        /// <param name="text">The string variable to be checked.</param>
        /// <param name="formatString">The format of the exception message.</param>
        /// <param name="args">The arguments of the format string.</param>
        /// <returns>Returns a <typeparamref name="TOut"/> instance containing the exception message.</returns>
        public TOut? AgainstNullOrWhiteSpace<TOut>(string text, string formatString, params object?[] args);
    }
}
