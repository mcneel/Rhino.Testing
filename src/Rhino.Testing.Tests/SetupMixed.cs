using Rhino.Geometry;
using Rhino.Testing.Fixtures;

namespace Rhino.Testing.Tests
{
    [RhinoTestFixture]
    public class SetupFromAttributeTest : RhinoTestFixture
    {
        [Test]
        public void Test() => Assert.True(true);
    }

    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup() => base.OneTimeSetup();

        public override void OneTimeTearDown() => base.OneTimeTearDown();
    }

    public class SetupFromFixtureTest : RhinoTestFixture
    {
        [Test]
        public void Test() => Assert.True(true);
    }
}
