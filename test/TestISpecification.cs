using System;
using Xunit;
using Xunit.Extensions;

namespace Xunit.Bdd.Test
{
	public class behaves_like_an_ispecification : ISpecification
	{
		public Exception ThrownException { get; set; }

		protected static int constructionCount = 0;

		public behaves_like_an_ispecification()
		{
			constructionCount++;
		}

		private bool observedInBase = false;


		public virtual void Observe()
		{
			observedInBase = true;
		}

		[Observation]
		public void should_call_base_observe()
		{
			observedInBase.ShouldBeTrue("Observe should be called in the base class");
		}

		[Observation]
		public void should_be_constructed_either_once_or_twice()
		{
			Assert.InRange(constructionCount, 1, 2);
		}

		[Observation(Skip = "Skipped this observation")]
		public void should_skip_this_observation()
		{
			Assert.True(false);
		}

		[Observation]
		public void should_have_no_exception()
		{
			ThrownException.ShouldBeNull();
		}
	}

	public class behaves_like_a_polymorphic_ispecification : behaves_like_an_ispecification
	{
		protected bool observedInDerived = false;

		public override void Observe()
		{
			base.Observe();
			observedInDerived = true;
		}

		[Observation]
		public void should_call_derived_observe()
		{
			observedInDerived.ShouldBeTrue("Observe should be called in the derived class");
		}
	}
	
	[HandleExceptions]
	public class behaves_like_an_ispecification_that_throws_during_setup : ISpecification
	{
		public Exception ThrownException { get; set; }
		public void Observe()
		{
			throw new TestException();
		}

		[Observation]
		public void should_handle_exception()
		{
			ThrownException.ShouldNotBeNull();
			ThrownException.ShouldBeType<TestException>();
		}
	}

	public class behaves_like_an_ispecification_that_unexpectedly_throws_during_setup : ISpecification
	{
		public Exception ThrownException { get; set; }
		public void Observe()
		{
			throw new TestException();
		}

		[Observation]
		public void should_be_inconclusive()
		{
			// This test will have an inconclusive result because of the exception thrown in Observe()
		}

		[Observation(Skip = "YOU SHOULD NEVER SEE THIS AS A TEST RESULT")] // The runner can't reach the point where it skips a test if its setup can't be run first.
		public void should_still_be_inconclusive_even_if_skipped()
		{
			// This test will have an inconclusive result because of the exception thrown in Observe()
		}
	}
}
