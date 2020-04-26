namespace Shared.WebApi.Core.Errors
{
    /// <summary>
    /// A contract for resolving user friendly error messages.
    /// </summary>
    public interface IErrorMessageSelector
    {
        /// <summary>
        /// Gets a user friendly error message of what went wrong based off the error code.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <returns>The user friendly message.</returns>
        string GetErrorMessage(int errorCode);
    }
}