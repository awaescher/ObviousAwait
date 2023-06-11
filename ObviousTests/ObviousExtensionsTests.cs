using FluentAssertions;
using NUnit.Framework;
using ObviousAwait;
using System.Reflection;
using System.Threading.Tasks;

namespace ObviousTests
{
	public class ObviousExtensionsTests
	{
		private const string AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT = "m_continueOnCapturedContext";
		private const string VALUETASK_FIELD_CONTINUE_ON_CAPTURED_CONTEXT = "_continueOnCapturedContext";

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
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(false);

				awaiter = Task.CompletedTask.ConfigureAwait(true).GetAwaiter();
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(true);

				// generic tasks
				var genericAwaiter = Task.FromResult("OK").ConfigureAwait(true).GetAwaiter();
				genericAwaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(true);

				genericAwaiter = Task.FromResult("OK").ConfigureAwait(false).GetAwaiter();
				genericAwaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(false);
			}
		}

		public class KeepContextMethod : ObviousExtensionsTests
		{
			[Test]
			public void Sets_ConfigureAwait_To_True()
			{
				var awaiter = Task.CompletedTask.KeepContext().GetAwaiter();
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(true);
			}

			[Test]
			public void Sets_ConfigureAwait_To_True_For_Generic_Tasks()
			{
				var awaiter = Task.FromResult("OK").KeepContext().GetAwaiter();
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(true);
			}


			[Test]
			public void Sets_ConfigureAwait_To_False_For_ValueTasks()
			{
				var awaitable = new ValueTask(Task.FromResult("OK")).KeepContext();

				var awaitableValueTask = awaitable.GetPrivateFieldValue<ValueTask>("_value");
				awaitableValueTask.GetPrivateFieldValue<bool>(VALUETASK_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(true);
			}
		}

		public class FreeContextMethod : ObviousExtensionsTests
		{
			[Test]
			public void Sets_ConfigureAwait_To_False()
			{
				var awaiter = Task.CompletedTask.FreeContext().GetAwaiter();
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(false);
			}

			[Test]
			public void Sets_ConfigureAwait_To_False_For_Generic_Tasks()
			{
				var awaiter = Task.FromResult("OK").FreeContext().GetAwaiter();
				awaiter.GetPrivateFieldValue<bool>(AWAITER_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(false);
			}

			[Test]
			public void Sets_ConfigureAwait_To_False_For_ValueTasks()
			{
				var awaitable = new ValueTask(Task.FromResult("OK")).FreeContext();

				var awaitableValueTask = awaitable.GetPrivateFieldValue<ValueTask>("_value");
				awaitableValueTask.GetPrivateFieldValue<bool>(VALUETASK_FIELD_CONTINUE_ON_CAPTURED_CONTEXT).Should().Be(false);
			}
		}
	}

	public static class ReflectionHelper
	{
		public static T GetPrivateFieldValue<T>(this object fromInstance, string fieldName) => (T)fromInstance.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(fromInstance);
	}
}
