using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

using NUnit.Framework;
using System.Linq;

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

            InitNativeResolver();
            AppDomain.CurrentDomain.AssemblyResolve += ManagedAssemblyResolver;

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
                PluginLoader.LoadPlugins(Configs.Current.LoadPlugins.Select(p => p.Location));
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

        static Assembly ManagedAssemblyResolver(object sender, ResolveEventArgs args)
        {
            string name = new AssemblyName(args.Name).Name;

            if (name.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
            {
                return default;
            }

            foreach (string path in GetSearchPaths())
            {
                if (TryLoadAssembly(path, name, out Assembly loaded))
                {
                    return loaded;
                }
            }

            TestContext.WriteLine($"Skip resolving assembly {name}");
            return null;
        }

        public static IEnumerable<string> GetSearchPaths()
        {
            // rhino assemblies and plugins
#if NET7_0_OR_GREATER
            yield return Path.Combine(Configs.Current.RhinoSystemDir, "netcore");
#else
            yield return Path.Combine(Configs.Current.RhinoSystemDir, "netfx");
#endif

            yield return Configs.Current.RhinoSystemDir;

            foreach (var path in PluginLoader.GetPluginSearchPaths())
            {
                // Grasshopper.dll is here
                yield return Path.Combine(path, @"Grasshopper");

                // RhinoCodePluginGH is here
                yield return Path.Combine(path, @"Grasshopper\Components");
            }

            yield return Path.GetDirectoryName(typeof(RhinoCore).Assembly.Location);
        }

        static bool TryLoadAssembly(string path, string name, out Assembly assembly)
        {
            assembly = default;

            string file = Path.Combine(path, name + ".dll");
            if (File.Exists(file))
            {
                TestContext.WriteLine($"Loading assembly from file {file}");
                assembly = Assembly.LoadFrom(file);
                return true;
            }

            file = Path.ChangeExtension(file, ".rhp");
            if (File.Exists(file))
            {
                TestContext.WriteLine($"Loading plugin assembly from file {file}");
                assembly = Assembly.LoadFrom(file);
                return true;
            }

            file = Path.ChangeExtension(file, ".gha");
            if (File.Exists(file))
            {
                TestContext.WriteLine($"Loading grasshopper plugin assembly from file {file}");
                assembly = Assembly.LoadFrom(file);
                return true;
            }

            return false;
        }

#if NET7_0_OR_GREATER
        // FIXME: Rhino.Runtime.InProcess.RhinoCore should take care of this 
        static System.Runtime.Loader.AssemblyLoadContext s_context;

#pragma warning disable IDE0090 // Use 'new(...)'
        static readonly ConcurrentDictionary<string, IntPtr> s_nativeCache = new ConcurrentDictionary<string, IntPtr>();
#pragma warning restore IDE0090 // Use 'new(...)'

        static void InitNativeResolver()
        {
            AppDomain.CurrentDomain.AssemblyLoad += ManageAssemblyLoaded;

            Assembly assembly = typeof(RhinoCore).Assembly;
            s_context = System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(assembly);
            s_context.ResolvingUnmanagedDll += ResolvingUnmanagedDll;
        }

        static void ManageAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            Assembly assembly = args.LoadedAssembly;

            if (assembly.IsDynamic
                    || System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(assembly) == s_context)
            {
                return;
            }

            NativeLibrary.SetDllImportResolver(assembly, NativeAssemblyResolver);
        }

        static IntPtr ResolvingUnmanagedDll(Assembly assembly, string name)
        {
            return NativeAssemblyResolver(name, assembly, null);
        }

        static IntPtr NativeAssemblyResolver(string libname, Assembly assembly, DllImportSearchPath? searchPath)
        {
            if (s_nativeCache.TryGetValue(libname, out var ptr))
            {
                return ptr;
            }

            TestContext.WriteLine($"Resolving {libname}... ");

            foreach (string path in new List<string>
            {
                assembly.Location,
                Path.Combine(Configs.Current.RhinoSystemDir, "netcore"),
                Configs.Current.RhinoSystemDir,
            })
            {
                var file = Path.Combine(path, libname + ".dll");
                if (File.Exists(file))
                {
                    ptr = NativeLibrary.Load(file);
                    s_nativeCache[libname] = ptr;
                    return ptr;
                }
            }

            s_nativeCache[libname] = IntPtr.Zero;
            TestContext.WriteLine($"no mapping found");
            return IntPtr.Zero;
        }

#else
        static void InitNativeResolver() { }
#endif
    }
}


