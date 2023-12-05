#region

using System.Collections.Immutable;

#endregion

namespace AgilizAPI.Identity;

public static class IdentityData
{
    public static ImmutableDictionary<string, string> getRoles()
    {
        return new Dictionary<string, string> {
            { "User", "User" },
            { "Entrepreneur", "Entrepreneur" }
        }.ToImmutableDictionary();
    }
}