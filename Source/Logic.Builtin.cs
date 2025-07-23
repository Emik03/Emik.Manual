// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;

public sealed partial class Logic
{
    /// <summary>Represents the bundled function for logic in <see cref="Logic"/>.</summary>
    /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#bundled-functions"/>
    public enum Builtin : byte
    {
        /// <summary>Can the player reach the given location?</summary>
        CanReachLocation,

        /// <summary>Checks if you've collected the specified value of a value-based item.</summary>
        /// <remarks><para>A value-based item is an item that is defined within <see cref="Item.GiveItems"/>.</para></remarks>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#itemvaluevaluenamecount"/>
        ItemValue,

        /// <summary>
        /// Requires an item only if that item exists. Useful if an item might have been disabled by a yaml option.
        /// </summary>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#optoneitemname"/>
        OptOne,

        /// <summary>Takes an entire requires string, and applies the check to each item inside it.</summary>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#optallitemname"/>
        OptAll,

        /// <summary>Checks whether the yaml option is enabled.</summary>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#yamlenabledoption_name-and-yamldisabledoption_name"/>
        YamlEnabled,

        /// <summary>Checks whether the yaml option is disabled.</summary>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#yamlenabledoption_name-and-yamldisabledoption_name"/>
        YamlDisabled,

        /// <summary>Performs a numeric comparison operation on a yaml option.</summary>
        /// <remarks><para>
        /// The corresponding argument string must be in the syntax of <c>option_name comparator_symbol value</c> where
        /// <c>option_name</c> is the option to compare. <c>comparator_symbol</c> is one of <c>==</c>, <c>=</c>, <c>!=</c>,
        /// <c>&gt;=</c>, <c>&lt;=</c>, <c>&gt;</c>, and <c>&lt;</c>. <c>value</c> is any type supporting
        /// <c>option_name</c>, such as a number, <c>Range</c>, <c>NamedRange</c>, <c>Choice</c>, or <c>Toggle</c>.
        /// </para></remarks>
        /// <seealso href="https://github.com/ManualForArchipelago/Manual/blob/main/docs/syntax/requires.md#yamlcompareoption_name-comparator_symbol-value"/>
        YamlCompare,

        /// <summary>Indicates a custom function, which deviates from the base functionality of Manual worlds.</summary>
        Custom,
    }
}
