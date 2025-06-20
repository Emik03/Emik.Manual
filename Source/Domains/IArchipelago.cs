// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents an object within the world.</summary>
/// <typeparam name="T">The type that implements this interface.</typeparam>
public interface IArchipelago<out T>
    where T : IArchipelago<T>
{
    /// <summary>The quantity of this instance.</summary>
    [Pure]
    int Count => 1;

    /// <summary>Gets the name.</summary>
    [Pure]
    Chars Name { get; }

    /// <summary>Gets the categories.</summary>
    [Pure]
    ImmutableArray<Category> Categories => [];

    /// <summary>Implicitly wraps <typeparamref name="T"/> around the <see cref="string"/> instance.</summary>
    /// <param name="name">The <see cref="string"/> to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    static abstract implicit operator T(string? name);

    /// <summary>
    /// Implicitly wraps <typeparamref name="T"/> around the <see cref="ReadOnlyMemory{T}"/> instance.
    /// </summary>
    /// <param name="name">The <see cref="ReadOnlyMemory{T}"/> to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    static abstract implicit operator T(ReadOnlyMemory<char> name);

    /// <inheritdoc cref="Json(ReadOnlySpan{T})"/>
    [Pure]
    static JsonArray Json(ImmutableArray<T> span) => Json(span.AsSpan());

    /// <summary>Creates the <see cref="JsonArray"/> around the passed in <see cref="ReadOnlySpan{T}"/>.</summary>
    /// <param name="span">The <see cref="ReadOnlySpan{T}"/> to enumerate.</param>
    /// <returns>The <see cref="JsonArray"/> containing values from the parameter <paramref name="span"/>.</returns>
    [Pure]
    static JsonArray Json(ReadOnlySpan<T> span)
    {
        JsonArray ret = [];

        foreach (var ap in span)
            ret.Add(ap.Name.ToString());

        return ret;
    }
}
