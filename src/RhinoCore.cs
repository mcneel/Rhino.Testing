using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Collections.Concurrent;

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

            InitNativeResolver();
            AppDomain.CurrentDomain.AssemblyResolve += ManagedAssemblyResolver;

            TestContext.WriteLine("Loading rhino core");
            RhinoCoreLoader.LoadCore();

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadRDK)
            {
                TestContext.WriteLine("Loading rdk");
                RhinoCoreLoader.LoadRDK();
            }

            if (Configs.Current.LoadGrasshopper || Configs.Current.LoadEto)
            {
                TestContext.WriteLine("Loading eto platform");
                RhinoCoreLoader.LoadEto();
            }

            if (Configs.Current.LoadGrasshopper)
            {
                TestContext.WriteLine("Loading grasshopper");
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

            foreach (string path in new List<string>
            {
#if NET7_0_OR_GREATER
                Path.Combine(Configs.Current.RhinoSystemDir, "netcore"),
#else
                Path.Combine(Configs.Current.RhinoSystemDir, "netfx"),
#endif
                Configs.Current.RhinoSystemDir,
                typeof(RhinoCore).Assembly.Location,
                Path.Combine(Configs.Current.RhinoSystemDir, @"Plug-ins\Grasshopper"),
            })
            {
                string file = Path.Combine(path, name + ".dll");
                if (File.Exists(file))
                {
                    TestContext.WriteLine($"Loading assembly from file {file}");
                    return Assembly.LoadFrom(file);
                }
            }

            TestContext.WriteLine($"Could not find assembly {name}");
            return null;
        }

#if NET7_0_OR_GREATER
        // FIXME: Rhino.Runtime.InProcess.RhinoCore should take care of this 
        static System.Runtime.Loader.AssemblyLoadContext s_context;

        static readonly ConcurrentDictionary<string, IntPtr> s_nativeCache = new ConcurrentDictionary<string, IntPtr>();

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


