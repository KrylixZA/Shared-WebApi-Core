using System;
using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// An exception to represents a 403 - Forbidden response.
    /// </summary>
    [Serializable]
    public class ForbiddenException : BaseHttpException
    {
        /// <summary>
        /// Creates a new instance of the ForbiddenException class.
        /// </summary>
        public ForbiddenException()
        {
        }

        /// <summary>
        /// Creates a new instance of the ForbiddenException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public ForbiddenException(string? message, int errorCode) : base(message, errorCode)
        {
        }

        /// <summary>
        /// Creates a new instance of the ForbiddenException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="errorCode">The error code.</param>
        public ForbiddenException(string? message, Exception? innerException, int errorCode) : base(message, innerException, errorCode)
        {
        }
        
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.Forbidden;
        }
    }
}