// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual.Domains;

/// <summary>Contains operations to create <see cref="Manual.Logic"/> instances that contain itself.</summary>
/// <typeparam name="T">The type that is implementing this interface.</typeparam>
public interface ILogicNode<T>
    where T : ILogicNode<T>
{
    /// <summary>Gets itself as a <see cref="Manual.Logic"/> requirement.</summary>
    [Pure]
    public Logic Logic { get; }

    /// <summary>Gets the <see cref="Manual.Logic"/> requirement that this instance directly contains.</summary>
    [Pure]
    public Logic? SelfLogic => null;

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static abstract Logic operator &(Logic? left, in T right);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static abstract Logic operator &(in T left, Logic? right);

    /// <inheritdoc cref="Logic.op_BitwiseAnd"/>
    [Pure]
    public static abstract Logic operator &(in T left, in T right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static abstract Logic operator |(Logic? left, in T right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static abstract Logic operator |(in T left, Logic? right);

    /// <inheritdoc cref="Logic.op_BitwiseOr"/>
    [Pure]
    public static abstract Logic operator |(in T left, in T right);
}
