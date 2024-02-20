using System;
using System.IO;

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


            // NOTE:
            // ensure RDK and its associated native libraries are ready
            // rdk.rhp plugin must be loaded before the rdk native library.
            // a fresh build of rhino on builder machines does not load this
            string rdkRhp = Path.Combine(Configs.Current.RhinoSystemDir, "Plug-ins", "rdk.rhp");
            Rhino.PlugIns.PlugIn.LoadPlugIn(rdkRhp, out Guid _);

            Rhino.Runtime.HostUtils.InitializeRhinoCommon_RDK();
        }

        public static void LoadEto()
        {
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
