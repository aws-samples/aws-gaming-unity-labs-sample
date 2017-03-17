using System;
using System.Collections.Generic;

namespace GameSupport
{
	public interface ISocialProvider 
	{
		event Action<ISocialProvider> AuthChangedEvent;

		SocialNetwork Name { get; }
		string Url { get; }
		string AuthToken { get; }

		LoginStatus Status { get; }

		void Initialize(Action<SocialCallbackArgs> callback);
		void Login(List<SocialPermission> permissions = null, Action<SocialCallbackArgs> callback = null);
		void Logout();
	}
}
