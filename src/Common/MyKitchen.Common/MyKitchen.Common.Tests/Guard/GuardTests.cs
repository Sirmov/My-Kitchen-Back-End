// |-----------------------------------------------------------------------------------------------------|
// <copyright file="GuardTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Tests.Guard
{
    using System.Text.RegularExpressions;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.Guard;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="Guard"/> class.
    /// </summary>
    [TestFixture]
    public class GuardTests
    {
        private const string ExceptionShouldBeNullMessage = "Exception should be null. No exception should be returned.";
        private const string ExceptionShouldNotBeNullMessage = "Exception should not be null.";
        private const string ExceptionMessageIsNotCorrect = "Exception message is not correct.";

        private const string ExceptionIsNotCorrectType = "Exception is not of correct type: {0}.";

        /// <summary>
        /// This test checks whether <see cref="Guard"/> throws an exception when an type without a string constructor
        /// is requested.
        /// </summary>
        [Test]
        public void Guard_NoStringConstructor_ThrowsInvalidOperationException()
        {
            // Arrange
            string exceptionMessage = string.Format(ExceptionMessages.TypeDoesNotHaveStringConstructor, nameof(Int32));
            Guard guard = new Guard();

            try
            {
                // Act
                guard.AgainstNull<int>(null, string.Empty);
            }
            catch (Exception exception)
            {
                // Assert
                Assert.That(exception, Is.TypeOf<InvalidOperationException>(), string.Format(ExceptionIsNotCorrectType, nameof(InvalidOperationException)));
                Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
                return;
            }

            Assert.Fail(ExceptionShouldNotBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNull{TOut}(object?, string, object?[])"/>
        /// returns an exception with the correct exception message when the argument is <see langword="null"/>.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstNull_ArgumentsIsNull_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNull<Exception>(null, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNull{TOut}(object?, string, object?[])"/>
        /// returns <see langword="null"/> when the argument is not <see langword="null"/>.
        /// </summary>
        /// <param name="argument">The argument which shouldn't <see langword="null"/>.</param>
        [Test]
        [TestCase(2)]
        public void AgainstNull_ArgumentIsNotNull_ReturnsNull(object argument)
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNull<Exception>(argument, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstTrue{TOut}(bool, string, object?[]))"/>
        /// returns an exception with the correct exception message when the argument is <see langword="true"/>.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstTrue_BooleanIsTrue_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstTrue<Exception>(true, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstTrue{TOut}(bool, string, object?[]))"/>
        /// returns <see langword="null"/> when the argument is <see langword="false"/>.
        /// </summary>
        [Test]
        public void AgainstTrue_BooleanIsFalse_ReturnsNull()
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstTrue<Exception>(false, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstFalse{TOut}(bool, string, object?[]))"/>
        /// returns an exception with the correct exception message when the argument is <see langword="false"/>.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstFalse_BooleanIsFalse_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstFalse<Exception>(false, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstFalse{TOut}(bool, string, object?[]))"/>
        /// returns <see langword="null"/> when the <paramref name="boolean"/> is <see langword="true"/>.
        /// </summary>
        [Test]
        public void AgainstFalse_BooleanIsTrue_ReturnsNull()
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstFalse<Exception>(true, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrEmpty{TOut}(string, string, object?[])"/>
        /// returns an exception with the correct exception message when the argument is <see langword="null"/>.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstNullOrEmpty_StringIsNull_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrEmpty<Exception>(null!, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrEmpty{TOut}(string, string, object?[])"/>
        /// returns an exception with the correct exception message when the argument is empty.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstNullOrEmpty_StringIsEmpty_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrEmpty<Exception>(string.Empty, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrEmpty{TOut}(string, string, object?[])"/>
        /// returns <see langword="null"/> when the <paramref name="text"/> is not empty.
        /// </summary>
        /// <param name="text">The non empty string.</param>
        [Test]
        [TestCase("I hope i don't fail")]
        public void AgainstNullOrEmpty_StringIsNotEmpty_ReturnsNull(string text)
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrEmpty<Exception>(text, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrWhiteSpace{TOut}(string, string, object?[])"/>
        /// returns an exception with the correct exception message when the argument is <see langword="null"/>.
        /// </summary>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("{0} exception.", new string[] { "Critical" })]
        public void AgainstNullOrWhiteSpace_StringIsNull_ReturnsExceptionMessage(
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrWhiteSpace<Exception>(null!, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrWhiteSpace{TOut}(string, string, object?[])"/>
        /// returns an exception with the correct exception message when the argument is whitespace.
        /// </summary>
        /// <param name="text">The whitespace text.</params>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("", "{0} exception.", new string[] { "Critical" })]
        [TestCase(" ", "{0} exception.", new string[] { "Critical" })]
        public void AgainstNullOrWhitespace_StringIsWhitespace_ReturnsExceptionMessage(
            string text,
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrWhiteSpace<Exception>(text, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstNullOrWhiteSpace{TOut}(string, string, object?[])"/>
        /// returns <see langword="null"/> when the <paramref name="text"/> is not whitespace.
        /// </summary>
        /// <param name="text">The non whitespace string.</param>
        [Test]
        [TestCase("I hope i don't fail")]
        public void AgainstNullOrWhitespace_StringIsNotWhitespace_ReturnsNull(string text)
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstNullOrWhiteSpace<Exception>(text, string.Empty!);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstRegex{TOut}(string, string, string, object?[])"/>
        /// returns an exception with the correct exception message when the <paramref name="text"/> does not
        /// match the regex <paramref name="pattern"/>.
        /// </summary>
        /// <param name="text">The text to be tested against the regex <paramref name="pattern"/>.</param>
        /// <param name="pattern">The regex pattern to be used to test the <paramref name="text"/>.</param>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("I won't pass ):", @"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b", "{0} exception.", new string[] { "Critical" })]
        public void AgainstRegex_TextIsNotMatchingPattern_ReturnsExceptionMessage(
            string text,
            string pattern,
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstRegex<Exception>(text, pattern, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstRegex{TOut}(string, string, string, object?[])"/>
        /// returns <see langword="null"/> when the <paramref name="text"/> matches the regex <paramref name="pattern"/>.
        /// </summary>
        /// <param name="text">The text to be tested against the regex <paramref name="pattern"/>.</param>
        /// <param name="pattern">The regex pattern to be used to test the <paramref name="text"/>.</param>
        [Test]
        [TestCase("test@mail.com", @"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b")]
        public void AgainstRegex_TextIsMatchingPattern_ReturnsNull(
            string text,
            string pattern)
        {
            // Arrange
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstRegex<Exception>(text, pattern, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstRegex{TOut}(string, RegularExpressions.Regex, string, object?[])"/>
        /// returns an exception with the correct exception message
        /// when the <paramref name="text"/> does not match the regex <paramref name="pattern"/>.
        /// </summary>
        /// <param name="text">The text to be tested against the regex <paramref name="pattern"/>.</param>
        /// <param name="pattern">The regex pattern to be used to test the <paramref name="text"/>.</param>
        /// <param name="exceptionMessageFormat">The string format of the exception message.</param>
        /// <param name="exceptionMessageParams">The params for the exception message.</param>
        [Test]
        [TestCase("I won't pass ):", @"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b", "{0} exception.", new string[] { "Critical" })]
        public void AgainstRegex_TextIsNotMatchingRegex_ReturnsExceptionMessage(
            string text,
            string pattern,
            string exceptionMessageFormat,
            string[]? exceptionMessageParams)
        {
            // Arrange
            string exceptionMessage = string.Format(exceptionMessageFormat, exceptionMessageParams!);
            Regex regex = new Regex(pattern);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstRegex<Exception>(text, regex, exceptionMessageFormat, exceptionMessageParams!);

            // Assert
            Assert.That(exception, Is.Not.Null, ExceptionShouldNotBeNullMessage);
            Assert.That(exception.Message, Is.EqualTo(exceptionMessage), ExceptionMessageIsNotCorrect);
        }

        /// <summary>
        /// This test checks whether <see cref="Guard.AgainstRegex{TOut}(string, RegularExpressions.Regex, string, object?[])"/>
        /// returns <see langword="null"/> when the <paramref name="text"/> matches the regex <paramref name="pattern"/>.
        /// </summary>
        /// <param name="text">The text to be tested against the regex <paramref name="pattern"/>.</param>
        /// <param name="pattern">The regex pattern to be used to test the <paramref name="text"/>.</param>
        [Test]
        [TestCase("test@mail.com", @"\b[\w\.-]+@[\w\.-]+\.\w{2,4}\b")]
        public void AgainstRegex_TextIsMatchingRegex_ReturnsNull(
            string text,
            string pattern)
        {
            // Arrange
            Regex regex = new Regex(pattern);
            Guard guard = new Guard();

            // Act
            var exception = guard.AgainstRegex<Exception>(text, regex, string.Empty);

            // Assert
            Assert.That(exception, Is.Null, ExceptionShouldBeNullMessage);
        }
    }
}
