using System;
using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// Represents an instance of a not found exception.
    /// </summary>
    [Serializable]
    public class NotFoundException : BaseHttpException
    {
        /// <summary>
        /// Creates a new instance of the NotFoundException class.
        /// </summary>
        public NotFoundException()
        {
        }

        /// <summary>
        /// Creates a new instance of the NotFoundException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="errorCode">The error code.</param>
        public NotFoundException(string? message, int errorCode) : base(message, errorCode)
        {
        }

        /// <summary>
        /// Creates a new instance of the NotFoundException class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="errorCode">The error code.</param>
        public NotFoundException(string? message, Exception? innerException, int errorCode) : base(message, innerException, errorCode)
        {
        }
        
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}