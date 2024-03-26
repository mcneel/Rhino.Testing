using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Rhino.Testing
{
    static class PluginLoader
    {

        public static string GetRHPPath(string rhpName)
        {
            string rhp = Path.Combine(Configs.Current.RhinoSystemDir, "Plug-ins", rhpName);
            if (File.Exists(rhp))
            {
                return rhp;
            }

            rhp = Path.Combine(Path.GetDirectoryName(Configs.Current.RhinoSystemDir), "Plug-ins", rhpName);
            if (File.Exists(rhp))
            {
                return rhp;
            }

            throw new FileNotFoundException(rhpName);
        }

        public static void LoadRDK()
        {
            // NOTE:
            // ensure RDK and its associated native libraries are ready
            // rdk.rhp plugin must be loaded before the rdk native library.
            // a fresh build of rhino on builder machines does not load this
            string rdkRhp = PluginLoader.GetRHPPath("rdk.rhp");
            Rhino.PlugIns.PlugIn.LoadPlugIn(rdkRhp, out Guid _);

            Rhino.Runtime.HostUtils.InitializeRhinoCommon_RDK();
        }

        public static void LoadGrasshopperInRhino()
        {
            Action m = () =>
            {
                TestContext.WriteLine("Loading grasshopper");
                PluginLoader.LoadGrasshopper();
            };

            RhinoApp.InvokeOnUiThread(m);
            return;
        }

        public static void LoadGrasshopper()
        {
            PlugIns.LoadPlugInResult res = 
                PlugIns.PlugIn.LoadPlugIn(GetRHPPath(@"Grasshopper\GrasshopperPlugin.rhp"), out Guid ghId);

            if (PlugIns.LoadPlugInResult.Success == res)
            {
                object ghObj = RhinoApp.GetPlugInObject(ghId);
                if (ghObj?.GetType().GetMethod("RunHeadless") is MethodInfo runHeadLess)
                    runHeadLess.Invoke(ghObj, null);
                else
                    TestContext.WriteLine("Failed loading grasshopper (Headless)");
            }
            else
                TestContext.WriteLine("Failed loading grasshopper plugin");
        }
    }
}
