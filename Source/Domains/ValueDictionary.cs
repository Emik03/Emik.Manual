// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>
/// The wrapper that primarily implements <see cref="IReadOnlyCollection{T}"/>,
/// while still containing the members in <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="T">The type of value in the collection.</typeparam>
/// <param name="dictionary">The dictionary to use.</param>
#pragma warning disable MA0016
public readonly partial struct ValueDictionary<T>([ProvidesContext] Dictionary<string, T> dictionary)
#pragma warning restore MA0016
    : IEqualityOperators<ValueDictionary<T>, ValueDictionary<T>, bool>,
        IEquatable<object>,
        IEquatable<ValueDictionary<T>>,
        IReadOnlyCollection<T>
{
    /// <summary>Contains the stored values.</summary>
    readonly Dictionary<string, T> _dictionary = dictionary;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.this"/>
    /// <exception cref="KeyNotFoundException">The key does not exist.</exception>
    [Pure]
    public ref readonly T? this[string key]
    {
        get
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary, key);

            if (Unsafe.IsNullRef(ref value))
                throw new KeyNotFoundException($"\"{key}\" is referenced before it is defined in {typeof(T).Name}!");

            return ref value;
        }
    }

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.this"/>
    /// <exception cref="KeyNotFoundException">The key does not exist.</exception>
    [Pure]
    public ref readonly T? this[Chars key] => ref this[key.Span];

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.this"/>
    /// <exception cref="KeyNotFoundException">The key does not exist.</exception>
    [Pure]
    public ref readonly T? this[ReadOnlyMemory<char> key] => ref this[key.Span];

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.this"/>
    /// <exception cref="KeyNotFoundException">The key does not exist.</exception>
    [Pure]
    public ref readonly T? this[ReadOnlySpan<char> key]
    {
        get
        {
            ref var value =
                ref CollectionsMarshal.GetValueRefOrNullRef(_dictionary.GetAlternateLookup<ReadOnlySpan<char>>(), key);

            if (Unsafe.IsNullRef(ref value))
                throw new KeyNotFoundException($"\"{key}\" is referenced before it is defined in {typeof(T).Name}!");

            return ref value;
        }
    }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    /// <inheritdoc cref="ImmutableArray{T}.IsDefault"/>
    [Pure]
    public bool IsDefault => _dictionary is null;

    /// <inheritdoc cref="ImmutableArray{T}.IsDefaultOrEmpty"/>
    [Pure]
    public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    /// <inheritdoc cref="ImmutableArray{T}.IsEmpty"/>
    [Pure]
    public bool IsEmpty => _dictionary.Count is 0;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
    [Pure]
    public int Count => _dictionary.Count;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
    [Pure]
    public IEnumerable<string> Keys => _dictionary.Keys;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Values"/>
    [Pure]
    public IEnumerable<T> Values => _dictionary.Values;

    /// <inheritdoc />
    [Pure]
    public static bool operator ==(ValueDictionary<T> left, ValueDictionary<T> right) =>
        left._dictionary == right._dictionary;

    /// <inheritdoc />
    [Pure]
    public static bool operator !=(ValueDictionary<T> left, ValueDictionary<T> right) =>
        left._dictionary != right._dictionary;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(string key) => _dictionary.ContainsKey(key);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(Chars key) => ContainsKey(key.Span);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(ReadOnlyMemory<char> key) => ContainsKey(key.Span);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(ReadOnlySpan<char> key) =>
        _dictionary.GetAlternateLookup<ReadOnlySpan<char>>().ContainsKey(key);

    /// <inheritdoc cref="object.Equals(object)"/>
    [Pure]
    public override bool Equals(object? obj) => obj is ValueDictionary<T> other && Equals(other);

    /// <inheritdoc />
    [Pure]
    public bool Equals(ValueDictionary<T> other) => this == other;

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd"/>
    /// <remarks><para>
    /// This method should only be used if you absolutely trust your data, typically to get around
    /// <see cref="Region"/>'s cyclical dependencies. No validation is performed to make sure the manual's invariants
    /// remain intact. In most cases, you should use the methods provided in <see cref="World"/> instead.
    /// </para></remarks>
    public bool TryAdd(string key, T value) => _dictionary.TryAdd(key, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(Chars key, T value) => TryAdd(key.Span, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(ReadOnlyMemory<char> key, T value) => TryAdd(key.Span, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(ReadOnlySpan<char> key, T value) =>
        _dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryAdd(key, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd<TArchipelago>(TArchipelago value)
        where TArchipelago : T, IArchipelago<TArchipelago> =>
        TryAdd(value.Name, value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [Pure]
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) => _dictionary.TryGetValue(key, out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [Pure]
    public bool TryGetValue(Chars key, [MaybeNullWhen(false)] out T value) => TryGetValue(key.Span, out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [Pure]
    public bool TryGetValue(ReadOnlyMemory<char> key, [MaybeNullWhen(false)] out T value) =>
        TryGetValue(key.Span, out value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [Pure]
    public bool TryGetValue(ReadOnlySpan<char> key, [MaybeNullWhen(false)] out T value) =>
        _dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(key, out value);

    /// <inheritdoc />
    [Pure]
    public override int GetHashCode() => _dictionary.GetHashCode();

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"[{_dictionary.Values.Conjoin()}]";

    /// <inheritdoc />
    [Pure]
    public IEnumerator<T> GetEnumerator() => _dictionary.Values.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
