// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipesServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests
{
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;

    using MongoDB.Bson;
    using MongoDB.Driver;

    using Moq;

    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Common.Result.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Contracts;
    using MyKitchen.Microservices.Recipes.Data.Models;
    using MyKitchen.Microservices.Recipes.Services.Mapping;
    using MyKitchen.Microservices.Recipes.Services.Recipes;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;
    using MyKitchen.Microservices.Recipes.Services.Tests.Fakes;
    using MyKitchen.Microservices.Recipes.Services.Tests.Fakes.Contracts;

    /// <summary>
    /// This test fixture contains unit tests for the <see cref="RecipesService"/> class.
    /// </summary>
    public class RecipesServiceTests
    {
        private readonly List<Recipe> recipes = new List<Recipe>()
        {
            new Recipe()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ImageId = ObjectId.GenerateNewId().ToString(),
                Title = "Ice cream",
            },
            new Recipe()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                ImageId = ObjectId.GenerateNewId().ToString(),
                Title = "Mayonnaise",
            },
        };

        private IMapper mapper = null!;
        private IFake<IRepository<Recipe, string>> recipeRepositoryFake = null!;
        private Mock<IRecipeImagesService> recipeImagesServiceMock = null!;
        private RecipesService recipesService = null!;

        /// <summary>
        /// This method is called before running every test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            AutoMapperConfig.RegisterMappings(typeof(RecipeDto).Assembly);
            this.mapper = AutoMapperConfig.MapperInstance;

            this.recipeRepositoryFake = new RepositoryFake<Recipe, string>(this.recipes);

            this.recipeImagesServiceMock = new Mock<IRecipeImagesService>();

            this.recipeImagesServiceMock.Setup(x => x.UploadRecipeImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(ObjectId.GenerateNewId().ToString());

            this.recipesService = new RecipesService(
                this.recipeImagesServiceMock.Object,
                this.recipeRepositoryFake.Instance,
                this.mapper);
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetAllRecipesAsync(Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns an empty collection when recipes exist.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetAllRecipesAsync_CollectionEmpty_ReturnsEmptyCollection()
        {
            // Arrange
            this.recipeRepositoryFake.Mock.Setup(x => x.All(It.IsAny<bool>())).Returns(new FakeFindFluent<Recipe, Recipe>([]));

            // Act
            var getResult = await this.recipesService.GetAllRecipesAsync();

            // Assert
            this.AssertResultIsSuccessful(getResult);
            Assert.That(getResult.Data, Is.Empty);
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetAllRecipesAsync(Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns all recipes as dtos.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetAllRecipesAsync_CollectionNotEmpty_ReturnsAllRecipesAsDtosAsync()
        {
            // Arrange
            // Act
            var getResult = await this.recipesService.GetAllRecipesAsync();

            // Assert
            this.AssertResultIsSuccessful(getResult);

            var recipeDtos = this.recipes.Select(this.mapper.Map<RecipeDto>).ToArray();

            Assert.That(getResult.Data, Is.EqualTo(recipeDtos)
                .Using<RecipeDto>((x, y) => x.Id == y.Id));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetRecipeAsync(string, Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns <see cref="BadRequestDetails"/> when id is not in correct format.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetRecipeAsync_IncorrectIdFormat_ReturnsBadRequestDetails()
        {
            // Arrange
            string id = "I'm surely not correct.";

            // Act
            var getResult = await this.recipesService.GetRecipeAsync(id);

            // Assert
            this.AssertResultIsFailed(getResult, typeof(BadRequestDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetRecipeAsync(string, Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns <see cref="NotFoundDetails"/> when no recipe with provided id exists.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetRecipeAsync_NoRecipeWithId_ReturnsNotFoundDetailsAsync()
        {
            // Arrange
            string id = ObjectId.GenerateNewId().ToString();

            // Act
            var getResult = await this.recipesService.GetRecipeAsync(id);

            // Assert
            this.AssertResultIsFailed(getResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetRecipeAsync(string, Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns the correct recipe as dto.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetRecipeAsync_RecipeWithIdExists_ReturnsRecipeDto()
        {
            // Arrange
            var recipe = this.recipes [0];

            // Act
            var getResult = await this.recipesService.GetRecipeAsync(recipe.Id);

            // Assert
            this.AssertResultIsSuccessful(getResult);
            Assert.That(getResult.Data!.Id, Is.EqualTo(recipe.Id));
            Assert.That(getResult.Data.Title, Is.EqualTo(recipe.Title));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.CreateRecipeAsync(string, RecipeInputDto)"/>
        /// returns <see cref="BadRequestDetails"/> when the recipe dto state is not valid. It checks the minimum
        /// length constraint of the text properties.
        /// </summary>
        /// <param name="title">The title of the recipe.</param>
        /// <param name="description">The description of the recipe.</param>
        /// <param name="ingredients">The ingredients of the recipe.</param>
        /// <param name="directions">The directions of the recipe.</param>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("Ic", "Delicious ice coffee", "Ice, water coffee, milk", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious", "Ice, water coffee, milk", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious ice coffee", "Ice,", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious ice coffee", "Ice, water coffee, milk", "Brew and!")]
        public async Task CreateRecipeAsync_RecipeNotValid_ReturnsBadRequestDetailsAsync(
            string title,
            string description,
            string ingredients,
            string directions)
        {
            // Arrange
            const string userId = "678e88d590605d3dd639a0a6";

            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = userId,
                Title = title,
                Description = description,
                Ingredients = ingredients,
                Directions = directions,
            };

            // Act
            var createResult = await this.recipesService.CreateRecipeAsync(userId, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(createResult, typeof(BadRequestDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.CreateRecipeAsync(string, RecipeInputDto)"/>
        /// returns <see cref="UnauthorizedDetails"/> when a user tries to create a recipe for another user.
        /// </summary>
        /// <param name="userId">The id of the user making the request.</param>
        /// <param name="ownerId">The id of the user owning the recipe.</param>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("678e8dd45c9980448d2e2a7b", "678e88d590605d3dd639a0a6")]
        public async Task CreateRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetailsAsync(string userId, string ownerId)
        {
            // Arrange
            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = ownerId,
                Title = "Ice coffee",
                Description = "Delicious ice coffee",
                Ingredients = "Ice, water coffee, milk",
                Directions = "Brew and enjoy!",
            };

            // Act
            var createResult = await this.recipesService.CreateRecipeAsync(userId, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(createResult, typeof(UnauthorizedDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.CreateRecipeAsync(string, RecipeInputDto)"/>
        /// in the event of a failure when uploading the recipe image the causing problem details are returned.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task CreateRecipeAsync_RecipeImageUploadingFails_ReturnsImageUploadFailure()
        {
            // Arrange
            this.recipeImagesServiceMock.Setup(x => x.UploadRecipeImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new NotFoundDetails(string.Empty));

            var ownerId = ObjectId.GenerateNewId().ToString();

            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = ownerId,
                Title = "Ice coffee",
                Description = "Delicious ice coffee",
                Ingredients = "Ice, water coffee, milk",
                Directions = "Brew and enjoy!",
            };

            // Act
            var createResult = await this.recipesService.CreateRecipeAsync(ownerId, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(createResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.CreateRecipeAsync(string, RecipeInputDto)"/>
        /// returns <see cref="InternalServerErrorDetails"/> when the creation fails.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task CreateRecipeAsync_RecipeAddingFails_ReturnsInternalServerErrorDetails()
        {
            // Arrange
            this.recipeRepositoryFake.Mock
                .Setup(x => x.AddAsync(It.IsAny<Recipe>()))
                .ReturnsAsync(new Exception());

            var ownerId = ObjectId.GenerateNewId().ToString();

            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = ownerId,
                Title = "Ice coffee",
                Description = "Delicious ice coffee",
                Ingredients = "Ice, water coffee, milk",
                Directions = "Brew and enjoy!",
            };

            // Act
            var createResult = await this.recipesService.CreateRecipeAsync(ownerId, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(createResult, typeof(InternalServerErrorDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.CreateRecipeAsync(string, RecipeInputDto)"/>
        /// returns a dto of the created recipe.
        /// </summary>
        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [Order(-1)]
        public async Task CreateRecipeAsync_RecipeIsAdded_ReturnsCreatedRecipeDto()
        {
            // Arrange
            var userId = ObjectId.GenerateNewId().ToString();

            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = userId,
                Title = "Ice coffee",
                Description = "Delicious ice coffee",
                Ingredients = "Ice, water coffee, milk",
                Directions = "Brew and enjoy!",
            };

            // Act
            var createResult = await this.recipesService.CreateRecipeAsync(userId, recipeInputDto);

            // Assert
            this.AssertResultIsSuccessful(createResult);

            var recipe = createResult.Data!;

            Assert.That(recipe, Is.Not.Null);
            Assert.That(recipe.CreatedOn, Is.GreaterThan(DateTime.MinValue));
            Assert.That(recipe.IsDeleted, Is.False);
            Assert.That(recipe.ImageId, Has.Length.EqualTo(24));
            Assert.That(recipe.UserId, Is.EqualTo(userId));
            Assert.That(recipe.Title, Is.EqualTo(recipeInputDto.Title));
            Assert.That(recipe.Description, Is.EqualTo(recipeInputDto.Description));
            Assert.That(recipe.Ingredients, Is.EqualTo(recipeInputDto.Ingredients));
            Assert.That(recipe.Directions, Is.EqualTo(recipeInputDto.Directions));
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_IncorrectIdFormat_ReturnsBadRequestDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_NoRecipeWithId_ReturnsNotFoundDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeImageDeletionFails_ReturnsImageUploadFailure()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeImageUploadingFails_ReturnsImageUploadFailure()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeIsAdded_ReturnsCreatedRecipeDto()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeUpdatingFails_ReturnsInternalServerErrorDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task UpdateRecipeAsync_RecipeIsUpdated_ReturnsCreatedRecipeDto()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_IncorrectIdFormat_ReturnsBadRequestDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_NoRecipeWithId_ReturnsNotFoundDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_RecipeImageDeletionFails_ReturnsImageUploadFailure()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_RecipeDeletingFails_ReturnsInternalServerErrorDetails()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        /// <returns>>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public Task DeleteRecipeAsync_RecipeIsDeleted_ReturnsSuccessfullResult()
        {
            Assert.Pass();
            return Task.CompletedTask;
        }

        private void AssertResultIsSuccessful<TFailure>(IResult<TFailure> result)
            where TFailure : class
        {
            Assert.That(result.IsSuccessful, Is.True, "Result should be successful.");
            Assert.That(result.IsFailed, Is.False, "Result should not be failed.");
        }

        private void AssertResultIsFailed<TFailure>(IResult<TFailure> result, Type? failureType = null)
            where TFailure : class
        {
            Assert.That(result.IsFailed, Is.True, "Result should be failed.");
            Assert.That(result.IsSuccessful, Is.False, "Result should not be successful.");

            if (failureType is not null)
            {
                Assert.That(result.Failure, Is.TypeOf(failureType));
            }
        }
    }
}
