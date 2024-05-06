using System;
using System.Collections.Generic;

using GH1 = Grasshopper;

namespace Rhino.Testing.Grasshopper
{
    static class GH1Loader
    {
        public static void LoadGHA(IEnumerable<string> ghaFiles)
        {
            System.Reflection.MethodInfo loader =
                GH1.Instances.ComponentServer
                             .GetType()
                             .GetMethod("LoadGHA", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (string gha in ghaFiles)
            {
                loader.Invoke(
                        GH1.Instances.ComponentServer,
                        new object[] { new GH1.Kernel.GH_ExternalFile(gha), false }
                    );
            }
        }
    }
}
