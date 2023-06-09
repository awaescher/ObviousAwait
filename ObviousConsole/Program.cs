using System.Threading.Tasks;

namespace ObviousAwait.Console
{
    class Program
    {
        static async Task Main()
        {
            #region Task

            await Task.Delay(100).ConfigureAwait(false);
            await Task.Delay(100).ConfigureAwait(true);

            await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: false);
            await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: true);

            await Task.Delay(100).KeepContext();
            await Task.Delay(100).FreeContext();

            #endregion

            #region ValueTask

            await new ValueTask(Task.Delay(100)).ConfigureAwait(false);
            await new ValueTask(Task.Delay(100)).ConfigureAwait(true);

            await new ValueTask(Task.Delay(100)).ConfigureAwait(continueOnCapturedContext: false);
            await new ValueTask(Task.Delay(100)).ConfigureAwait(continueOnCapturedContext: true);

            await new ValueTask(Task.Delay(100)).KeepContext();
            await new ValueTask(Task.Delay(100)).FreeContext();

            #endregion
        }
    }
}