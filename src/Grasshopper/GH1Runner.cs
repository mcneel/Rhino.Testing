using System;
using System.Linq;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace Rhino.Testing.Grasshopper
{
    static class GH1Runner
    {
        static readonly Guid s_ctxBakeId = new("AE2531B4-BAB2-4BB1-B5BF-F2143D10C132");
        static readonly Guid s_ctxPrintId = new("73215EC5-0EB5-4F85-9E07-B09C4590CE2B");

        public static void TestGrasshopper(string ghFile, out bool result, out GHReport report, GHMessageLevel reportLevel)
        {
            GH_Document ghDoc = RunGrasshopper(ghFile, out report, reportLevel);
            bool hasCtxComponents = false;
            result = true;

            foreach (GH_Component ctxComp in ghDoc.Objects.Where(ao => ao.ComponentGuid == s_ctxBakeId
                                                                    || ao.ComponentGuid == s_ctxPrintId)
                                                          .Cast<GH_Component>())
            {
                hasCtxComponents = true;
                IGH_Param gparam = ctxComp.Params.Input[0];
                IGH_Structure data = gparam.VolatileData;

                foreach (GH_Path p in data.Paths)
                {
                    foreach (GH_Boolean d in data.get_Branch(p)
                                                 .OfType<GH_Boolean>())
                    {
                        result &= d.Value;
                    }
                }
            }

            result &= hasCtxComponents;
        }

        static GH_Document RunGrasshopper(string ghFile, out GHReport report, GHMessageLevel reportLevel)
        {
            GH_Document ghDoc = ReadDocument(ghFile);

            ghDoc.Enabled = true;
            ghDoc.NewSolution(expireAllObjects: false, mode: GH_SolutionMode.Silent);

            report = CreateReport(ghDoc, reportLevel);
            return ghDoc;
        }

        static GH_Document ReadDocument(string ghFile)
        {
            var io = new GH_DocumentIO();
            io.Open(ghFile);

            return io.Document;
        }

        static GHReport CreateReport(GH_Document ghDoc, GHMessageLevel reportLevel)
        {
            var entries = new List<GHReportEntry>();

            foreach (IGH_ActiveObject activeObject in ghDoc.Objects.OfType<IGH_ActiveObject>())
            {
                if (TryCreateReportEntry(activeObject, out GHReportEntry entry, reportLevel))
                {
                    entries.Add(entry);
                }
            }

            return new GHReport(entries);
        }

        static bool TryCreateReportEntry(IGH_ActiveObject activeObj, out GHReportEntry entry, GHMessageLevel reportLevel)
        {
            entry = default;

            var messages = new List<GHMessage>();

            if (reportLevel >= GHMessageLevel.Remark)
                foreach (string message in activeObj.RuntimeMessages(GH_RuntimeMessageLevel.Remark))
                    messages.Add(new GHMessage(message, GHMessageLevel.Remark));

            if (reportLevel >= GHMessageLevel.Warning)
                foreach (string message in activeObj.RuntimeMessages(GH_RuntimeMessageLevel.Warning))
                    messages.Add(new GHMessage(message, GHMessageLevel.Warning));

            if (reportLevel >= GHMessageLevel.Error)
                foreach (string message in activeObj.RuntimeMessages(GH_RuntimeMessageLevel.Error))
                    messages.Add(new GHMessage(message, GHMessageLevel.Error));

            if (messages.Count != 0)
            {
                entry = new GHReportEntry(activeObj, activeObj.ComponentGuid, activeObj.InstanceGuid, messages);
                return true;
            }

            return false;
        }
    }
}
