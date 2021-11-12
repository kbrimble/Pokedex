using FluentAssertions;

namespace Pokedex.UnitTests;

public static class TestHelpers
{
    internal static void ShouldBeEquivalent<T>(T actual, T expected)
        => actual.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<T>());
}
