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
        public string AccessToken { get; set; }

        /// <summary>
        /// The expiration time in minutes.
        /// </summary>
        [Required]
        [JsonProperty("expiresIn", Required = Required.Always)]
        public int ExpiresIn { get; set; }
    }
}