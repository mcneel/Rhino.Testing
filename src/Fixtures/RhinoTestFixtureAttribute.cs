using System;
using NUnit.Framework;

namespace Rhino.Testing.Fixtures
{

    public sealed class RhinoTestFixtureAttribute : TestFixtureAttribute, IDisposable
    {
        /// <summary>Default Constructor</summary>
        public RhinoTestFixtureAttribute()
        {
            Init();
        }

        /// <summary>Initialises RhinoCore</summary>
        protected static void Init()
        {
            RhinoCore.Initialize();
        }

        public void Dispose()
        {
            RhinoCore.TearDown();
        }

    }

}
