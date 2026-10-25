using System.Security.Claims;

namespace TrainLab.SharedKernel.Users;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return Guid.TryParse(value, out var userId)
            ? userId
            : throw new InvalidOperationException("The current user has no valid id claim.");
    }
}