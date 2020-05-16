using System;
using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// An exception to represents a 401 - Unauthorized response.
    /// </summary>
    [Serializable]
    public class UnauthorizedException : BaseHttpException
    {
        /// <summary>
        /// Creates a new instance of the UnauthorizedException class.
        /// </summary>
        public UnauthorizedException()
        {
        }

        /// <summary>
        /// Creates a new instance of the UnauthorizedException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public UnauthorizedException(string? message, int errorCode) : base(message, errorCode)
        {
        }

        /// <summary>
        /// Creates a new instance of the UnauthorizedException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="errorCode">The error code.</param>
        public UnauthorizedException(string? message, Exception? innerException, int errorCode) : base(message, innerException, errorCode)
        {
        }
        
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.Unauthorized;
        }
    }
}