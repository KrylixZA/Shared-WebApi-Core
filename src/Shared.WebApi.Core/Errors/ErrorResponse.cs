using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace Shared.WebApi.Core.Errors
{
    /// <summary>
    /// A base error response model that will represent any errors that are handled by the API.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ErrorResponse
    {
        /// <summary>
        /// The error code correlating to the error that occurred.
        /// </summary>
        [Required]
        [JsonProperty("errorCode", Required = Required.Always)]
        public int ErrorCode { get; set; }

        /// <summary>
        /// The details of the error that occurred, including the stacktrace or any other relevant information.
        /// </summary>
        [Required]
        [JsonProperty("errorDetails", Required = Required.Always)]
        public string ErrorDetails { get; set; } = null!;

        /// <summary>
        /// A user friendly description of what went wrong.
        /// </summary>
        [JsonProperty("errorMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string ErrorMessage { get; set; } = null!;

        /// <summary>
        /// An enumeration of any inner exceptions that may have occurred.
        /// </summary>
        [JsonProperty("innerExceptions", NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<Exception>? InnerExceptions { get; set; }

        /// <summary>
        /// Returns a JSON string representation of this instance.
        /// </summary>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}