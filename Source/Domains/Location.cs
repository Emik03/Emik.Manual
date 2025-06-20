// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents the element in the <c>locations.json</c> file.</summary>
/// <param name="Name">The unique name of the location.</param>
/// <param name="Logic">
/// A boolean logic object that describes the required items, counts, etc. needed to reach this location.
/// </param>
/// <param name="Categories">A list of categories to be applied to this location.</param>
/// <param name="Region">The name of the region this location is part of.</param>
/// <param name="Options">Additional boolean configurations for this location.</param>
/// <param name="CategoryAllowList">
/// Places an item that matches at least one of the categories listed in this
/// setting at this location. Does not check logical access to the location.
/// </param>
/// <param name="CategoryDenyList">
/// Configures what item categories should not end up at this location during
/// normal generation. Does not check logical access to the location.
/// </param>
/// <param name="ItemAllowList">
/// Places an item that matches one of the item names listed in this setting
/// at this location. Does not check logical access to the location.
/// </param>
/// <param name="ItemDenyList">
/// Configures what item names should not end up at this location during
/// normal generation. Does not check logical access to the location.
/// </param>
/// <param name="HintEntrance">
/// Adds additional text to this location's hints to convey useful
/// information. Typically used for entrance randomization.
/// </param>
/// <param name="Id">
/// Skips the item ID forward to the given value. This can be used to provide buffer space for future items.
/// </param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/locations.md"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct Location(
    Chars Name,
    Logic? Logic = null,
    ImmutableArray<Category> Categories = default,
    Region Region = default,
    LocationOptions Options = LocationOptions.None,
    ImmutableArray<Category> CategoryAllowList = default,
    ImmutableArray<Category> CategoryDenyList = default,
    ImmutableArray<Item> ItemAllowList = default,
    ImmutableArray<Item> ItemDenyList = default,
    Chars HintEntrance = default,
    int? Id = null
) : IAddTo, IArchipelago<Location>
{
    /// <inheritdoc />
    [Pure]
    public static implicit operator Location(string? name) => new(name.AsMemory());

    /// <inheritdoc />
    [Pure]
    public static implicit operator Location(ReadOnlyMemory<char> name) => new(name);

    /// <inheritdoc />
    // ReSharper disable once CognitiveComplexity
    void IAddTo.CopyTo([NotNullIfNotNull(nameof(value))] ref JsonNode? value)
    {
        JsonObject obj = new() { ["name"] = Name.ToString() };

        if (Id is { } id)
            obj["id"] = id;

        if (!Categories.IsDefaultOrEmpty)
            obj["category"] = IArchipelago<Category>.Json(Categories);

        if (Logic is not null)
            obj["requires"] = Logic.ToString();

        if (!HintEntrance.Memory.IsEmpty)
            obj["hint_entrance"] = HintEntrance.ToString();

        if (Region is { Name: { IsEmpty: false } name })
            obj["region"] = name.ToString();

        if (Options.Has(LocationOptions.Victory))
            obj["victory"] = true;

        if (Options.Has(LocationOptions.PreHint))
            obj["prehint"] = true;

        if (!ItemAllowList.IsDefaultOrEmpty)
            obj["place_item"] = IArchipelago<Item>.Json(ItemAllowList);

        if (!ItemDenyList.IsDefaultOrEmpty)
            obj["dont_place_item"] = IArchipelago<Item>.Json(ItemDenyList);

        if (!CategoryAllowList.IsDefaultOrEmpty)
            obj["place_item_category"] = IArchipelago<Category>.Json(CategoryAllowList);

        if (!CategoryDenyList.IsDefaultOrEmpty)
            obj["dont_place_item_category"] = IArchipelago<Category>.Json(CategoryDenyList);

        switch (value)
        {
            case JsonObject:
                value = obj;
                return;
            case JsonArray it:
                it.Add(obj);
                break;
            default:
                value = new JsonArray(obj);
                break;
        }
    }

    /// <inheritdoc />
    [Pure]
    public override string ToString() => IAddTo.ToJsonString(this);
}
