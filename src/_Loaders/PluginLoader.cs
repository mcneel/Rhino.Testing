using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Rhino.Testing
{
    static class PluginLoader
    {
        public static IEnumerable<string> GetPluginSearchPaths()
        {
            const string PLUGINS = "Plug-ins";

            yield return Path.Combine(Configs.Current.RhinoSystemDir, PLUGINS);
            yield return Path.Combine(Path.GetDirectoryName(Configs.Current.RhinoSystemDir), PLUGINS);
        }

        public static string GetRHPPath(string rhpPath)
        {
            string rhp = Path.Combine(Configs.Current.RhinoSystemDir, rhpPath);
            if (File.Exists(rhp))
            {
                return rhp;
            }

            rhp = Path.Combine(Path.GetDirectoryName(Configs.Current.RhinoSystemDir), rhpPath);
            if (File.Exists(rhp))
            {
                return rhp;
            }

            throw new FileNotFoundException(rhpPath);
        }

        public static void LoadRDK()
        {
            TestContext.WriteLine("Loading rdk");

            // NOTE:
            // ensure RDK and its associated native libraries are ready
            // rdk.rhp plugin must be loaded before the rdk native library.
            // a fresh build of rhino on builder machines does not load this
            string rdkRhp = GetRHPPath(@"Plug-ins\rdk.rhp");
            Rhino.PlugIns.PlugIn.LoadPlugIn(rdkRhp, out Guid _);

            Rhino.Runtime.HostUtils.InitializeRhinoCommon_RDK();
        }

        public static void LoadGrasshopperInRhino()
        {
            Action m = () =>
            {
                TestContext.WriteLine("Loading grasshopper");
                LoadGrasshopper();
            };

            RhinoApp.InvokeOnUiThread(m);
            return;
        }

        public static void LoadLegacyIronPython()
        {
            TestContext.WriteLine("Loading legacy ironpython");

            PlugIns.LoadPlugInResult res =
                PlugIns.PlugIn.LoadPlugIn(GetRHPPath(@"Plug-ins\IronPython\RhinoDLR_Python.rhp"), out Guid _);

            if (PlugIns.LoadPlugInResult.Success != res)
                TestContext.WriteLine("Failed loading legacy ironpython plugin");
        }

        public static void LoadGrasshopper()
        {
            TestContext.WriteLine("Loading grasshopper");

            PlugIns.LoadPlugInResult res =
                PlugIns.PlugIn.LoadPlugIn(GetRHPPath(@"Plug-ins\Grasshopper\GrasshopperPlugin.rhp"), out Guid ghId);

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

        public static void LoadPlugins(IEnumerable<string> rhpPaths)
        {
            foreach (var rhpPath in rhpPaths)
            {
                string fullPath = GetRHPPath(rhpPath);

                TestContext.WriteLine($"Loading plugin from {fullPath}");

                if (PlugIns.PlugIn.LoadPlugIn(fullPath, out Guid _) 
                            != PlugIns.LoadPlugInResult.Success)
                {
                    TestContext.WriteLine("Failed loading legacy ironpython plugin");
                }
            }
        }
    }
}
