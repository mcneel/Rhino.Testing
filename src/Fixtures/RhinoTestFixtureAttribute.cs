using System;
using NUnit.Framework;

namespace Rhino.Testing.Fixtures
{

    /// <summary>
    /// Ensures Rhino is loaded correctly before any tests run, or any test data is loaded.
    /// This attribute does not require you to inherit from the <see cref="RhinoTestFixture"/> class,
    /// nor do you need to create a class with a  <see cref="OneTimeSetUpAttribute"/> method.
    /// </summary>
    public sealed class RhinoTestFixtureAttribute : TestFixtureAttribute, IDisposable
    {
        /// <summary>Default Constructor</summary>
        public RhinoTestFixtureAttribute()
        {
            Init();
        }

        /// <summary>Initialises RhinoCore</summary>
        private static void Init()
        {
            RhinoCore.Initialize();
        }

        public void Dispose()
        {
            RhinoCore.TearDown();
        }

    }

}
