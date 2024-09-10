using System;
using System.Linq;
using System.Collections.Generic;

namespace Rhino.Testing.Grasshopper
{
    public sealed class GHReport
    {
        readonly GHReportEntry[] _entries;

        public IEnumerable<GHReportEntry> Entries => _entries.AsEnumerable();

        public bool HasErrors => _entries.Any(e => e.HasErrors);

        public GHReport(IEnumerable<GHReportEntry> entries)
        {
            _entries = entries.ToArray();
        }
    }
}
