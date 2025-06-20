// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Describes the entry when connecting regions conditionally with logic.</summary>
/// <param name="Region">The region to connect to.</param>
/// <param name="Logic">The logic that would enable this connection.</param>
public sealed partial record Passage(Region Region, Logic? Logic = null) : IAddTo, IArchipelago<Passage>
{
    /// <inheritdoc />
    public Chars Name => Region.Name;

    /// <inheritdoc />
    public static implicit operator Passage(string? name) => new((Region)name);

    /// <inheritdoc />
    public static implicit operator Passage(ReadOnlyMemory<char> name) => new((Region)name);

    /// <inheritdoc />
    public void CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value)
    {
        if (Logic is not null)
            (value ??= new JsonObject())[Name.ToString()] = Logic.ToString();
    }
}
