// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Represents the element in the <c>game.json</c> file's <c>starting_items</c> array.</summary>
/// <param name="Set">
/// The set of items or categories to choose from. Will choose any item if <see langword="default"/>.
/// </param>
/// <param name="Amount">
/// How many items of this block will be randomly added to inventory.
/// Will add every item in the block if <see langword="null"/>.
/// </param>
/// <param name="DependsOn">
/// Causes the <see cref="StartingItemBlock"/> to only occur when any of the matching items
/// have already been added to starting inventory by any previous starting item blocks.
/// </param>
/// <param name="Yaml">Names of options that will decide if this block is rolled.</param>
/// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/making/game.md#starting-inventory"/>
[StructLayout(LayoutKind.Auto)]
public readonly partial record struct StartingItemBlock(
    CategoryOrItemListBuilder Set = default,
    int? Amount = null,
    ArchipelagoListBuilder<Item> DependsOn = default,
    ImmutableArray<Yaml> Yaml = default
) : IAddTo
{
    /// <summary>Implicitly wraps the <see cref="Category"/>.</summary>
    /// <param name="category">The item to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    public static implicit operator StartingItemBlock(Category category) => new([category]);

    /// <summary>Implicitly wraps the <see cref="Category"/>.</summary>
    /// <param name="category">The item to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    public static implicit operator StartingItemBlock((Category Category, int Amount) category) =>
        new([category.Category], category.Amount);

    /// <summary>Implicitly wraps the <see cref="Item"/>.</summary>
    /// <param name="item">The item to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    public static implicit operator StartingItemBlock(Item item) => new([item]);

    /// <summary>Implicitly wraps the <see cref="Item"/>.</summary>
    /// <param name="item">The item to wrap.</param>
    /// <returns>The wrapped instance.</returns>
    [Pure]
    public static implicit operator StartingItemBlock((Item Item, int Amount) item) => new([item.Item], item.Amount);

    /// <inheritdoc />
    public void CopyTo(ref JsonNode? value, IReadOnlyCollection<Region>? regions)
    {
        JsonNode obj = new JsonObject();
        Set.CopyTo(ref obj, regions);

        if (Amount is { } choose)
            obj["random"] = choose;

        if (Manual.Yaml.Json(Yaml) is { } options)
            obj["yaml_option"] = options;

        if (!DependsOn.IsDefaultOrEmpty)
            obj["if_previous_item"] = DependsOn.Json();

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
    public override string ToString() =>
        $"[{Set.CategoryBuilder.Select(x => x.Name).Concat(Set.ItemBuilder.Select(x => x.Name)).Conjoin()}]";
}
