using System.Security.Claims;

namespace Joblin.Application.Common.Abstractions;

public interface IIdentityService
{
    /// <summary>
    /// Check if the current user is authenticated and has the given role.
    /// </summary>
    /// <param name="role">The <see cref="string"/> role to be checked for.</param>
    /// <returns>True if the user has that role, otherwise false</returns>
    Task<bool> IsInRoleAsync(string role);

    /// <summary>
    /// Check if the current user is authenticated and has the given policy.
    /// </summary>
    /// <param name="policyName">The <see cref="string"/> policy to be checked for.</param>
    /// <returns>True if the user has that policy, otherwise false</returns>
    Task<bool> AuthorizeAsync(string policyName);

    /// <summary>
    /// Check if the current user is authenticated and has the given claim.
    /// </summary>
    /// <param name="claim"></param>
    /// <returns></returns>
    Task<bool> AuthorizeAsync(Claim claim);
}