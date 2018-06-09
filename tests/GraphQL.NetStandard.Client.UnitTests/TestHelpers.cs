using FluentAssertions;

namespace GraphQL.NetStandard.Client.UnitTests
{
    public static class TestHelpers
    {
        public static bool AreEqual<T>(T object1, T object2)
        {
            object1.ShouldBeEquivalentTo(object2);

            return true;
        }
    }
}
