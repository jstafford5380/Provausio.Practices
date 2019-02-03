using System;

namespace Provausio.Practices.EventSourcing.Aggregate
{
    public class HandlerNotRegisteredException : Exception
    {
        public Type HandlerType { get; }

        public HandlerNotRegisteredException(Type handlerType, string message)
            : base(message)
        {
            HandlerType = handlerType;
        }
    }
}