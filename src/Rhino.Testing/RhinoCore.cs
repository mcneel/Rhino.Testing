using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using NUnit.Framework;

namespace Rhino.Testing
{
    // https://docs.nunit.org/articles/vs-test-adapter/AdapterV4-Release-Notes.html
    // https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/AdapterSettings.cs#L143
    static class RhinoCore
    {
        static bool s_initd;
        static bool s_inRhino;

        static string FirstExistingSystemDir(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            foreach (string entry in value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string path = entry.Trim();
                if (Directory.Exists(path))
                    return path;
            }

            return null;
        }

        public static void Initialize()
        {
            if (s_initd)
            {
                return;
            }
            var rhinoProcessName = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "Rhinoceros" : "Rhino";
            s_inRhino = Process.GetCurrentProcess().ProcessName.Equals(rhinoProcessName, StringComparison.OrdinalIgnoreCase);
            if (s_inRhino)
            {
                TestContext.WriteLine("Configuring rhino process");
                PluginLoader.LoadGrasshopperInRhino();
                s_initd = true;
                return;
            }

            // Do not require the RhinoSystemDir to be set, as the RhinoInside resolver can find Rhino in the "vanilla" installation locations.
            string rhinoDir = FirstExistingSystemDir(Configs.Current.RhinoSystemDir);
            if (string.IsNullOrEmpty(rhinoDir))
                RhinoInside.Resolver.Initialize();
            else
                RhinoInside.Resolver.Initialize(rhinoDir);

            Configs.Current.RhinoSystemDir = RhinoInside.Resolver.RhinoSystemDirectory;

            TestContext.WriteLine("Loading rhino core");
            RhinoCoreLoader.LoadCore(
                createDoc: Configs.Current.CreateRhinoDoc,
                createView: Configs.Current.CreateRhinoView
                );

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadGrasshopper2 || Configs.Current.LoadEto)
            {
                RhinoCoreLoader.LoadEto();
            }

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadGrasshopper2 || Configs.Current.LoadRDK)
            {
                PluginLoader.LoadRDK();
            }

            if (Configs.Current.LoadLegacyIronPython)
            {
                PluginLoader.LoadLegacyIronPython();
            }

            if (Configs.Current.LoadPlugins.Count != 0)
            {
                PluginLoader.LoadPlugins(Configs.Current.LoadPlugins.Select(p => p.Location));
            }

            if (Configs.Current.LoadGrasshopper)
            {
                PluginLoader.LoadGrasshopper();
            }

            if (Configs.Current.LoadGrasshopper2)
            {
                PluginLoader.LoadGrasshopper2();
            }

            s_initd = true;
        }

        public static void TearDown()
        {
            RhinoCoreLoader.DisposeCore();
            s_inRhino = false;
            s_initd = false;
        }
    }
}


