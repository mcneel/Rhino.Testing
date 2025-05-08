namespace Rhino.Testing.Tests
{
    [SetUpFixture]
    public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
    {
        public override void OneTimeSetup() => base.OneTimeSetup();

        public override void OneTimeTearDown() => base.OneTimeTearDown();
    }
}
