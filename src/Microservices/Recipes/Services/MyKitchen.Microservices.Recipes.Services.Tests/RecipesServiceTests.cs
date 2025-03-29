// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipesServiceTests.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Tests
{
    using System.Threading.Tasks;

    using AutoMapper;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;

    using MongoDB.Bson;

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
                UserId = ObjectId.GenerateNewId().ToString(),
                ImageId = ObjectId.GenerateNewId().ToString(),
                Title = "Ice cream",
            },
            new Recipe()
            {
                Id = ObjectId.GenerateNewId().ToString(),
                UserId = ObjectId.GenerateNewId().ToString(),
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

            this.recipeImagesServiceMock.Setup(x => x.DeleteRecipeImageAsync(It.IsAny<string>()))
                .ReturnsAsync(Common.ServiceResult.ServiceResult.Success);

            this.recipesService = new RecipesService(
                this.recipeImagesServiceMock.Object,
                this.recipeRepositoryFake.Instance,
                this.mapper);
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.GetAllRecipesAsync(Common.QueryOptions.QueryOptions{Recipe}?)"/>
        /// returns an empty collection when recipes exist.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task GetRecipeAsync_RecipeWithIdExists_ReturnsRecipeDto()
        {
            // Arrange
            var recipe = this.recipes[0];

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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task CreateRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetailsAsync()
        {
            // Arrange
            string userId = "678e8dd45c9980448d2e2a7b";
            string ownerId = "678e88d590605d3dd639a0a6";

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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
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
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
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

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// returns <see cref="BadRequestDetails"/> when the id's format is not correct.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_IncorrectIdFormat_ReturnsBadRequestDetails()
        {
            // Arrange
            string id = "I'm surely not correct.";

            // Act
            var updateResult = await this.recipesService.UpdateRecipeAsync(string.Empty, id, new ());

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(BadRequestDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"
        /// return <see cref="NotFoundDetails"/> when no recipe with matching id was found.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_NoRecipeWithId_ReturnsNotFoundDetails()
        {
            // Arrange
            string id = ObjectId.GenerateNewId().ToString();

            // Act
            var updateResult = await this.recipesService.UpdateRecipeAsync(ObjectId.GenerateNewId().ToString(), id, new ());

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// returns <see cref="BadRequestDetails"/> when the recipe dto state is not valid. It checks the minimum
        /// length constraint of the text properties.
        /// </summary>
        /// <param name="title">The title of the recipe.</param>
        /// <param name="description">The description of the recipe.</param>
        /// <param name="ingredients">The ingredients of the recipe.</param>
        /// <param name="directions">The directions of the recipe.</param>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        [TestCase("Ic", "Delicious ice coffee", "Ice, water coffee, milk", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious", "Ice, water coffee, milk", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious ice coffee", "Ice,", "Brew and enjoy!")]
        [TestCase("Ice coffee", "Delicious ice coffee", "Ice, water coffee, milk", "Brew and!")]
        public async Task UpdateRecipeAsync_RecipeNotValid_ReturnsBadRequestDetailsAsync(
            string title,
            string description,
            string ingredients,
            string directions)
        {
            // Arrange
            string userId = ObjectId.GenerateNewId().ToString();

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
            var updateResult = await this.recipesService.UpdateRecipeAsync(userId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(BadRequestDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// returns <see cref="UnauthorizedDetails"/> when the current user isn't the owner of the recipe.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetails()
        {
            // Arrange
            string userId = "678e8dd45c9980448d2e2a7b";
            string ownerId = "678e88d590605d3dd639a0a6";

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
            var updateResult = await this.recipesService.UpdateRecipeAsync(userId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(UnauthorizedDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// in the event of a failure when deleting the recipe image the causing problem details are returned.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_RecipeImageDeletionFails_ReturnsImageDeleteFailure()
        {
            // Arrange
            this.recipeImagesServiceMock.Setup(x => x.DeleteRecipeImageAsync(It.IsAny<string>()))
                .ReturnsAsync(new NotFoundDetails(string.Empty));

            var ownerId = this.recipes[0].UserId;

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
            var updateResult = await this.recipesService.UpdateRecipeAsync(ownerId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// in the event of a failure when uploading the recipe image the causing problem details are returned.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_RecipeImageUploadingFails_ReturnsImageUploadFailure()
        {
             // Arrange
            this.recipeImagesServiceMock.Setup(x => x.UploadRecipeImageAsync(It.IsAny<IFormFile>()))
                .ReturnsAsync(new NotFoundDetails(string.Empty));

            var ownerId = this.recipes[0].UserId;

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
            var updateResult = await this.recipesService.UpdateRecipeAsync(ownerId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// returns <see cref="InternalServerErrorDetails"/> when updating fails.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_RecipeUpdatingFails_ReturnsInternalServerErrorDetails()
        {
            // Arrange
            this.recipeRepositoryFake.Mock
                .Setup(x => x.UpdateAsync(It.IsAny<string>(), It.IsAny<Recipe>()))
                .ReturnsAsync(new Exception());

            var ownerId = this.recipes[0].UserId;

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
            var updateResult = await this.recipesService.UpdateRecipeAsync(ownerId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsFailed(updateResult, typeof(InternalServerErrorDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.UpdateRecipeAsync(string, string, RecipeInputDto)"/>
        /// correctly updates the data of the recipe and returns the dto of the updated recipe.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task UpdateRecipeAsync_RecipeIsUpdated_ReturnsUpdatedRecipeDto()
        {
            // Arrange
            var random = new Random();
            var salt = string.Join(string.Empty, random.NextInt64().ToString().Take(5));

            var ownerId = this.recipes[0].UserId;

            var recipeInputDto = new RecipeInputDto()
            {
                Image = new FormFile(default, default, default, default, default),
                UserId = ownerId,
                Title = $"Ice coffee {salt}",
                Description = $"Delicious ice coffee {salt}",
                Ingredients = $"Ice, water coffee, milk {salt}",
                Directions = $"Brew and enjoy! {salt}",
            };

            // Act
            var updateResult = await this.recipesService.UpdateRecipeAsync(ownerId, this.recipes[0].Id, recipeInputDto);

            // Assert
            this.AssertResultIsSuccessful(updateResult);

            var recipeDto = updateResult.Data!;
            Assert.That(recipeDto.Title, Is.EqualTo(recipeInputDto.Title));
            Assert.That(recipeDto.Description, Is.EqualTo(recipeInputDto.Description));
            Assert.That(recipeDto.Ingredients, Is.EqualTo(recipeInputDto.Ingredients));
            Assert.That(recipeDto.Directions, Is.EqualTo(recipeInputDto.Directions));
            Assert.That(recipeDto.ModifiedOn, Is.GreaterThan(DateTime.MinValue));

            var recipe = (await this.recipeRepositoryFake.Instance.FindAsync(recipeDto.Id, false)).Data!;
            Assert.That(recipe.Title, Is.EqualTo(recipeInputDto.Title));
            Assert.That(recipe.Description, Is.EqualTo(recipeInputDto.Description));
            Assert.That(recipe.Ingredients, Is.EqualTo(recipeInputDto.Ingredients));
            Assert.That(recipe.Directions, Is.EqualTo(recipeInputDto.Directions));
            Assert.That(recipeDto.ModifiedOn, Is.GreaterThan(DateTime.MinValue));
        }

        /// <summary>
        /// This test whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// returns <see cref="BadRequestDetails"/> when the id's format is not correct.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_IncorrectIdFormat_ReturnsBadRequestDetails()
        {
            // Arrange
            string id = "I'm surely not correct.";

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(string.Empty, id);

            // Assert
            this.AssertResultIsFailed(deleteResult, typeof(BadRequestDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// returns <see cref="NotFoundDetails"/> when no recipe with provided id exists.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_NoRecipeWithId_ReturnsNotFoundDetails()
        {
           // Arrange
            string id = ObjectId.GenerateNewId().ToString();

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(string.Empty, id);

            // Assert
            this.AssertResultIsFailed(deleteResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// returns <see cref="UnauthorizedDetails"/> when the current user isn't the owner of the recipe.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_RecipeUserIdDoesNotMatchCurrentUserId_ReturnsUnauthorizedDetails()
        {
           // Arrange
            string userId = ObjectId.GenerateNewId().ToString();

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(userId, this.recipes[0].Id);

            // Assert
            this.AssertResultIsFailed(deleteResult, typeof(UnauthorizedDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// in the event of a failure when deleting the recipe image the causing problem details are returned.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_RecipeImageDeletionFails_ReturnsImageDeleteFailure()
        {
            // Arrange
            this.recipeImagesServiceMock.Setup(x => x.DeleteRecipeImageAsync(It.IsAny<string>()))
                .ReturnsAsync(new NotFoundDetails(string.Empty));

            var recipe = this.recipes[0];

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(recipe.UserId, recipe.Id);

            // Assert
            this.AssertResultIsFailed(deleteResult, typeof(NotFoundDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// returns <see cref="InternalServerErrorDetails"/>  when deleting fails.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_RecipeDeletingFails_ReturnsInternalServerErrorDetails()
        {
            // Arrange
            this.recipeRepositoryFake.Mock
                .Setup(x => x.DeleteAsync(It.IsAny<string>()))
                .ReturnsAsync(new Exception());

            var recipe = this.recipes[0];

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(recipe.UserId, recipe.Id);

            // Assert
            this.AssertResultIsFailed(deleteResult, typeof(InternalServerErrorDetails));
        }

        /// <summary>
        /// This test checks whether <see cref="RecipesService.DeleteRecipeAsync(string, string)"/>
        /// returns a successful result when deletion is successful.
        /// </summary>
        /// <returns>Returns a <see cref="Task"/> representing the asynchronous operation.</returns>
        [Test]
        public async Task DeleteRecipeAsync_RecipeIsDeleted_ReturnsSuccessfulResult()
        {
            // Arrange
            var recipe = this.recipes[0];

            // Act
            var deleteResult = await this.recipesService.DeleteRecipeAsync(recipe.UserId, recipe.Id);

            // Assert
            this.AssertResultIsSuccessful(deleteResult);

            var findResult = await this.recipeRepositoryFake.Instance.FindAsync(recipe.Id, true);

            this.AssertResultIsSuccessful(findResult);
            Assert.That(findResult.Data, Is.Not.Null);
            Assert.That(findResult.Data.IsDeleted, Is.True);
            Assert.That(findResult.Data.DeletedOn, Is.GreaterThan(DateTime.MinValue));

            findResult.Data.IsDeleted = false;
            await this.recipeRepositoryFake.Instance.UpdateAsync(recipe.Id, findResult.Data);
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
