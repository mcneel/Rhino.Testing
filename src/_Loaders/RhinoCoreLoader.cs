using System;
using System.IO;

using NUnit.Framework;

namespace Rhino.Testing
{
    static class RhinoCoreLoader
    {
        static IDisposable s_core;

        public static void LoadCore()
        {
#if NET7_0_OR_GREATER
            string[] args = new string[] { "/netcore " };
#else
            string[] args = new string[] { "/netfx " };
#endif

            s_core = new Rhino.Runtime.InProcess.RhinoCore(args);
        }

        public static void LoadEto()
        {
            TestContext.WriteLine("Loading eto platform");

            Eto.Platform.AllowReinitialize = true;
            Eto.Platform.Initialize(Eto.Platforms.Wpf);
        }

        public static void DisposeCore()
        {
            (s_core as Rhino.Runtime.InProcess.RhinoCore)?.Dispose();
            s_core = null;
        }
    }
}
