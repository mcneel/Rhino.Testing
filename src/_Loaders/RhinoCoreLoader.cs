using System;
using System.IO;

using NUnit.Framework;

namespace Rhino.Testing
{
    static class RhinoCoreLoader
    {
        const string VIEWPORT_TITLE = "RhinoTestingViewport";

        static IDisposable s_core;
        static IDisposable s_doc;

        public static void LoadCore(bool createDoc, bool createView)
        {
#if NET7_0_OR_GREATER
            string[] args = new string[] { "/netcore " };
#else
            string[] args = new string[] { "/netfx " };
#endif

            s_core = new Rhino.Runtime.InProcess.RhinoCore(args);

            if (createDoc)
            {
                // set this view as active doc
                // this specifically helps with rhinoscriptsyntax calls
                // that use scriptcontext.doc
                Rhino.RhinoDoc.ActiveDoc = Rhino.RhinoDoc.CreateHeadless(string.Empty);

                if (createView)
                {
                    Display.RhinoView view = 
                        Rhino.RhinoDoc.ActiveDoc.Views.Add(
                                VIEWPORT_TITLE,
                                Display.DefinedViewportProjection.Top,
                                new System.Drawing.Rectangle(0, 0, 100, 100),
                                floating: false
                            );

                    // set this view as active view
                    // this specifically helps with rhinoscriptsyntax calls
                    // that use scriptcontext.doc.Views.ActiveView.ActiveViewport
                    Rhino.RhinoDoc.ActiveDoc.Views.ActiveView = view;
                }

                s_doc = Rhino.RhinoDoc.ActiveDoc;                
            }
        }

        public static void LoadEto()
        {
            TestContext.WriteLine("Loading eto platform");

            Eto.Platform.AllowReinitialize = true;
            Eto.Platform.Initialize(Eto.Platforms.Wpf);
        }

        public static void DisposeCore()
        {
            (s_core as Rhino.Runtime.InProcess.RhinoCore)?.Dispose();
            (s_doc as Rhino.RhinoDoc)?.Dispose();

            s_core = default;
            s_doc = default;
        }
    }
}
