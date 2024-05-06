using System;

using NUnit.Framework;

using Rhino.Testing.Grasshopper;

namespace Rhino.Testing.Fixtures
{
    [TestFixture]
    public abstract class RhinoTestFixture
    {
        protected static Configs Configs => Configs.Current;

        protected static void TestGrasshopper(string ghFile, out bool result, out GHReport report, GHMessageLevel reportLevel = GHMessageLevel.Error)
        {
            GH1Runner.TestGrasshopper(ghFile, out result, out report, reportLevel);
        }
    }
}
