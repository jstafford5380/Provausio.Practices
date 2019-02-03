using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DAS.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Provausio.Practices.EventSourcing
{
    public static class ProjectionLoader
    {
        /// <summary>
        /// Scans the assembly that contains the specified type and then starts the projections.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="targetAssemblyType"></param>
        /// <returns></returns>
        public static async Task ScanForAndStartProjections(
            IServiceProvider provider, 
            Type targetAssemblyType)
        {
            var projectionTypes =
                Assembly.GetAssembly(targetAssemblyType)
                    .GetTypes()
                    .Where(type => type.GetInterfaces().Contains(typeof(IProjection)) && !type.IsAbstract)
                    .ToArray();

            await StartProjections(provider, projectionTypes).ConfigureAwait(false);
        }

        /// <summary>
        /// Starts the specified projections.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <param name="projectionTypes"></param>
        /// <returns></returns>
        public static async Task StartProjections(
            IServiceProvider serviceProvider, 
            params Type[] projectionTypes)
        {
            Logger.Information($"Found {projectionTypes.Length} projections. Attempting to start...", null);
            
            foreach (var projectionType in projectionTypes)
            {
                var projection = (IProjection) ActivatorUtilities.CreateInstance(serviceProvider, projectionType);
                await projection.StartAsync().ConfigureAwait(false);
                Logger.Information($"Started {projectionType.FullName}!", null);
            }
        }
    }
}
