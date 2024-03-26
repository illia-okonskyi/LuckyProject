using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.ConsoleTool.Helpers
{
    public static class HostedServiceHelper
    {
        public static async Task ExecuteAsync(
            Func<CancellationToken, Task> func,
            IEnvironmentService environmentService,
            ILogger logger,
            CancellationToken cancellationToken,
            Action<int> onBeforeExit = null)
        {
            var exitCode = 0;
            try
            {
                await func(cancellationToken);
            }
            catch (LpExitCodeException appErrorEx)
            {
                exitCode = appErrorEx.ExitCode;
                logger.LogError(appErrorEx, null);
            }
            catch (OperationCanceledException)
            {
                // NOTE: Do nothing, swallow
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unexpected error");
                exitCode = -1;
            }
            finally
            {
                if (onBeforeExit != null)
                {
                    onBeforeExit(exitCode);
                }
                environmentService.Exit(exitCode);
            }
        }
    }
}
