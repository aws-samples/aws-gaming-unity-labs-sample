using UnityEngine;
using System.Collections.Generic;
using System;

namespace GameSupport
{
	public enum SocialNetwork
	{
		Facebook
	}

	public class SocialManager : MonoBehaviour
	{
		Dictionary<SocialNetwork, ISocialProvider> m_providers = new Dictionary<SocialNetwork, ISocialProvider>();
		ISocialProvider m_activeProvider;

		public event Action LoginChangedEvent;

		public LoginStatus Status { get; private set; }

		public ISocialProvider Provider { get { return m_activeProvider; } }


		public void Awake()
		{
			Status = LoginStatus.Processing;

			m_providers[SocialNetwork.Facebook] = new FacebookProvider();
			
			foreach(ISocialProvider provider in m_providers.Values)
			{
				provider.AuthChangedEvent += HandleAuthChangedEvent;
				provider.Initialize(HandleInitializedCallback);
			}
		}


		public void Login(List<SocialPermission> permissions = null, Action<SocialCallbackArgs> callback = null)
		{
			if(m_activeProvider != null)
			{
				m_activeProvider.Login(permissions, callback);
			}
		}


		public void Login(SocialNetwork network, Action<SocialCallbackArgs> callback = null)
		{
			Status = LoginStatus.Processing;

			m_activeProvider = m_providers[network];
			m_activeProvider.Login(null, callback);
		}


		public void Logout()
		{
			if(m_activeProvider != null)
			{
				m_activeProvider.Logout();
				m_activeProvider = null;
			}

			Status = LoginStatus.LoggedOut;
		}


		void HandleInitializedCallback(SocialCallbackArgs args)
		{
			//if we are already logged in, do nothing
			if(Status == LoginStatus.LoggedIn)
			{
				return;
			}

			//if any of the providers are still initializing, do nothing
			foreach(var key in m_providers.Keys)
			{
				if(m_providers[key].Status == LoginStatus.Processing)
				{
					return;
				}
			}

			//if all providers are initialized, set the status
			Status = LoginStatus.LoggedOut;
		}


		void HandleAuthChangedEvent (ISocialProvider provider)
		{
			m_activeProvider = provider;

			Status = m_activeProvider.Status;

			var handlers = LoginChangedEvent;
			if(handlers != null)
			{
				LoginChangedEvent();
			}
		}
	}
}