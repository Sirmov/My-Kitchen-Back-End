// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ResultTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Common.Tests.Result
{
    using MyKitchen.Common.Result;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="Result{TFailure}"/> class.
    /// </summary>
    [TestFixture]
    public class ResultTests
    {
        private const string ResultShouldNotBeNullMessage = "Result should not be null.";
        private const string ResultShouldBeSuccessfulMessage = "Result should be successful.";

        // private const string ResultShouldNotBeSuccessfulMessage = "Result should not be successful.";
        private const string ResultShouldBeFailedMessage = "Result should be failed.";

        // private const string ResultShouldNotBeFailedMessage = "Result should not be failed.";
        private const string ResultFailureMessageShouldBeMessage = "Result failure message should be \"{0}\"";
        private const string ResultFailureShouldBeNullMessage = "Result failure should be null.";
        private const string DependOnShouldReturnFalseMessage = "DependOn should return false.";
        private const string DependOnShouldReturnTrueMessage = "DependOn should return true.";

        /// <summary>
        /// This test checks whether <see cref="Result{TFailure}.Result()"/> creates result.
        /// </summary>
        [Test]
        public void Result_EmptyConstructor_CreatesResult()
        {
            // Arrange
            // Act
            var result = new Result<Exception>();

            // Assert
            Assert.That(result, Is.Not.Null, ResultShouldNotBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Result{TFailure}.Result()"/> creates a successful result.
        /// </summary>
        [Test]
        public void Result_EmptyConstructor_CreatesSuccessfulResult()
        {
            // Arrange
            // Act
            var result = new Result<Exception>();

            // Assert
            Assert.That(result.IsSuccessful, Is.True, ResultShouldBeSuccessfulMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Result{TFailure}.Result(TFailure)"/> creates a result.
        /// </summary>
        [Test]
        public void Result_TFailureConstructor_CreatesResult()
        {
            // Arrange
            // Act
            var result = new Result<Exception>(new Exception());

            // Assert
            Assert.That(result, Is.Not.Null, ResultShouldNotBeNullMessage);
        }

        /// <summary>
        /// This test checks whether <see cref="Result{TFailure}.Result(TFailure)"/> creates a failed result.
        /// </summary>
        [Test]
        public void Result_TFailureConstructor_CreatesFailedResult()
        {
            // Arrange
            const string exceptionMessage = "I failed the test.";
            Exception exception = new (exceptionMessage);

            // Act
            var result = new Result<Exception>(exception);

            // Assert
            Assert.That(result.IsFailed, Is.True, ResultShouldBeFailedMessage);
        }

        /// <summary>
        /// This test checks whether the <see cref="Result{TFailure}.Result(TFailure)"/> sets
        /// the correct exception to the <see cref="Result{TFailure}.Failure"/> property.
        /// </summary>
        /// <param name="exceptionMessage">The exception message used for creating the exception used to test the setter.</param>
        [Test]
        [TestCase("I failed the test.")]
        public void Result_TFailureConstructor_SetsCorrectValue(string exceptionMessage)
        {
            // Arrange
            Exception exception = new (exceptionMessage);

            // Act
            var result = new Result<Exception>(exception);

            // Assert
            Assert.That(result.Failure!.Message, Is.EqualTo(exceptionMessage), string.Format(ResultFailureMessageShouldBeMessage, exceptionMessage));
        }

        /// <summary>
        /// This test checks whether setting the <see cref="Result{TFailure}.Failure"/> to a
        /// not <see langword="null"/> value changes the <see cref="Result{TFailure}.IsFailed"/>
        /// flag to <see langword="true"/>.
        /// </summary>
        [Test]
        public void Result_SetFailure_FailsResult()
        {
            // Arrange
            Exception exception = new ();

            var result = new Result<Exception>
            {
                // Act
                Failure = exception,
            };

            // Assert
            Assert.That(result.IsFailed, Is.True, ResultShouldBeFailedMessage);
        }

        // TODO: Succeed, Failed
        public void Result_FailureIsNull_

        /// <summary>
        /// This test checks whether the <see cref="Result{TFailure}.DependOn(Common.Result.Contracts.IResult{TFailure})"/>
        /// sets the <see cref="Result{TFailure}.Failure"/> to the failure of the failed dependency result.
        /// </summary>
        /// <param name="exceptionMessage">The exception message used for creating the exception used to test the setter.</param>
        [Test]
        [TestCase("I hope i get transferred.")]
        public void DependOn_DependencyResultFailed_DependencyFailureIsSet(string exceptionMessage)
        {
            // Arrange
            var failedResult = new Result<Exception>(new Exception(exceptionMessage));
            var result = new Result<Exception>();

            // Act
            result.DependOn(failedResult);

            // Assert
            Assert.That(result.Failure!.Message, Is.EqualTo(exceptionMessage), string.Format(ResultFailureMessageShouldBeMessage, exceptionMessage));
        }

        /// <summary>
        /// This test checks whether the <see cref="Result{TFailure}.DependOn(Common.Result.Contracts.IResult{TFailure})"/>
        /// doesn't change the result failure if the dependency result is successful.
        /// </summary>
        [Test]
        public void DependOn_DependencyResultSuccessful_ResultFailureNotChanged()
        {
            // Arrange
            var successfulResult = new Result<Exception>();
            var result = new Result<Exception>();

            // Act
            result.DependOn(successfulResult);

            // Assert
            Assert.That(result.Failure, Is.Null, ResultFailureShouldBeNullMessage);
        }

        /// <summary>
        /// This test checks whether the <see cref="Result{TFailure}.DependOn(Common.Result.Contracts.IResult{TFailure})"/>
        /// returns false when the dependency result is failed.
        /// </summary>
        [Test]
        public void DependOn_DependencyResultFailed_ReturnsFalse()
        {
            // Arrange
            var failedResult = new Result<Exception>(new Exception());
            var result = new Result<Exception>();

            // Act
            var isSuccessful = result.DependOn(failedResult);

            // Assert
            Assert.That(isSuccessful, Is.False, DependOnShouldReturnFalseMessage);
        }

        /// <summary>
        /// This test checks whether the <see cref="Result{TFailure}.DependOn(Common.Result.Contracts.IResult{TFailure})"/>
        /// returns true when the dependency result is successful.
        /// </summary>
        [Test]
        public void DependOn_DependencyResultFailed_ReturnsTrue()
        {
            // Arrange
            var successfulResult = new Result<Exception>();
            var result = new Result<Exception>();

            // Act
            var isSuccessful = result.DependOn(successfulResult);

            // Assert
            Assert.That(isSuccessful, Is.True, DependOnShouldReturnTrueMessage);
        }
    }
}
