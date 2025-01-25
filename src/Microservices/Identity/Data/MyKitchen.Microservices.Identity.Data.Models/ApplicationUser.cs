// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApplicationUser.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Data.Models
{
    using AspNetCore.Identity.Mongo.Model;

    /// <summary>
    /// This class is a extension of the base mongo identity user class. It inherits <see cref="MongoUser{TKey}"/>.
    /// </summary>
    public class ApplicationUser : MongoUser<string>
    {
    }
}
