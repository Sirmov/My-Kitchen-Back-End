// |-----------------------------------------------------------------------------------------------------|
// <copyright file="TokenOptions.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Identity.Common.Options
{
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;

    /// <summary>
    /// This class adapts the options patter in .NET.
    /// It is used to encapsulate the configuration of the different application tokens.
    /// </summary>
    public class TokenOptions
    {
        private SymmetricSecurityKey? issuerSigningKey;

        /// <summary>
        /// Gets the symmetric security key used to sign the issued tokens.
        /// </summary>
        public SymmetricSecurityKey IssuerSigningKey
        {
            get
            {
                this.issuerSigningKey ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.SecurityKey));
                return this.issuerSigningKey;
            }
        }

        /// <summary>
        /// Gets or sets the secret security key used for creating the <see cref="TokenOptions.IssuerSigningKey"/>.
        /// </summary>
        public string SecurityKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the lifetime of a access token.
        /// </summary>
        public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

        /// <summary>
        /// Gets or sets the lifetime of a refresh token.
        /// </summary>
        public TimeSpan RefreshTokenLifetime { get; set; } = TimeSpan.FromDays(15);

        /// <summary>
        /// Gets or sets the jwt bearer options.
        /// </summary>
        public JwtBearerOptions JwtBearerOptions { get; set; } = new ();
    }
}
