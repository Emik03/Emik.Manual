// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents the <see cref="ReadOnlyMemory{T}"/> of <see cref="char"/>, with implicit conversions.</summary>
/// <param name="Memory">The characters.</param>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Chars(ReadOnlyMemory<char> Memory) : IComparable,
    IComparable<object>,
    IComparable<Chars>,
    IComparisonOperators<Chars, Chars, bool>,
    IEquatable<object>
{
    /// <inheritdoc cref="ReadOnlySpan{T}.this"/>
    public ref readonly char this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => ref Span[i];
    }

    /// <inheritdoc cref="ReadOnlyMemory{T}.IsEmpty"/>
    public bool IsEmpty
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => Memory.IsEmpty;
    }

    /// <inheritdoc cref="ReadOnlyMemory{T}.Length"/>
    public int Length
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => Memory.Length;
    }

    /// <inheritdoc cref="ReadOnlyMemory{T}.Span"/>
    public ReadOnlySpan<char> Span
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
        get => Memory.Span;
    }

    /// <summary>
    /// Implicitly converts the <see name="char"/> <see cref="Array"/> to the <see cref="Chars"/>.
    /// </summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Chars(char[]? chars) => new(chars);

    /// <summary>Implicitly converts the <see name="string"/> to the <see cref="Chars"/>.</summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Chars(string? chars) => new(chars.AsMemory());

    /// <summary>
    /// Implicitly converts the <see name="ArraySegment{T}"/> to the <see cref="Chars"/>.
    /// </summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Chars(ArraySegment<char> chars) => new(chars);

    /// <summary>Implicitly converts the <see name="Memory{T}"/> to the <see cref="Chars"/>.</summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Chars(Memory<char> chars) => new(chars);

    /// <summary>
    /// Implicitly converts the <see name="ReadOnlyMemory{T}"/> to the <see cref="Chars"/>.
    /// </summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Chars(ReadOnlyMemory<char> chars) => new(chars);

    /// <summary>Implicitly gets <see cref="Memory"/>.</summary>
    /// <param name="chars">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlyMemory<char>(Chars chars) => chars.Memory;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator >(Chars left, Chars right) => left.CompareTo(right) > 0;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator >=(Chars left, Chars right) => left.CompareTo(right) >= 0;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator <(Chars left, Chars right) => left.CompareTo(right) < 0;

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static bool operator <=(Chars left, Chars right) => left.CompareTo(right) <= 0;

    /// <inheritdoc cref="ReadOnlyMemory{T}.CopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void CopyTo(Memory<char> destination) => Memory.CopyTo(destination);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public bool Equals(Chars other) => Span.Equals(other.Span, StringComparison.Ordinal);

    /// <inheritdoc cref="ReadOnlyMemory{T}.TryCopyTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryCopyTo(Memory<char> destination) => Memory.TryCopyTo(destination);

    /// <inheritdoc cref="ReadOnlyMemory{T}.ToArray"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public char[] ToArray() => Memory.ToArray();

    /// <inheritdoc cref="IComparable.CompareTo"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int CompareTo(object? other) => CompareTo(other is Chars chars ? chars : default);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public int CompareTo(Chars other) => other.Span.CompareTo(other.Span, StringComparison.Ordinal);

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override int GetHashCode() => Memory.GetHashCode();

    /// <inheritdoc />
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public override string ToString() => Memory.ToString();

    /// <inheritdoc cref="ReadOnlyMemory{T}.Slice(int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure] // ReSharper disable once ReplaceSliceWithRangeIndexer
#pragma warning disable IDE0057
    public Chars Slice(int start) => Memory.Slice(start);
#pragma warning restore IDE0057
    /// <inheritdoc cref="ReadOnlyMemory{T}.Slice(int, int)"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public Chars Slice(int start, int length) => Memory.Slice(start, length);

    /// <summary>Throws or replaces with an empty string on matching a <see cref="Regex"/>.</summary>
    /// <param name="regex">The regex to match with.</param>
    /// <param name="throwOnMatch">Whether to throw when a match occurs.</param>
    /// <returns>Itself with potentially removed characters.</returns>
    /// <exception cref="FormatException">
    /// The parameter <paramref name="throwOnMatch"/> is <see langword="true"/>,
    /// and the parameter <paramref name="regex"/> matched with this instance.
    /// </exception>
    [Pure]
    public Chars Replace(Regex regex, bool throwOnMatch)
    {
        if (throwOnMatch)
        {
            foreach (var match in regex.EnumerateMatches(Span))
                throw new FormatException(
                    $"The string \"{this}\" has invalid characters (such as \"{Slice(match.Index, match.Length)}\" at character {match.Index})"
                );

            return this;
        }

        StringBuilder? builder = null;
        var offset = 0;

        foreach (var m in regex.EnumerateMatches(Span))
        {
            _ = builder is null
                ? (builder = new(Length, Length)).Append(Span[..m.Index]).Append(Span[(m.Index + m.Length)..])
                : builder.Remove(m.Index - offset, m.Length);

            offset += m.Length;
        }

        return builder?.ToString() ?? this;
    }

    /// <summary>Throws or replaces with an empty string on matching a <see cref="SearchValues{T}"/>.</summary>
    /// <param name="search">The search to match with.</param>
    /// <param name="throwOnMatch">Whether to throw when a match occurs.</param>
    /// <returns>Itself with potentially removed characters.</returns>
    /// <exception cref="FormatException">
    /// The parameter <paramref name="throwOnMatch"/> is <see langword="true"/>,
    /// and the parameter <paramref name="search"/> matched with this instance.
    /// </exception>
    [MustUseReturnValue]
    public Chars Replace(SearchValues<char> search, bool throwOnMatch)
    {
        if (!throwOnMatch)
            return Span.SplitOn(search).ToString();

        if (Span.IndexOfAny(search) is not -1 and var i)
            throw new FormatException(
                $"The string \"{this}\" has invalid characters (such as \"{this[i]}\" at character {i})"
            );

        return this;
    }

    /// <inheritdoc cref="ReadOnlyMemory{T}.Pin"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public MemoryHandle Pin() => Memory.Pin();

    /// <summary>Gets an enumerator for this span.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public ReadOnlySpan<char>.Enumerator GetEnumerator() => Span.GetEnumerator();
}
