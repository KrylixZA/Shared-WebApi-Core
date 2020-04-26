using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// Represents an instance of a bad request.
    /// </summary>
    public class BadRequestException : BaseHttpException
    {
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.BadRequest;
        }
    }
}