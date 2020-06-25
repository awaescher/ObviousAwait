using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ObviousAwait
{
	public static class ObviousExtensions
	{
		public static ConfiguredTaskAwaitable KeepContext(this Task t) => t.ConfigureAwait(true);

		public static ConfiguredTaskAwaitable<T> KeepContext<T>(this Task<T> t) => t.ConfigureAwait(true);

		public static ConfiguredTaskAwaitable FreeContext(this Task t) => t.ConfigureAwait(false);

		public static ConfiguredTaskAwaitable<T> FreeContext<T>(this Task<T> t) => t.ConfigureAwait(false);
	}
}