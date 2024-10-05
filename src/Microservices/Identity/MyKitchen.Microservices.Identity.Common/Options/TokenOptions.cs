namespace MyKitchen.Microservices.Identity.Common
{
    using System.Text;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.IdentityModel.Tokens;

    public class TokenOptions
    {
        private SymmetricSecurityKey? issuerSigningKey = null;

        public SymmetricSecurityKey IssuerSigningKey
        {
            get
            {
                this.issuerSigningKey ??= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.SecurityKey));

                return this.issuerSigningKey;
            }
        }

        public string SecurityKey { get; set; } = string.Empty;

        public TimeSpan AccessTokenLifetime = TimeSpan.FromMinutes(15);

        public JwtBearerOptions JwtBearerOptions { get; set; } = new();
    }
}
