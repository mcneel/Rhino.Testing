using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

using RhinoInside;
using NUnit.Framework;

namespace Rhino.Testing
{
    static class PluginLoader
    {
        public static IEnumerable<string> GetPluginSearchPaths()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                const string MANAGED_PLUGINS = "RhCore.framework/Versions/A/Resources/ManagedPlugIns";
                yield return Path.Combine(Configs.Current.RhinoSystemDir, MANAGED_PLUGINS);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                const string PLUGINS = "Plug-ins";
                yield return Path.Combine(Configs.Current.RhinoSystemDir, PLUGINS);
                yield return Path.Combine(Path.GetDirectoryName(Configs.Current.RhinoSystemDir), PLUGINS);
            }
        }

        public static string GetRHPPath(string rhpPath)
        {
            string rhp = Path.Combine(Configs.Current.RhinoSystemDir, rhpPath);
            if (File.Exists(rhp))
            {
                return rhp;
            }

            // on macOS plugins can be bundles
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (Directory.Exists(rhp))
                {
                    return rhp;
                }
            }

            // On Rhino-Windows, walk back up from System path to find plugin path
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rhp = Path.Combine(Path.GetDirectoryName(Configs.Current.RhinoSystemDir), rhpPath);
                if (File.Exists(rhp))
                {
                    return rhp;
                }
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
            string rdkRhp;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rdkRhp = "RhCommonRDK.framework/Versions/A/RhCommonRDK";
                Rhino.PlugIns.PlugIn.LoadPlugIn(rdkRhp, out Guid _);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rdkRhp = GetRHPPath(@"Plug-ins\rdk.rhp");
                Rhino.PlugIns.PlugIn.LoadPlugIn(rdkRhp, out Guid _);
            }
            else
                throw new RhinoInsideInitializationException("Failed loading rdk plugin");

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

            string rhpPath;
            PlugIns.LoadPlugInResult res = PlugIns.LoadPlugInResult.ErrorUnknown;
            Guid rpyId = Guid.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rhpPath = GetRHPPath("RhCore.framework/Versions/A/Resources/ManagedPlugIns/RhinoDLR_Python.rhp/RhinoDLR_Python.rhp");
                res = PlugIns.PlugIn.LoadPlugIn(rhpPath, out rpyId);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rhpPath = GetRHPPath(@"Plug-ins\IronPython\RhinoDLR_Python.rhp");
                res = PlugIns.PlugIn.LoadPlugIn(rhpPath, out rpyId);
            }

            if (Guid.Empty == rpyId)
            {
                throw new RhinoInsideInitializationException("Failed loading legacy ironpython plugin (missing plugin id)");
            }

            if (PlugIns.LoadPlugInResult.Success != res)
            {
                throw new RhinoInsideInitializationException("Failed loading legacy ironpython plugin");
            }
        }

        public static void LoadGrasshopper()
        {
            TestContext.WriteLine("Loading grasshopper");

            string rhpPath;
            PlugIns.LoadPlugInResult res = PlugIns.LoadPlugInResult.ErrorUnknown;
            Guid ghId = Guid.Empty;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                rhpPath = GetRHPPath("RhCore.framework/Versions/A/Resources/ManagedPlugIns/GrasshopperPlugin.rhp/GrasshopperPlugin.rhp");
                res = PlugIns.PlugIn.LoadPlugIn(rhpPath, out ghId);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                rhpPath = GetRHPPath(@"Plug-ins\Grasshopper\GrasshopperPlugin.rhp");
                res = PlugIns.PlugIn.LoadPlugIn(rhpPath, out ghId);
            }

            if (Guid.Empty == ghId)
            {
                throw new RhinoInsideInitializationException("Failed loading grasshopper plugin (missing plugin id)");
            }

            if (PlugIns.LoadPlugInResult.Success == res)
            {
                object ghObj = RhinoApp.GetPlugInObject(ghId);
                if (ghObj is null)
                    throw new RhinoInsideInitializationException("Failed getting grasshopper plugin instance");

                if (ghObj?.GetType().GetMethod("RunHeadless") is MethodInfo runHeadLess)
                    runHeadLess.Invoke(ghObj, null);
                else
                    throw new RhinoInsideInitializationException("Failed loading grasshopper (Headless)");
            }
            else
            {
                throw new RhinoInsideInitializationException("Failed loading grasshopper plugin");
            }
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
                    throw new RhinoInsideInitializationException($"Failed loading plugin: {fullPath}");
                }
            }
        }
    }
}
