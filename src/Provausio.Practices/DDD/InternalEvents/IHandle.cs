using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Provausio.Practices.DDD.InternalEvents
{
    public class InternalEventBus
    {
        internal ConcurrentDictionary<Type, IList<Func<object, Task>>> Handlers 
            = new ConcurrentDictionary<Type, IList<Func<object, Task>>>();

        private readonly IServiceProvider _serviceProvider;

        public InternalEventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ScanHandlers(params Assembly[] assembliesToScan)
        {
            var handlers = AppDomain.CurrentDomain
                .GetAssemblies()
                .Concat(assembliesToScan ?? new Assembly[0])
                .SelectMany(assy => assy.GetTypes())
                .Where(type => typeof(IInternalEventHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                .ToList();

            foreach (var handler in handlers)
            {
                var handlerInterfaces = handler
                        .GetInterfaces()
                        .Where(i => typeof(IInternalEventHandler).IsAssignableFrom(i) 
                                    && i != typeof(IInternalEventHandler));

                foreach (var handlerDeclaration in handlerInterfaces)
                {
                    var messageType = handlerDeclaration.GetGenericArguments().Single();
                    UpdateHandlers(messageType, (msg) =>
                    {
                        var handlerInstance = ActivatorUtilities.CreateInstance(_serviceProvider, handler);
                        return (Task) handlerInstance
                            .GetType().GetMethod("Handle")
                            .Invoke(handlerInstance, new []{msg});
                    });
                }
            }
        }

        private void UpdateHandlers(Type messageType, Func<object, Task> handler)
        {
            if(!Handlers.ContainsKey(messageType))
                Handlers.TryAdd(messageType, new List<Func<object, Task>>());

            if(!Handlers[messageType].Contains(handler))
                Handlers[messageType].Add(handler);
        }

        public async Task RaiseEvent<T>(T message) 
            where T : InternalEvent
        {
            // TODO: optimize and throttle etc.

            if(!Handlers.ContainsKey(typeof(T)))
                return;

            var handlers = Handlers[typeof(T)];
            var dispatched = new List<Task>();
            foreach(var handler in handlers)
                dispatched.Add(handler(message));

            await Task.WhenAll(dispatched);
        }
    }

    public class InternalEvent
    {

    }

    public interface IInternalEventHandler
    {

    }

    /// <summary>
    /// Designates the class as a handler of an <see cref="InternalEvent"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IHandle<T> : IInternalEventHandler
        where T : InternalEvent
    {
        Task Handle(T message);
    }
}
