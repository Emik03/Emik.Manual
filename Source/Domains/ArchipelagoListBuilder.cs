// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;
#pragma warning disable MA0016
/// <summary>Builds the <see cref="ImmutableArray{T}"/> while allowing implicit conversions.</summary>
/// <typeparam name="T">The type of collection.</typeparam>
[StructLayout(LayoutKind.Auto)]
public readonly partial struct ArchipelagoListBuilder<T> : IList<T>
    where T : IArchipelago<T>
{
    /// <summary>The builder.</summary>
    [ProvidesContext]
    readonly ImmutableArray<T>.Builder _builder;

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    public ArchipelagoListBuilder() => _builder = ImmutableArray.CreateBuilder<T>();

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    /// <param name="item">The item to add.</param>
    public ArchipelagoListBuilder(string item)
        : this((T)item) { }

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    /// <param name="span">The span of elements to populate with.</param>
    public ArchipelagoListBuilder(ReadOnlySpan<string> span)
    {
        _builder = ImmutableArray.CreateBuilder<T>(span.Length);

        foreach (var s in span)
            _builder.Add(s);
    }

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    /// <param name="span">The span of elements to populate with.</param>
    public ArchipelagoListBuilder(ReadOnlySpan<T> span) =>
        (_builder = ImmutableArray.CreateBuilder<T>(span.Length)).AddRange(span);

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    /// <param name="item">The item to add.</param>
    public ArchipelagoListBuilder(T item) => (_builder = ImmutableArray.CreateBuilder<T>(1)).Add(item);

    /// <summary>Initializes a new instance of the <see cref="ArchipelagoListBuilder{T}"/> struct.</summary>
    /// <param name="builder">The builder to use.</param>
    internal ArchipelagoListBuilder(ImmutableArray<T>.Builder builder) => _builder = builder;

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Capacity"/>
    [Pure]
    public int Capacity => _builder.Capacity;

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Count"/>
    [Pure]
    public int Count => _builder.Count;

    /// <inheritdoc cref="ImmutableArray{T}.IsDefault"/>
    [Pure]
    public bool IsDefault => _builder is null;

    /// <inheritdoc cref="ImmutableArray{T}.IsDefaultOrEmpty"/>
    [Pure]
    public bool IsDefaultOrEmpty => IsDefault || IsEmpty;

    /// <inheritdoc cref="ReadOnlySpan{T}.IsEmpty"/>
    [Pure]
    public bool IsEmpty => _builder.Count is 0;

    /// <inheritdoc />
    [Pure]
    bool ICollection<T>.IsReadOnly => false;

    /// <inheritdoc cref="ImmutableArray{T}.Builder.this[int]"/>
    [Pure]
    public T this[int index]
    {
        get => _builder[index];
        set => _builder[index] = value;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.this[int]"/>
    [Pure]
    public T this[Index index]
    {
        get => _builder[index];
        set => _builder[index] = value;
    }

    /// <summary>Implicitly uses the constructor.</summary>
    /// <param name="item">The item to wrap.</param>
    /// <returns>The list.</returns>
    public static implicit operator ArchipelagoListBuilder<T>(string item) => new(item);

    /// <summary>Implicitly uses the constructor.</summary>
    /// <param name="item">The item to wrap.</param>
    /// <returns>The list.</returns>
    public static implicit operator ArchipelagoListBuilder<T>(T item) => new(item);

    /// <summary>Syncs the <see cref="Dictionary{TKey, TValue}"/> and one reference to contain the same value.</summary>
    /// <param name="dictionary">The dictionary to synchronize.</param>
    /// <param name="writer">The reference to synchronize.</param>
    /// <param name="fallback">The fallback priority for when one isn't specified.</param>
    /// <param name="strict">
    /// Whether to throw an exception when the parameter <paramref name="dictionary"/> does not contain the key.
    /// </param>
    /// <exception cref="KeyNotFoundException">
    /// This list contains an element whose name does not exist in the parameter <paramref name="dictionary"/>,
    /// and the parameter <paramref name="strict"/> is <see langword="true"/>.
    /// </exception>
    public static void Sync(
        Dictionary<string, T> dictionary,
        ref T writer,
        Priority fallback = Priority.None,
        bool strict = true
    )
    {
        var name = writer.Name.Span;
        var lookup = dictionary.GetAlternateLookup<ReadOnlySpan<char>>();
        ref var reference = ref CollectionsMarshal.GetValueRefOrAddDefault(lookup, name, out var exists);

        if (exists) // ReSharper disable once NullableWarningSuppressionIsUsed
            writer = reference!;
        else if (strict)
            throw new KeyNotFoundException($"\"{name}\" is referenced before it is defined in {typeof(T).Name}!");
        else if (typeof(T) == typeof(Item))
        {
            ref readonly var item = ref Unsafe.As<T, Item>(ref Unsafe.AsRef(writer));

            writer = reference = Unsafe.BitCast<Item, T>(
                item with
                {
                    Name = item.Name.Replace(World.DisallowedItemChars, strict),
                    Priority = item.Priority is Priority.None ? fallback : item.Priority,
                }
            );
        }
        else
            reference = writer;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add"/>
    public void Add(string item) => Add((T)item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Add"/>
    public void Add(T item) => _builder.Add(item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(IEnumerable{T})"/>
    public void AddRange(IEnumerable<string> items) => AddRange(items.Select(Cast));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(IEnumerable{T})"/>
    public void AddRange(IEnumerable<T> items) => _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T}.Builder)"/>
    public void AddRange(ImmutableArray<string>.Builder items) => AddRange(items.Select(Cast));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T}.Builder)"/>
    public void AddRange(ImmutableArray<T>.Builder items) => _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T})"/>
    public void AddRange(ImmutableArray<string> items) => AddRange(items.AsSpan());

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T})"/>
    public void AddRange(ImmutableArray<T> items) => _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T}, int)"/>
    public void AddRange(ImmutableArray<string> items, int length) => AddRange(items.AsSpan(..length));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ImmutableArray{T}, int)"/>
    public void AddRange(ImmutableArray<T> items, int length) => _builder.AddRange(items, length);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange{TDerived}(ImmutableArray{TDerived})"/>
    public void AddRange<TDerived>(ImmutableArray<TDerived> items)
        where TDerived : T =>
        _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ReadOnlySpan{T})"/>
    public void AddRange(ReadOnlySpan<string> items)
    {
        foreach (var item in items)
            Add(item);
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(ReadOnlySpan{T})"/>
    public void AddRange(ReadOnlySpan<T> items) => _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange{TDerived}(ReadOnlySpan{TDerived})"/>
    public void AddRange<TDerived>(ReadOnlySpan<TDerived> items)
        where TDerived : T =>
        _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange{TDerived}(TDerived[])"/>
    public void AddRange(string[] items) => AddRange(items.Select(Cast));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange{TDerived}(TDerived[])"/>
    public void AddRange<TDerived>(TDerived[] items)
        where TDerived : T =>
        _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(T[], int)"/>
    public void AddRange(string[] items, int length) => AddRange(items.AsSpan(..length));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(T[], int)"/>
    public void AddRange(T[] items, int length) => _builder.AddRange(items, length);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.AddRange(T[])"/>
    public void AddRange(params T[] items) => _builder.AddRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Clear"/>
    public void Clear() => _builder.Clear();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Contains"/>
    [Pure]
    public bool Contains(string item) => Contains((T)item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Contains"/>
    [Pure]
    public bool Contains(T item) => _builder.Contains(item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.CopyTo(Span{T})"/>
    public void CopyTo(Span<T> destination) => _builder.CopyTo(destination);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.CopyTo(T[], int)"/>
    public void CopyTo(T[] array, int index) => _builder.CopyTo(array, index);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.CopyTo(T[])"/>
    public void CopyTo(T[] destination) => _builder.CopyTo(destination);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.CopyTo(int, T[], int, int)"/>
    public void CopyTo(int sourceIndex, T[] destination, int destinationIndex, int length) =>
        _builder.CopyTo(sourceIndex, destination, destinationIndex, length);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T)"/>
    [Pure]
    public int IndexOf(string item) => IndexOf((T)item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T)"/>
    [Pure]
    public int IndexOf(T item) => _builder.IndexOf(item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int)"/>
    [Pure]
    public int IndexOf(string item, int startIndex) => IndexOf((T)item, startIndex);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int)"/>
    [Pure]
    public int IndexOf(T item, int startIndex) => _builder.IndexOf(item, startIndex);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, IEqualityComparer{T})"/>
    [Pure]
    public int IndexOf(string item, int startIndex, IEqualityComparer<T> equalityComparer) =>
        IndexOf((T)item, startIndex, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, IEqualityComparer{T})"/>
    [Pure]
    public int IndexOf(T item, int startIndex, IEqualityComparer<T> equalityComparer) =>
        _builder.IndexOf(item, startIndex, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, int)"/>
    [Pure]
    public int IndexOf(string item, int startIndex, int count) => IndexOf((T)item, startIndex, count);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, int)"/>
    [Pure]
    public int IndexOf(T item, int startIndex, int count) => _builder.IndexOf(item, startIndex, count);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, int, IEqualityComparer{T})"/>
    [Pure]
    public int IndexOf(string item, int startIndex, int count, IEqualityComparer<T> equalityComparer) =>
        IndexOf((T)item, startIndex, count, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.IndexOf(T, int, int, IEqualityComparer{T})"/>
    [Pure]
    public int IndexOf(T item, int startIndex, int count, IEqualityComparer<T> equalityComparer) =>
        _builder.IndexOf(item, startIndex, count, equalityComparer);

    /// <summary>Creates the <see cref="JsonArray"/> of this instance.</summary>
    /// <returns>The <see cref="JsonArray"/> containing values from this instance.</returns>
    [Pure]
    public JsonArray Json()
    {
        JsonArray ret = [];

        for (int i = 0, count = Count; i < count; i++)
            ret.Add(this[i].Name);

        return ret;
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T)"/>
    [Pure]
    public int LastIndexOf(string item) => LastIndexOf((T)item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T)"/>
    [Pure]
    public int LastIndexOf(T item) => _builder.LastIndexOf(item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int)"/>
    [Pure]
    public int LastIndexOf(string item, int startIndex) => LastIndexOf((T)item, startIndex);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int)"/>
    [Pure]
    public int LastIndexOf(T item, int startIndex) => _builder.LastIndexOf(item, startIndex);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int, int)"/>
    [Pure]
    public int LastIndexOf(string item, int startIndex, int count) => LastIndexOf((T)item, startIndex, count);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int, int)"/>
    [Pure]
    public int LastIndexOf(T item, int startIndex, int count) => _builder.LastIndexOf(item, startIndex, count);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int, int, IEqualityComparer{T})"/>
    [Pure]
    public int LastIndexOf(string item, int startIndex, int count, IEqualityComparer<T> equalityComparer) =>
        LastIndexOf((T)item, startIndex, count, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.LastIndexOf(T, int, int, IEqualityComparer{T})"/>
    [Pure]
    public int LastIndexOf(T item, int startIndex, int count, IEqualityComparer<T> equalityComparer) =>
        _builder.LastIndexOf(item, startIndex, count, equalityComparer);

    /// <inheritdoc />
    [Pure]
    public override string ToString() => Json().ToJsonString();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Insert(int, T)"/>
    public void Insert(int index, string item) => Insert(index, (T)item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Insert(int, T)"/>
    public void Insert(int index, T item) => _builder.Insert(index, item);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.InsertRange(int, IEnumerable{T})"/>
    public void InsertRange(int index, IEnumerable<string> items) => InsertRange(index, items.Select(Cast));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.InsertRange(int, IEnumerable{T})"/>
    public void InsertRange(int index, IEnumerable<T> items) => _builder.InsertRange(index, items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.InsertRange(int, ImmutableArray{T})"/>
    public void InsertRange(int index, ImmutableArray<string> items)
    {
        foreach (var item in items)
            Insert(index++, item);
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.InsertRange(int, ImmutableArray{T})"/>
    public void InsertRange(int index, ImmutableArray<T> items) => _builder.InsertRange(index, items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.DrainToImmutable"/>
    [Pure]
    public ImmutableArray<T> DrainToImmutable() => IsDefault ? [] : _builder.DrainToImmutable();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.GetEnumerator"/>
    [Pure]
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.GetEnumerator"/>
    [Pure]
    public IEnumerator<T> GetEnumerator() => _builder.GetEnumerator();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ItemRef"/>
    [Pure]
    public ref readonly T ItemRef(int index) => ref _builder.ItemRef(index);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.MoveToImmutable"/>
    [Pure]
    public ImmutableArray<T> MoveToImmutable() => IsDefault ? [] : _builder.MoveToImmutable();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Sort()"/>
    public void Sort() => _builder.Sort();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Sort(Comparison{T})"/>
    public void Sort(Comparison<T> comparison) => _builder.Sort(comparison);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Sort(IComparer{T})"/>
    public void Sort(IComparer<T> comparer) => _builder.Sort(comparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Sort(int, int, IComparer{T})"/>
    public void Sort(int index, int count, IComparer<T> comparer) => _builder.Sort(index, count, comparer);

    /// <summary>Syncs the <see cref="Dictionary{TKey, TValue}"/> and itself to contain the same values.</summary>
    /// <param name="dictionary">The dictionary to synchronize.</param>
    /// <param name="fallback">The fallback priority for when one isn't specified.</param>
    /// <param name="strict">
    /// Whether to throw an exception when the parameter <paramref name="dictionary"/> does not contain the key.
    /// </param>
    /// <exception cref="KeyNotFoundException">
    /// This list contains an element whose name does not exist in the parameter <paramref name="dictionary"/>,
    /// and the parameter <paramref name="strict"/> is <see langword="true"/>.
    /// </exception>
    public void Sync(Dictionary<string, T> dictionary, Priority fallback = Priority.None, bool strict = true)
    {
        if (IsDefault)
            return;

        for (int i = 0, count = Count; i < count; i++)
            Sync(dictionary, ref Unsafe.AsRef(_builder.ItemRef(i)), fallback, strict);
    }

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Remove(T)"/>
    public bool Remove(string element) => Remove((T)element);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Remove(T)"/>
    public bool Remove(T element) => _builder.Remove(element);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Remove(T, IEqualityComparer{T})"/>
    public bool Remove(string element, IEqualityComparer<T> equalityComparer) => Remove((T)element, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Remove(T, IEqualityComparer{T})"/>
    public bool Remove(T element, IEqualityComparer<T> equalityComparer) => _builder.Remove(element, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveAll"/>
    public void RemoveAll(Predicate<T> match) => _builder.RemoveAll(match);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveAt"/>
    public void RemoveAt(int index) => _builder.RemoveAt(index);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveRange(IEnumerable{T})"/>
    public void RemoveRange(IEnumerable<string> items) => RemoveRange(items.Select(Cast));

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveRange(IEnumerable{T})"/>
    public void RemoveRange(IEnumerable<T> items) => _builder.RemoveRange(items);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveRange(IEnumerable{T}, IEqualityComparer{T})"/>
    public void RemoveRange(IEnumerable<string> items, IEqualityComparer<T> equalityComparer) =>
        RemoveRange(items.Select(Cast), equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.RemoveRange(IEnumerable{T}, IEqualityComparer{T})"/>
    public void RemoveRange(IEnumerable<T> items, IEqualityComparer<T> equalityComparer) =>
        _builder.RemoveRange(items, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Replace(T, T)"/>
    public void Replace(string oldValue, string newValue) => Replace((T)oldValue, (T)newValue);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Replace(T, T)"/>
    public void Replace(T oldValue, T newValue) => _builder.Replace(oldValue, newValue);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Replace(T, T, IEqualityComparer{T})"/>
    public void Replace(string oldValue, string newValue, IEqualityComparer<T> equalityComparer) =>
        Replace((T)oldValue, (T)newValue, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Replace(T, T, IEqualityComparer{T})"/>
    public void Replace(T oldValue, T newValue, IEqualityComparer<T> equalityComparer) =>
        _builder.Replace(oldValue, newValue, equalityComparer);

    /// <inheritdoc cref="ImmutableArray{T}.Builder.Reverse"/>
    public void Reverse() => _builder.Reverse();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToArray"/>
    [Pure]
    public T[] ToArray() => _builder.ToArray();

    /// <inheritdoc cref="ImmutableArray{T}.Builder.ToImmutable"/>
    [Pure]
    public ImmutableArray<T> ToImmutable() => IsDefault ? [] : _builder.ToImmutable();

    /// <summary>Performs the conversion.</summary>
    /// <param name="it">The <see cref="string"/> to convert.</param>
    /// <returns>The converted <typeparamref name="T"/> from the parameter <paramref name="it"/>.</returns>
    [Pure]
    static T Cast(string it) => (T)it;
}
