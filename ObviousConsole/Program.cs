using System.Threading.Tasks;

namespace ObviousAwait.Console
{
	class Program
	{
		static async Task Main()
		{
			await Task.Delay(100).ConfigureAwait(false);
			await Task.Delay(100).ConfigureAwait(true);

			await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: false);
			await Task.Delay(100).ConfigureAwait(continueOnCapturedContext: true);

			await Task.Delay(100).KeepContext();
			await Task.Delay(100).FreeContext();
		}
	}
}
