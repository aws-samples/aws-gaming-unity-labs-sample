using UnityEngine;
using Facebook.Unity;
using System;
using System.Collections.Generic;

namespace GameSupport
{
	public class FacebookProvider : ISocialProvider
	{
		Action<SocialCallbackArgs> m_initializeCallback;
		LoginStatus m_loginStatus;

		public event Action<ISocialProvider> AuthChangedEvent;


		public SocialNetwork Name { get { return SocialNetwork.Facebook; } }
		public string Url { get { return "graph.facebook.com";  } }
		public string AuthToken { get { return AccessToken.CurrentAccessToken != null ? AccessToken.CurrentAccessToken.TokenString : null; } }

		public LoginStatus Status 
		{ 
			get { return m_loginStatus; }

			private set
			{
				if(value != m_loginStatus)
				{
					m_loginStatus = value;

					var handlers = AuthChangedEvent;
					if(handlers != null)
					{
						handlers(this);
					}
				}
			}
		}

		public void Initialize(Action<SocialCallbackArgs> callback)
		{
			m_initializeCallback = callback;

			Status = LoginStatus.Processing;

			if (!FB.IsInitialized) 
			{
				FB.Init(HandleInitializedCallback, HandleHideUnityCallback);
			} 
			else 
			{
				HandleInitializedCallback();
			}
		}


		public void Login(List<SocialPermission> permissions = null, Action<SocialCallbackArgs> callback = null)
		{
			Status = LoginStatus.Processing;

			//get the current permissions
			AccessToken accessToken = AccessToken.CurrentAccessToken;
			List<string> grantedPermissions = (accessToken == null) ? new List<string>() : new List<string>(accessToken.Permissions);

			//gather the needed permissions
			bool requestingPublishPermissions = false;
			List<string> neededPermissions = new List<string>();

			if(permissions != null)
			{
				for(int i=0; i<permissions.Count; ++i)
				{
					requestingPublishPermissions = requestingPublishPermissions || IsPublishPermission(permissions[i]);

					string nativePermission = GetNativePermission(permissions[i]);
					if(grantedPermissions.Contains(nativePermission))
					{
						neededPermissions.Add(nativePermission);
					}
				}
			}

			//we are logged in and have all the needed permissions
			if (FB.IsLoggedIn && neededPermissions.Count == 0)
			{
				Status = LoginStatus.LoggedIn;

				if(callback != null)
				{
					callback(new SocialCallbackArgs(this, SocialResult.Success));
				}

				return;
			}

			//login
			if(requestingPublishPermissions)
			{
				FB.LogInWithPublishPermissions(neededPermissions, (result) => {HandleLoginCallback(result, callback);} );
			}
			else
			{
				FB.LogInWithReadPermissions(neededPermissions, (result) => {HandleLoginCallback(result, callback);} );
			}
		}


		public void Logout()
		{
			Status = LoginStatus.LoggedOut;

			FB.LogOut();
		}


		void HandleInitializedCallback()
		{
            if (FB.IsInitialized)
            {
                FB.ActivateApp();
            }

			Status = FB.IsLoggedIn ? LoginStatus.LoggedIn : LoginStatus.LoggedOut;

			if(m_initializeCallback != null)
			{
				m_initializeCallback(new SocialCallbackArgs(this, SocialResult.Success));
			}
		}


		void HandleAuthChangedCallback()
		{
			Debug.Log("Social:Facebook Auth changed callback");

			Status = FB.IsLoggedIn ? LoginStatus.LoggedIn : LoginStatus.LoggedOut;
		}


		void HandleLoginCallback (ILoginResult result, Action<SocialCallbackArgs> loginCallback) 
		{
			SocialResult socialResult = SocialResult.Success;
			if(result.Cancelled) 
			{
				Status = LoginStatus.LoggedOut;
				socialResult = SocialResult.Cancelled;
			}
			else if(!string.IsNullOrEmpty(result.Error))
			{
				Status = LoginStatus.LoggedOut;
				socialResult = SocialResult.Failure;
				Debug.LogError (result.Error);
			}

			Status = FB.IsLoggedIn ? LoginStatus.LoggedIn : LoginStatus.LoggedOut;

			if(loginCallback != null)
			{
				var args = new SocialCallbackArgs(this, socialResult, result.Error);
				loginCallback(args);
			}
		}
	

		void HandleHideUnityCallback (bool isGameShown)
		{
		}

		string GetNativePermission(SocialPermission permission)
		{
			switch(permission)
			{
			case SocialPermission.PublicProfile:	return "public_profile";
			case SocialPermission.Email: 			return "email";
			case SocialPermission.UserFriends:		return "user_friends";
			case SocialPermission.PublishActions:	return "publish_actions";
			default: 
				Debug.LogError("Facebook provider does not handle the following permission: " + permission);
				return null;
			}
		}

		bool IsPublishPermission(SocialPermission permission)
		{
			if(permission == SocialPermission.PublishActions)
			{
				return true;
			}

			return false;
		}
	}
}
