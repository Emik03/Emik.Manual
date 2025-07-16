// SPDX-License-Identifier: MPL-2.0
namespace Emik.Manual;

/// <summary>Provides the <see cref="Check"/> method.</summary>
static class LogicMarker
{
    /// <summary>Returns itself, discarding the parameter. Prints both if compiled with a specific constant.</summary>
    /// <param name="left">The logic to return.</param>
    /// <param name="right">The logic to discard.</param>
    /// <param name="line">The caller line number to discard.</param>
    /// <returns>Itself.</returns>
    public static Logic? Check(this Logic? left, [UsedImplicitly] Logic? right, [CallerLineNumber] int line = 0)
    {
#if LOGIC_TRIM_CONSOLE_WRITE
        Console.WriteLine(
            $"""
             Found optimization: {line}
             Preserving: {left}
             Discarding: {right}

             """
        );
#endif
        left?.IsOptimized = true;
        return left;
    }

    /// <summary>Determines whether logic contains structurally the same data.</summary>
    /// <param name="left">The logic to compare from.</param>
    /// <param name="right">The logic to compare to.</param>
    /// <returns>Whether both instances are equal.</returns>
    public static bool Commutative(this Logic? left, Logic? right) =>
        left == right ||
        left?.Binary is ({ } ll, { } lr) && // Commutative Law
        right?.Binary is ({ } rl, { } rr) &&
        left.IsAnd == right.IsAnd &&
        (ll.Commutative(rl) && lr.Commutative(rr) || ll.Commutative(rr) && lr.Commutative(rl));
}
