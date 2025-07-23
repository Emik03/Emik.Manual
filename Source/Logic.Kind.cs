// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;

public sealed partial class Logic
{
    /// <summary>The kind.</summary>
    public enum Kind : byte
    {
        /// <summary>
        /// Both <see cref="Left"/> and <see cref="Right"/> must be fulfilled for this node to be fulfilled.
        /// </summary>
        And,

        /// <summary>
        /// Either <see cref="Left"/> and <see cref="Right"/> must be fulfilled for this node to be fulfilled.
        /// </summary>
        Or,

        /// <summary>A particular <see cref="Item"/> must be obtained for this node to be fulfilled.</summary>
        Item,

        /// <summary>
        /// A particular amount of an <see cref="Item"/> must be obtained for this node to be fulfilled.
        /// </summary>
        ItemCount,

        /// <summary>
        /// A particular percentage of an <see cref="Item"/> must be obtained for this node to be fulfilled.
        /// </summary>
        ItemPercent,

        /// <summary>A particular <see cref="Category"/> must be obtained for this node to be fulfilled.</summary>
        Category,

        /// <summary>
        /// A particular amount of an <see cref="Category"/> must be obtained for this node to be fulfilled.
        /// </summary>
        CategoryCount,

        /// <summary>
        /// A particular percentage of an <see cref="Category"/> must be obtained for this node to be fulfilled.
        /// </summary>
        CategoryPercent,

        /// <summary>A particular <see cref="Region"/> must be reachable for this node to be fulfilled.</summary>
        Region,

        /// <summary>A particular <see cref="Location"/> must be reachable for this node to be fulfilled.</summary>
        Location,

        /// <summary>A particular amount of a phantom item must be obtained for this node to be fulfilled.</summary>
        ItemValue,

        /// <summary>
        /// A particular <see cref="Item"/> must be obtained or not exist for this node to be fulfilled.
        /// </summary>
        OptOne,

        /// <summary><see cref="Left"/> must be fulfilled or not exist for this node to be fulfilled.</summary>
        OptAll,

        /// <summary>The option must be enabled for this node to be fulfilled.</summary>
        YamlEnabled,

        /// <summary>The option must be disabled for this node to be fulfilled.</summary>
        YamlDisabled,

        /// <summary>The option must be equal to a particular value for this node to be fulfilled.</summary>
        YamlEqual,

        /// <summary>The option must not be equal to a particular value for this node to be fulfilled.</summary>
        YamlNotEqual,

        /// <summary>The option must be greater than a particular value for this node to be fulfilled.</summary>
        YamlGreaterThan,

        /// <summary>The option must be greater or equal a particular value for this node to be fulfilled.</summary>
        YamlGreaterThanOrEqual,

        /// <summary>The option must be less than a particular value for this node to be fulfilled.</summary>
        YamlLessThan,

        /// <summary>The option must be less or equal a particular value for this node to be fulfilled.</summary>
        YamlLessThanOrEqual,

        /// <summary>Represents a custom function. Functionality will vary from world-to-world.</summary>
        Custom,
    }
}
