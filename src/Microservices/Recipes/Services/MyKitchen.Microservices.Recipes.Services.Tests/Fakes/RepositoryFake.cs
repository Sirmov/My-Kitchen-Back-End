// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RepositoryFake.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests.Fakes
{
    using MongoDB.Bson;

    using Moq;

    using MyKitchen.Common.Result;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models.Common;
    using MyKitchen.Microservices.Recipes.Services.Tests.Fakes.Contracts;

    /// <summary>
    /// This class is a fake of <see cref="IRepository{TDocument, TKey}"/>.
    /// </summary>
    /// <typeparam name="TData">The type of the data stored.</typeparam>
    /// <typeparam name="TKey">The type of the id of the data.</typeparam>
    public class RepositoryFake<TData, TKey> : IFake<IRepository<TData, TKey>>
        where TData : BaseDocument<TKey>
        where TKey : notnull
    {
        private readonly List<TData> data;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFake{TData, TKey}"/> class.
        /// </summary>
        public RepositoryFake()
            : this(new List<TData>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFake{TData, TKey}"/> class.
        /// </summary>
        /// <param name="data">The data collection used for seeding.</param>
        public RepositoryFake(List<TData> data)
        {
            this.data = data;

            this.Mock = new Mock<IRepository<TData, TKey>>();
            this.SetupBehavior(this.Mock);
            this.Instance = this.Mock.Object;
        }

        /// <inheritdoc/>
        public IRepository<TData, TKey> Instance { get; }

        /// <inheritdoc/>
        public Mock<IRepository<TData, TKey>> Mock { get; }

        /// <inheritdoc/>
        public void SetupBehavior(Mock<IRepository<TData, TKey>> mock)
        {
            mock.Setup(x => x.All(It.IsAny<bool>()))
                .Returns((bool withDeleted) =>
                {
                    var items = withDeleted ? this.data : this.data.Where(x => x.IsDeleted == false);

                    return new FakeFindFluent<TData, TData>(items);
                });

            mock.Setup(x => x.FindAsync(It.IsAny<TKey>(), It.IsAny<bool>()))
                .ReturnsAsync((TKey id, bool withDeleted) =>
                {
                    var items = withDeleted ? this.data : this.data.Where(x => x.IsDeleted == false);
                    var item = items.FirstOrDefault(x => x.Id?.Equals(id) ?? false);

                    return item is null ? new NullReferenceException() : item;
                });

            mock.Setup(x => x.AddAsync(It.IsAny<TData>()))
                .ReturnsAsync((TData item) =>
                {
                    this.data.Add(item);
                    item.CreatedOn = DateTime.UtcNow;
                    item.Id = default!;
                    return item;
                });

            mock.Setup(x => x.UpdateAsync(It.IsAny<TKey>(), It.IsAny<TData>()))
                .ReturnsAsync((TKey id, TData item) =>
                {
                    var index = this.data.FindIndex(x => x.Id.Equals(id));
                    this.data[index] = item;
                    return Result<Exception>.Success;
                });

            mock.Setup(x => x.DeleteAsync(It.IsAny<TKey>()))
                .ReturnsAsync((TKey id) =>
                {
                    this.data.RemoveAt(this.data.FindIndex(x => x.Id.Equals(id)));
                    return Result<Exception>.Success;
                });
        }
    }
}
