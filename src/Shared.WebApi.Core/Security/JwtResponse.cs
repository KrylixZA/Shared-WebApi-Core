using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Shared.WebApi.Core.Security
{
    /// <summary>
    /// An object containing the JWT response properties.
    /// </summary>
    public class JwtResponse
    {
        /// <summary>
        /// The access token.
        /// </summary>
        [Required]
        [JsonProperty("accessToken", Required = Required.Always)]
        public string AccessToken { get; set; } = null!;

        /// <summary>
        /// The expiration time in minutes.
        /// </summary>
        [Required]
        [JsonProperty("expiresInMinutes", Required = Required.Always)]
        public int ExpiresInMinutes { get; set; }
    }
}