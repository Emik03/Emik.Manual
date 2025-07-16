// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>
/// The wrapper that primarily implements <see cref="IReadOnlyCollection{T}"/>,
/// while still containing the members in <see cref="IReadOnlyDictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="T">The type of value in the collection.</typeparam>
/// <param name="dictionary">The dictionary to use.</param>
#pragma warning disable MA0016
public readonly partial struct ArchipelagoDictionaryValues<T>([ProvidesContext] Dictionary<string, T> dictionary)
#pragma warning restore MA0016
    : IReadOnlyCollection<T>
{
    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.this"/>
    /// <exception cref="KeyNotFoundException">The key does not exist.</exception>
    [Pure]
    public ref readonly T? this[string key]
    {
        get
        {
            ref var value = ref CollectionsMarshal.GetValueRefOrNullRef(dictionary, key);

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
                ref CollectionsMarshal.GetValueRefOrNullRef(dictionary.GetAlternateLookup<ReadOnlySpan<char>>(), key);

            if (Unsafe.IsNullRef(ref value))
                throw new KeyNotFoundException($"\"{key}\" is referenced before it is defined in {typeof(T).Name}!");

            return ref value;
        }
    }

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    /// <inheritdoc cref="ImmutableArray{T}.IsDefault"/>
    [Pure]
    public bool IsDefault => dictionary is null;

    /// <inheritdoc cref="ImmutableArray{T}.IsDefaultOrEmpty"/>
    [Pure]
    public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

    // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
    /// <inheritdoc cref="ImmutableArray{T}.IsEmpty"/>
    [Pure]
    public bool IsEmpty => dictionary.Count is 0;

    /// <inheritdoc cref="IReadOnlyCollection{T}.Count"/>
    [Pure]
    public int Count => dictionary.Count;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Keys"/>
    [Pure]
    public IEnumerable<string> Keys => dictionary.Keys;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.Values"/>
    [Pure]
    public IEnumerable<T> Values => dictionary.Values;

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(string key) => dictionary.ContainsKey(key);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(Chars key) => ContainsKey(key.Span);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(ReadOnlyMemory<char> key) => ContainsKey(key.Span);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.ContainsKey"/>
    [Pure]
    public bool ContainsKey(ReadOnlySpan<char> key) =>
        dictionary.GetAlternateLookup<ReadOnlySpan<char>>().ContainsKey(key);

    /// <inheritdoc cref="Dictionary{TKey, TValue}.TryAdd"/>
    /// <remarks><para>
    /// This method should only be used if you absolutely trust your data, typically to get around
    /// <see cref="Region"/>'s cyclical dependencies. No validation is performed to make sure the manual's invariants
    /// remain intact. In most cases, you should use the methods provided in <see cref="World"/> instead.
    /// </para></remarks>
    public bool TryAdd(string key, T value) => dictionary.TryAdd(key, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(Chars key, T value) => TryAdd(key.Span, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(ReadOnlyMemory<char> key, T value) => TryAdd(key.Span, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd(ReadOnlySpan<char> key, T value) =>
        dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryAdd(key, value);

    /// <inheritdoc cref="TryAdd(string, T)"/>
    public bool TryAdd<TArchipelago>(TArchipelago value)
        where TArchipelago : T, IArchipelago<TArchipelago> =>
        TryAdd(value.Name, value);

    /// <inheritdoc cref="IReadOnlyDictionary{TKey, TValue}.TryGetValue"/>
    [Pure]
    public bool TryGetValue(string key, [MaybeNullWhen(false)] out T value) => dictionary.TryGetValue(key, out value);

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
        dictionary.GetAlternateLookup<ReadOnlySpan<char>>().TryGetValue(key, out value);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => $"[{dictionary.Values.Conjoin()}]";

    /// <inheritdoc />
    [Pure]
    public IEnumerator<T> GetEnumerator() => dictionary.Values.GetEnumerator();

    /// <inheritdoc />
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
