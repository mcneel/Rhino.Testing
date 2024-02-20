using System;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace Rhino.Testing
{
    static class PluginLoader
    {
        public static void LoadGrasshopperInRhino()
        {
            Action m = () =>
            {
                TestContext.WriteLine("Loading grasshopper");
                PluginLoader.LoadGrasshopper();
            };

            Rhino.RhinoApp.InvokeOnUiThread(m);
            return;
        }


        public static void LoadGrasshopper()
        {
            Rhino.PlugIns.PlugIn.LoadPlugIn(GetRHPPath(@"Grasshopper\GrasshopperPlugin.rhp"), out Guid _);

            object ghObj = Rhino.RhinoApp.GetPlugInObject("Grasshopper");
            if (ghObj?.GetType().GetMethod("RunHeadless") is MethodInfo runHeadLess)
                runHeadLess.Invoke(ghObj, null);
            else
                TestContext.WriteLine("Failed loading grasshopper (Headless)");
        }

        static string GetRHPPath(string rhpName)
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
    }
}
