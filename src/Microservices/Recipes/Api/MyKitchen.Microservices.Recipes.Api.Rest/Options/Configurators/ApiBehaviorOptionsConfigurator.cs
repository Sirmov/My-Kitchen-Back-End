// |-----------------------------------------------------------------------------------------------------|
// <copyright file="ApiBehaviorOptionsConfigurator.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Api.Rest.Options.Configurators
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// This class implements the <see cref="IConfigureOptions{TOptions}"/>.
    /// It is used to configure the <see cref="ApiBehaviorOptions"/>.
    /// </summary>
    public class ApiBehaviorOptionsConfigurator : IConfigureOptions<ApiBehaviorOptions>
    {
        /// <inheritdoc/>
        public void Configure(ApiBehaviorOptions options)
        {
            options.SuppressModelStateInvalidFilter = true;
        }
    }
}
