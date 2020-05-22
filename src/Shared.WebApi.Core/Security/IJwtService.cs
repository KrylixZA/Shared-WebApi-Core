using System;

namespace Shared.WebApi.Core.Security
{
    /// <summary>
    /// A contract for JWT services.
    /// </summary>
    public interface IJwtService
    {
        /// <summary>
        /// Generates a security tokens based off the user's email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="now">The system's current date and time.</param>
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email, DateTime now);
        
        /// <summary>
        /// Generates a security tokens based off the user's email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="now">The system's current date and time.</param>
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email, int userId, DateTime now);
        
        /// <summary>
        /// Generates a security tokens based off the user's email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="userId">The user's unique identifier.</param>
        /// <param name="now">The system's current date and time.</param>
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email, Guid userId, DateTime now);
    }
}