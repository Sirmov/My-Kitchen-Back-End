// |-----------------------------------------------------------------------------------------------------|
// <copyright file="RecipeImagesService.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Recipes
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Http;

    using MongoDB.Bson;
    using MongoDB.Driver;
    using MongoDB.Driver.GridFS;

    using MyKitchen.Common.Constants;
    using MyKitchen.Common.ProblemDetails;
    using MyKitchen.Microservices.Recipes.Data.Common;
    using MyKitchen.Microservices.Recipes.Services.Common.ServiceResult;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Contracts;
    using MyKitchen.Microservices.Recipes.Services.Recipes.Dtos;

    /// <summary>
    /// This class encapsulates the business logic related to the images of the recipes.
    /// </summary>
    public class RecipeImagesService : IRecipeImagesService
    {
        private readonly IGridFSBucket bucket;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecipeImagesService"/> class.
        /// </summary>
        /// <param name="bucket">The implementation of <see cref="IGridFSBucket"/>.</param>
        public RecipeImagesService(IGridFSBucket bucket)
        {
            this.bucket = bucket;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<string>> UploadRecipeImageAsync(IFormFile recipeImage)
        {
            var contentType = recipeImage.ContentType;
            var fileName = recipeImage.FileName;

            var options = new GridFSUploadOptions
            {
                Metadata = new BsonDocument { { "FileName", fileName }, { "ContentType", contentType } },
            };

            var validationErrors = this.ValidateImage(recipeImage);

            if (validationErrors != string.Empty)
            {
                return new BadRequestDetails(validationErrors);
            }

            ObjectId imageId;

            using (var stream = recipeImage.OpenReadStream())
            {
                try
                {
                    imageId = await this.bucket.UploadFromStreamAsync(recipeImage.FileName, stream, options);
                }
                catch (Exception)
                {
                    return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
                }
            }

            return imageId.ToString();
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<RecipeImageDto>> GetRecipeImageAsync(string recipeImageId)
        {
            if (!ObjectId.TryParse(recipeImageId, out var recipeImageObjectId))
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidFormat, nameof(recipeImageId)));
            }

            var filter = Builders<GridFSFileInfo<ObjectId>>.Filter.Eq(f => f.Id, recipeImageObjectId);

            var image = await (await this.bucket.FindAsync(filter)).SingleOrDefaultAsync();

            if (image is null)
            {
                return new NotFoundDetails(string.Format(ExceptionMessages.EntityNotFound, nameof(image)));
            }

            string contentType = image.Metadata.TryGetValue("ContentType", out var bsonValue) ? bsonValue.AsString : "application/octet-stream";

            var stream = await this.bucket.OpenDownloadStreamAsync(recipeImageObjectId);

            var dto = new RecipeImageDto()
            {
                FileStream = stream,
                ContentType = contentType,
                FileName = image.Filename,
            };

            return dto;
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> DeleteRecipeImageAsync(string recipeImageId)
        {
            if (!ObjectId.TryParse(recipeImageId, out var recipeImageObjectId))
            {
                return new BadRequestDetails(string.Format(ExceptionMessages.InvalidFormat, nameof(recipeImageId)));
            }

            try
            {
                await this.bucket.DeleteAsync(recipeImageObjectId);
            }
            catch (Exception)
            {
                return new InternalServerErrorDetails(ExceptionMessages.InternalServerError);
            }

            return ServiceResult.Success;
        }

        private string ValidateImage(IFormFile image)
        {
            List<string> validationErrors = new ();

            if (image.FileName.Length < ModelConstraints.Recipe.ImageNameMinSize ||
                image.FileName.Length > ModelConstraints.Recipe.ImageNameMaxSize)
            {
                validationErrors.Add(
                    string.Format(
                        ExceptionMessages.ShouldBeBetween,
                        nameof(image.FileName),
                        nameof(image),
                        ModelConstraints.Recipe.ImageNameMinSize,
                        ModelConstraints.Recipe.ImageNameMaxSize));
            }

            if (image.Length < ModelConstraints.Recipe.ImageMinSize || image.Length > ModelConstraints.Recipe.ImageMaxSize)
            {
                validationErrors.Add(string.Format(ExceptionMessages.SizeIsNotAcceptable, nameof(image)));
            }

            return string.Join(' ', validationErrors);
        }
    }
}
