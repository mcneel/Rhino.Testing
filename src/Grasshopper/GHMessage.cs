using System;

namespace Rhino.Testing.Grasshopper
{
    public enum GHMessageLevel
    {
        Remark,
        Warning,
        Error,
    }

    public sealed class GHMessage
    {
        public string Message { get; }

        public GHMessageLevel Level { get; }

        public GHMessage(string message, GHMessageLevel level)
        {
            Message = message;
            Level = level;
        }
    }
}
