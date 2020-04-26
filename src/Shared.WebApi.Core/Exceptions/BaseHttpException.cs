using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// Represents a base HTTP exception.
    /// </summary>
    public abstract class BaseHttpException : Exception
    {
        /// <summary>
        /// The error code correlating to the error that occurred.
        /// </summary>
        [Required]
        public int ErrorCode { get; set; }
        
        /// <summary>
        /// Gets the System.Net.HttpStatusCode associated with the exception that was thrown.
        /// </summary>
        public virtual HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}