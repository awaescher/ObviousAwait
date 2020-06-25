using FluentAssertions;
using NUnit.Framework;
using ObviousAwait;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ObviousTests
{
	public class ObviousExtensionsTests
	{
		private FieldInfo _capturedContextField;
		private FieldInfo _genericCapturedContextField;

		[OneTimeSetUp]
		public void Setup()
		{
			_capturedContextField = typeof(ConfiguredTaskAwaitable.ConfiguredTaskAwaiter)
				.GetField("m_continueOnCapturedContext", BindingFlags.Instance | BindingFlags.NonPublic);

			_genericCapturedContextField = typeof(ConfiguredTaskAwaitable<string>.ConfiguredTaskAwaiter)
				.GetField("m_continueOnCapturedContext", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		public class SetupTests : ObviousExtensionsTests
		{
			/// <summary>
			/// Test whether the method to detect if ConfigureAwait() was called with true or false
			/// does work as expected before we use it to check KeepContext() and FreeContext()
			/// </summary>
			[Test]
			public void CapturedContext_Detection_Does_Work_With_ConfigureAwait()
			{
				// non-generic tasks
				var awaiter = Task.CompletedTask.ConfigureAwait(false).GetAwaiter();
				_capturedContextField.GetValue(awaiter).Should().Be(false);

				awaiter = Task.CompletedTask.ConfigureAwait(true).GetAwaiter();
				_capturedContextField.GetValue(awaiter).Should().Be(true);

				// generic tasks
				var genericAwaiter = Task.FromResult("OK").ConfigureAwait(true).GetAwaiter();
				_genericCapturedContextField.GetValue(genericAwaiter).Should().Be(true);

				genericAwaiter = Task.FromResult("OK").ConfigureAwait(false).GetAwaiter();
				_genericCapturedContextField.GetValue(genericAwaiter).Should().Be(false);
			}
		}

		public class KeepContextMethod : ObviousExtensionsTests
		{
			[Test]
			public void Sets_ConfigureAwait_To_True()
			{
				var awaiter = Task.CompletedTask.KeepContext().GetAwaiter();
				_capturedContextField.GetValue(awaiter).Should().Be(true);
			}

			[Test]
			public void Sets_ConfigureAwait_To_True_For_Generic_Tasks()
			{
				var awaiter = Task.FromResult("OK").KeepContext().GetAwaiter();
				_genericCapturedContextField.GetValue(awaiter).Should().Be(true);
			}
		}

		public class FreeContextMethod : ObviousExtensionsTests
		{
			[Test]
			public void Sets_ConfigureAwait_To_False()
			{
				var awaiter = Task.CompletedTask.FreeContext().GetAwaiter();
				_capturedContextField.GetValue(awaiter).Should().Be(false);
			}

			[Test]
			public void Sets_ConfigureAwait_To_False_For_Generic_Tasks()
			{
				var awaiter = Task.FromResult("OK").FreeContext().GetAwaiter();
				_genericCapturedContextField.GetValue(awaiter).Should().Be(false);
			}
		}
	}
}
