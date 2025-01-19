// |-----------------------------------------------------------------------------------------------------|
// <copyright file="IFake.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests.Fakes.Contracts
{
    using Moq;

    /// <summary>
    /// This interface defines the functionalities of a fake class.
    /// </summary>
    /// <typeparam name="TFake">The type of the faked class.</>
    public interface IFake<TFake>
        where TFake : class
    {
        /// <summary>
        /// Gets the fake instance of the faked class <typeparamref name="TFake"/>.
        /// </summary>
        public TFake Instance { get; }

        /// <summary>
        /// Gets the fake mock of the faked class <typeparamref name="TFake"/>.
        /// </summary>
        public Mock<TFake> Mock { get; }

        /// <summary>
        /// This method sets up the behavior of the fake class.
        /// </summary>
        /// <param name="mock">The <see cref="Mock{TFake}"/> of the class.</param>
        public void SetupBehavior(Mock<TFake> mock);
    }
}
