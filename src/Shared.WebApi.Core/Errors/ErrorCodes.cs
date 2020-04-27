namespace Shared.WebApi.Core.Errors
{
    /// <summary>
    /// A list of error codes that can be returned from the shared library.
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// The token provided does not provide access to this resource.
        /// </summary>
        UnauthorizedRequest = 1,
        
        /// <summary>
        /// The request was forbidden from proceeding.
        /// </summary>
        ForbiddenRequest = 2
    }
}