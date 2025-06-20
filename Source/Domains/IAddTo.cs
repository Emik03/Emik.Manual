// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>The interface for adding the implementing instances onto the <see cref="JsonNode"/>.</summary>
public interface IAddTo
{
    /// <summary>Converts the current instance to a <see langword="string"/> in JSON format.</summary>
    /// <param name="addTo">The <see cref="IAddTo"/> to convert.</param>
    /// <returns>The JSON <see langword="string"/>.</returns>
    [Pure]
    static string ToJsonString<T>(in T addTo)
        where T : IAddTo
    {
        JsonNode obj = new JsonObject();
        addTo.CopyTo(ref obj);
        return obj.ToJsonString();
    }

    /// <summary>Adds itself to the value.</summary>
    /// <param name="value">The value to mutate.</param>
    void CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value);
}
