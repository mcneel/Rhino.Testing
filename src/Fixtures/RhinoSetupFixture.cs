using System;

using NUnit.Framework;

namespace Rhino.Testing.Fixtures
{
    [SetUpFixture]
    public abstract class RhinoSetupFixture
    {
        [OneTimeSetUp]
        public virtual void OneTimeSetup()
        {
            RhinoCore.Initialize();
        }

        [OneTimeTearDown]
        public virtual void OneTimeTearDown()
        {
            RhinoCore.TearDown();
        }
    }
}


