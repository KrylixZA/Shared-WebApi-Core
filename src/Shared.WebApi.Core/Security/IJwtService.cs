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
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email);
        
        /// <summary>
        /// Generates a security tokens based off the user's email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email, int userId);
        
        /// <summary>
        /// Generates a security tokens based off the user's email.
        /// </summary>
        /// <param name="email">The user's email.</param>
        /// <param name="userId">The user's unique identifier.</param>
        /// <returns>An encoded JWT token.</returns>
        JwtResponse GenerateSecurityToken(string email, Guid userId);
    }
}