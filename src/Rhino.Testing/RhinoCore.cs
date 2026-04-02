using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using NUnit.Framework;

namespace Rhino.Testing
{
    // https://docs.nunit.org/articles/vs-test-adapter/AdapterV4-Release-Notes.html
    // https://github.com/nunit/nunit3-vs-adapter/blob/master/src/NUnitTestAdapter/AdapterSettings.cs#L143
    static class RhinoCore
    {
        static bool s_initd;
        static bool s_inRhino;

        public static void Initialize()
        {
            if (s_initd)
            {
                return;
            }

            s_inRhino = Process.GetCurrentProcess().ProcessName.Equals("Rhino", StringComparison.OrdinalIgnoreCase);
            if (s_inRhino)
            {
                TestContext.WriteLine("Configuring rhino process");
                PluginLoader.LoadGrasshopperInRhino();
                s_initd = true;
                return;
            }

            if (!Directory.Exists(Configs.Current.RhinoSystemDir))
            {
                throw new DirectoryNotFoundException(Configs.Current.RhinoSystemDir);
            }

            //add the Package directories to the RHINO_PACKAGE_DIRS environment variable so that RhinoInside can find the plugins in those directories
            foreach (var packageDir in Configs.Current.PackageDirectories)
            {
                var env = Environment.GetEnvironmentVariable("RHINO_PACKAGE_DIRS", EnvironmentVariableTarget.Process);
                env = string.IsNullOrEmpty(env) ? packageDir.Path : $"{env};{packageDir.Path}";
                Environment.SetEnvironmentVariable("RHINO_PACKAGE_DIRS", env, EnvironmentVariableTarget.Process);
            }

            RhinoInside.Resolver.Initialize(Configs.Current.RhinoSystemDir);
            Configs.Current.RhinoSystemDir = RhinoInside.Resolver.RhinoSystemDirectory;

            TestContext.WriteLine("Loading rhino core");
            RhinoCoreLoader.LoadCore(
                createDoc: Configs.Current.CreateRhinoDoc,
                createView: Configs.Current.CreateRhinoView
                );

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadEto)
            {
                RhinoCoreLoader.LoadEto();
            }

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadRDK)
            {
                PluginLoader.LoadRDK();
            }

            if (Configs.Current.LoadLegacyIronPython)
            {
                PluginLoader.LoadLegacyIronPython();
            }

            if (Configs.Current.LoadPlugins.Count != 0)
            {
                PluginLoader.LoadPlugins(Configs.Current.LoadPlugins.Select(p => p.Location), Configs.Current.PackageDirectories.Select(d => d.Path));
            }

            if (Configs.Current.LoadGrasshopper)
            {
                PluginLoader.LoadGrasshopper();
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


