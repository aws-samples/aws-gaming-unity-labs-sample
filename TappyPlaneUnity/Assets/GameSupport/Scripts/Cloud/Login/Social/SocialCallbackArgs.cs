using System;
using System.Collections.Generic;

namespace GameSupport
{
	public enum SocialResult
	{
		None,
		Success,
		Failure,
		Cancelled
	}

	public class SocialCallbackArgs
	{
		public readonly ISocialProvider provider;
		public readonly SocialResult result;
		public readonly string error;
		
		public SocialCallbackArgs(ISocialProvider inProvider, SocialResult inResult, string inError = null)
		{
			provider = inProvider;
			result = inResult;
			error = inError;
		}
	}
}
