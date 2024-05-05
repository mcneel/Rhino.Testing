using System;
using System.Collections.Generic;

namespace Rhino.Testing
{
    static class GHALoader
    {
        public static void LoadGHA(IEnumerable<string> ghaFiles)
        {
            System.Reflection.MethodInfo loader =
                Grasshopper.Instances.ComponentServer.GetType()
                                     .GetMethod("LoadGHA", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            foreach (string gha in ghaFiles)
            {
                loader.Invoke(
                        Grasshopper.Instances.ComponentServer,
                        new object[] { new Grasshopper.Kernel.GH_ExternalFile(gha), false }
                    );
            }
        }
    }
}
