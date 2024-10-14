// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApplicationRole.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Models
{
    using AspNetCore.Identity.Mongo.Model;

    using MongoDB.Bson;

    /// <summary>
    /// This class is a extension of the base mongo identity role class. It inherits <see cref="MongoRole{TKey}"/>.
    /// </summary>
    public class ApplicationRole : MongoRole<ObjectId>
    {
    }
}
