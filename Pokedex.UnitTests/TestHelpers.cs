using System.Net;
using System.Net.Http;
using FluentAssertions;
using Refit;

namespace Pokedex.UnitTests;

public static class TestHelpers
{
    internal static void ShouldBeEquivalent<T>(T actual, T expected)
        => actual.Should().BeEquivalentTo(expected, opts => opts.ComparingByMembers<T>());

    public static ApiResponse<T> BuildApiResponse<T>(HttpStatusCode httpStatusCode, T? response)
        => new(new HttpResponseMessage(httpStatusCode), response!, new RefitSettings());
}
