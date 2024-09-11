using NUnit.Framework;

[SetUpFixture]
public sealed class SetupFixture : Rhino.Testing.Fixtures.RhinoSetupFixture
{
    public override void OneTimeSetup()
    {
        base.OneTimeSetup();

        LoadGHA(new string[] {
            /*** Add your plugins here
              @"/path/to/gh1plugins/FirstTestPlugin.gha",
              @"/path/to/gh1plugins/SecondTestPlugin.gha",
            */
            });

        // your custom setup
    }

    public override void OneTimeTearDown()
    {
        base.OneTimeTearDown();

        // you custom teardown
    }
}
