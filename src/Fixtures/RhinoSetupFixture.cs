using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Rhino.Testing.Fixtures
{
    [SetUpFixture]
    public abstract class RhinoSetupFixture
    {
        protected static void LoadGHA(IEnumerable<string> ghaPaths)
        {
            if (ghaPaths is null)
                throw new ArgumentNullException(nameof(ghaPaths));

            Rhino.Testing.Grasshopper.GH1Loader.LoadGHA(ghaPaths);
        }

        protected static Configs Configs => Configs.Current;

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


