using System;
using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// Represents an instance of a bad request.
    /// </summary>
    public class BadRequestException : BaseHttpException
    {
        /// <summary>
        /// Creates a new instance of the BadRequestException class.
        /// </summary>
        public BadRequestException()
        {
        }

        /// <summary>
        /// Creates a new instance of the BadRequestException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode"></param>
        public BadRequestException(string? message, int errorCode) : base(message, errorCode)
        {
        }

        /// <summary>
        /// Creates a new instance of the BadRequestException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="errorCode">The error code.</param>
        public BadRequestException(string? message, Exception? innerException, int errorCode) : base(message, innerException, errorCode)
        {
        }
        
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }
    }
}