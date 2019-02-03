using System;

namespace Provausio.Practices.EventSourcing
{
    public class DirtyStreamException : Exception
    {
        public DirtyStreamException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
