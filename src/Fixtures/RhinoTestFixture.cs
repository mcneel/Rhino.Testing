using System;

using NUnit.Framework;

namespace Rhino.Testing.Fixtures
{
    [TestFixture]
    public abstract class RhinoTestFixture
    {
        protected static Configs Configs => Configs.Current;
    }
}
