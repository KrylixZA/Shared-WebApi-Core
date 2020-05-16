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
        /// Creates a new instance of the BaseHttpException class.
        /// </summary>
        protected BaseHttpException()
        {
        }

        /// <summary>
        /// Creates a new instance of the BaseHttpException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        protected BaseHttpException(string? message, int errorCode) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <summary>
        /// Creates a new instance of the BaseHttpException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The exception.</param>
        /// <param name="errorCode">The error code.</param>
        protected BaseHttpException(string? message, Exception? innerException, int errorCode) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
        
        /// <summary>
        /// The error code correlating to the error that occurred.
        /// </summary>
        [Required]
        public int ErrorCode { get; }
        
        /// <summary>
        /// Gets the System.Net.HttpStatusCode associated with the exception that was thrown.
        /// </summary>
        public virtual HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.InternalServerError;
        }
    }
}