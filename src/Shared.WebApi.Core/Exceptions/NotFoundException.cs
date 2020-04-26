using System.Net;

namespace Shared.WebApi.Core.Exceptions
{
    /// <summary>
    /// Represents an instance of a not found exception.
    /// </summary>
    public class NotFoundException : BaseHttpException
    {
        /// <inheritdoc />
        public override HttpStatusCode GetHttpStatusCode()
        {
            return HttpStatusCode.NotFound;
        }
    }
}