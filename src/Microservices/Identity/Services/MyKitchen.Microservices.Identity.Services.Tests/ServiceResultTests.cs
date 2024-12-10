// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ServiceResultTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Services.Tests
{
    using System.Net;

    using Microsoft.AspNetCore.Mvc;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Identity.Services.Common.ServiceResult;

    /// <summary>
    /// This test fixture contains unit tests for the service result classes.
    /// </summary>
    [TestFixture]
    public class ServiceResultTests
    {
        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResult(Func{TData, IActionResult})"/>
        /// calls the onSuccess delegate when the result is successful.
        /// </summary>
        [Test]
        public void ToActionResult_DataServiceResultIsSuccessful_CallsOnSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<int>(0);
            bool isCalled = false;

            Func<int, IActionResult> onSuccessDelegate = (int _) =>
            {
                isCalled = true;
                return new EmptyResult();
            };

            // Act
            serviceResult.ToActionResult(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResult(Func{IActionResult})"/>
        /// calls the onSuccess delegate when the result is successful.
        /// </summary>
        [Test]
        public void ToActionResult_ServiceResultIsSuccessful_CallsOnSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult();
            bool isCalled = false;

            Func<IActionResult> onSuccessDelegate = () =>
            {
                isCalled = true;
                return new EmptyResult();
            };

            // Act
            serviceResult.ToActionResult(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResultAsync(Func{TData, Task{IActionResult}})"/>
        /// calls the onSuccess delegate when the result is successful.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task ToActionResultAsync_DataServiceResultIsSuccessful_CallsOnSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult<int>(0);
            bool isCalled = false;

            Func<int, Task<IActionResult>> onSuccessDelegate = async (int _) =>
            {
                isCalled = true;
                return await Task.FromResult(new EmptyResult());
            };

            // Act
            await serviceResult.ToActionResultAsync(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResultAsync(Func{Task{IActionResult}})"/>
        /// calls the onSuccess delegate when the result is successful.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task ToActionResultAsync_ServiceResultIsSuccessful_CallsOnSuccess()
        {
            // Arrange
            var serviceResult = new ServiceResult();
            bool isCalled = false;

            Func<Task<IActionResult>> onSuccessDelegate = async () =>
            {
                isCalled = true;
                return await Task.FromResult(new EmptyResult());
            };

            // Act
            await serviceResult.ToActionResultAsync(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.True);
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResult(Func{TData, IActionResult})"/>
        /// returns a <see cref="ObjectResult"/> with the problem details when the result is failed.
        /// </summary>
        [Test]
        public void ToActionResult_DataServiceResultIsNotSuccessful_ReturnsObjectResultWithProblemDetails()
        {
            // Arrange
            var serviceResult = new ServiceResult<int>(new InternalServerErrorDetails());
            bool isCalled = false;

            Func<int, IActionResult> onSuccessDelegate = (int _) =>
            {
                isCalled = true;
                return new EmptyResult();
            };

            // Act
            var actionResult = serviceResult.ToActionResult(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.False);
            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            var problemDetails = (ProblemDetails)objectResult.Value!;
            Assert.That(problemDetails.Status, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResult(Func{IActionResult})"/>
        /// returns a <see cref="ObjectResult"/> with the problem details when the result is failed.
        /// </summary>
        [Test]
        public void ToActionResult_ServiceResultIsNotSuccessful_ReturnsObjectResultWithProblemDetails()
        {
            // Arrange
            var serviceResult = new ServiceResult(new InternalServerErrorDetails());
            bool isCalled = false;

            Func<IActionResult> onSuccessDelegate = () =>
            {
                isCalled = true;
                return new EmptyResult();
            };

            // Act
            var actionResult = serviceResult.ToActionResult(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.False);
            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            var problemDetails = (ProblemDetails)objectResult.Value!;
            Assert.That(problemDetails.Status, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResultAsync(Func{TData, Task{IActionResult}})"/>
        /// returns a <see cref="ObjectResult"/> with the problem details when the result is failed.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task ToActionResultAsync_DataServiceResultIsNotSuccessful_ReturnsObjectResultWithProblemDetails()
        {
            // Arrange
            var serviceResult = new ServiceResult<int>(new InternalServerErrorDetails());
            bool isCalled = false;

            Func<int, Task<IActionResult>> onSuccessDelegate = async (int _) =>
            {
                isCalled = true;
                return await Task.FromResult(new EmptyResult());
            };

            // Act
            var actionResult = await serviceResult.ToActionResultAsync(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.False);
            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            var problemDetails = (ProblemDetails)objectResult.Value!;
            Assert.That(problemDetails.Status, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }

        /// <summary>
        /// This test checks whether <see cref="ServiceResultBase{TData}.ToActionResultAsync(Func{Task{IActionResult}})"/>
        /// returns a <see cref="ObjectResult"/> with the problem details when the result is failed.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task ToActionResultAsync_ServiceResultIsNotSuccessful_ReturnsObjectResultWithProblemDetails()
        {
            // Arrange
            var serviceResult = new ServiceResult(new InternalServerErrorDetails());
            bool isCalled = false;

            Func<Task<IActionResult>> onSuccessDelegate = async () =>
            {
                isCalled = true;
                return await Task.FromResult(new EmptyResult());
            };

            // Act
            var actionResult = await serviceResult.ToActionResultAsync(onSuccessDelegate);

            // Assert
            Assert.That(isCalled, Is.False);
            Assert.That(actionResult, Is.TypeOf<ObjectResult>());
            var objectResult = (ObjectResult)actionResult;
            var problemDetails = (ProblemDetails)objectResult.Value!;
            Assert.That(problemDetails.Status, Is.EqualTo((int)HttpStatusCode.InternalServerError));
        }
    }
}
