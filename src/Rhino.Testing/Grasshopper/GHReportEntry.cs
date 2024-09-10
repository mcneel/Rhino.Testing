using System;
using System.Linq;
using System.Collections.Generic;

namespace Rhino.Testing.Grasshopper
{
    public sealed class GHReportEntry
    {
        readonly GHMessage[] _messages;
        readonly object _source;

        public Guid TypeId { get; }

        public Guid InstanceId { get; }

        public IEnumerable<GHMessage> Messages => _messages.AsEnumerable();

        public bool HasErrors => _messages.Any(m => m.Level == GHMessageLevel.Error);

        public bool HasWarnings => _messages.Any(m => m.Level == GHMessageLevel.Warning);

        public bool HasRemarks => _messages.Any(m => m.Level == GHMessageLevel.Remark);

        public GHReportEntry(object source, Guid typeId, Guid instanceId, IEnumerable<GHMessage> messages)
        {
            _source = source;
            _messages = messages.ToArray();

            TypeId = typeId;
            InstanceId = instanceId;
        }

        public T GetSourceAs<T>() => (T)_source;
    }
}
