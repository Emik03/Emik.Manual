// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;

/// <summary>Represents a yaml setting.</summary>
/// <param name="Name">The name of the value.</param>
public readonly partial record struct Yaml([Match(Yaml.DisallowedChars, true)] Chars Name)
    : IComparisonOperators<Yaml, int, Logic>, IEquatable<object>
{
    /// <summary>The characters that are disallowed.</summary>
    [StringSyntax(StringSyntaxAttribute.Regex)]
    const string DisallowedChars = "[^'()]";

    /// <summary>Searches for characters that are disallowed.</summary>
    static readonly SearchValues<char> s_disallowedChars = SearchValues.Create("'()");

    /// <summary>
    /// Creates the <see cref="Logic"/> determining whether this option should be enabled or disabled.
    /// </summary>
    /// <param name="enabled">
    /// Whether to use <see cref="Logic.Builtin.YamlEnabled"/> or <see cref="Logic.Builtin.YamlDisabled"/>.
    /// </param>
    [Pure]
    public Logic this[bool enabled] => enabled ? Logic.Enabled(this) : Logic.Disabled(this);

    /// <summary>Creates the <see cref="Logic"/> that requires this value be greater than the value provided.</summary>
    /// <param name="count">The threshold. If <c>IsFromEnd</c>, asks if less than or equal instead.</param>
    [Pure]
    public Logic this[Index count] => count.IsFromEnd ? this <= count.Value : this >= count.Value;

    /// <summary>Gets itself in <see cref="Logic.Disabled"/>.</summary>
    [Pure]
    public Logic Disabled => Logic.Disabled(this);

    /// <summary>Gets itself in <see cref="Logic.Enabled"/>.</summary>
    [Pure]
    public Logic Enabled => Logic.Enabled(this);

    /// <summary>
    /// Implicitly converts the <see name="char"/> <see cref="Array"/> to the <see cref="Yaml"/>.
    /// </summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Yaml([Match(DisallowedChars, true)] char[]? yaml) => new(yaml);

    /// <summary>Implicitly converts the <see name="string"/> to the <see cref="Yaml"/>.</summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Yaml([Match(DisallowedChars, true)] string? yaml) => new(yaml.AsMemory());

    /// <summary>
    /// Implicitly converts the <see name="ArraySegment{T}"/> to the <see cref="Yaml"/>.
    /// </summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Yaml([Match(DisallowedChars, true)] ArraySegment<char> yaml) => new(yaml);

    /// <summary>Implicitly converts the <see name="Memory{T}"/> to the <see cref="Yaml"/>.</summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Yaml([Match(DisallowedChars, true)] Memory<char> yaml) => new(yaml);

    /// <summary>
    /// Implicitly converts the <see name="ReadOnlyMemory{T}"/> to the <see cref="Yaml"/>.
    /// </summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator Yaml([Match(DisallowedChars, true)] ReadOnlyMemory<char> yaml) => new(yaml);

    /// <summary>Implicitly gets <see cref="Name"/>.</summary>
    /// <param name="yaml">The characters to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining), Pure]
    public static implicit operator ReadOnlyMemory<char>(Yaml yaml) => yaml.Name;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is equal.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator ==(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlEqual);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is not equal.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator !=(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlNotEqual);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is greater than.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator >(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlGreaterThan);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is less than or equal.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator >=(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlGreaterThanOrEqual);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is less than.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator <(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlLessThan);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is greater than or equal.</summary>
    /// <param name="l">The value to compare.</param>
    /// <param name="r">The constant to compare it to.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator <=(Yaml l, int r) => new(l.Name, r, Logic.Kind.YamlLessThanOrEqual);

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is equal.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator ==(int l, Yaml r) => r == l;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is not equal.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator !=(int l, Yaml r) => r != l;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is greater than or equal.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator >=(int l, Yaml r) => r <= l;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is greater than.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator >(int l, Yaml r) => r < l;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is less than or equal.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator <=(int l, Yaml r) => r >= l;

    /// <summary>Creates the <see cref="Logic"/> determining whether the value is less than.</summary>
    /// <param name="l">The constant to compare it to.</param>
    /// <param name="r">The value to compare.</param>
    /// <returns>The new <see cref="Logic"/> instance.</returns>
    [Pure]
    public static Logic operator <(int l, Yaml r) => r > l;

    /// <inheritdoc cref="Json(ReadOnlySpan{Yaml})"/>
    [Pure]
    public static JsonArray? Json(params ImmutableArray<Yaml> array) => Json(array.AsSpan());

    /// <summary>Converts the <see cref="Span{T}"/> into the <see cref="JsonArray"/>.</summary>
    /// <param name="span">The <see cref="Span{T}"/> to convert.</param>
    /// <returns>The <see cref="JsonArray"/> containing the values from <paramref name="span"/>.</returns>
    [Pure]
    public static JsonArray? Json(params ReadOnlySpan<Yaml> span)
    {
        if (span.IsEmpty)
            return null;

        JsonArray options = [];

        foreach (var option in span)
            options.Add(option.ToString());

        return options;
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Name.ToString();

    /// <summary>Gets <see cref="Name"/> as a string.</summary>
    /// <param name="strict">Whether to throw when this instance contains disallowed characters.</param>
    /// <returns>The string.</returns>
    [MustUseReturnValue]
    public string ToString(bool strict) => Name.Replace(s_disallowedChars, strict).ToString();
}
