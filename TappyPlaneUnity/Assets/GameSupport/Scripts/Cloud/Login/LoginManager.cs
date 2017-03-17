using UnityEngine;
using UnityEngine.Assertions;
using System;

namespace GameSupport
{
	public class LoginManager : MonoBehaviour
	{
		[SerializeField] CognitoIdentityBehavior m_cognitoIdentity;
		[SerializeField] SocialManager m_socialManager;


		public event Action StatusChangedEvent;


		public LoginStatus Status { get; private set; }


		public void Awake()
		{
			Status = LoginStatus.LoggedOut;

			m_cognitoIdentity.StatusUpdatedEvent += HandleIdentityUpdatedEvent;
			m_socialManager.LoginChangedEvent += HandleSocialLoginChangedEvent;
		}


		public void LoginAsGuest(string guestName)
		{
			m_cognitoIdentity.LoginAsGuest(guestName);
			Status = LoginStatus.Processing;
		}

		public void Login(SocialNetwork socialNetwork)
		{
			m_socialManager.Login(socialNetwork);
			Status = LoginStatus.Processing;
		}


		public void Logout()
		{
			m_cognitoIdentity.Logout();
			m_socialManager.Logout();

			CheckStatus();
		}


		void HandleIdentityUpdatedEvent()
		{
			CheckStatus();
		}


		void HandleSocialLoginChangedEvent()
		{
			if(m_socialManager.Status == LoginStatus.LoggedIn)
			{
				ISocialProvider provider = m_socialManager.Provider;
				m_cognitoIdentity.AddAuthentication(provider.Url, provider.AuthToken);
			}

			CheckStatus();
		}


		void CheckStatus()
		{
			LoginStatus socialStatus = m_socialManager.Status;
			LoginStatus identityStatus = m_cognitoIdentity.Status;

			LoginStatus oldStatus = Status;

			if(identityStatus == LoginStatus.LoggedIn)
			{
				Status = LoginStatus.LoggedIn;
			}
			else if (socialStatus == LoginStatus.Processing || identityStatus == LoginStatus.Processing)
			{
				Status = LoginStatus.Processing;
			}
			else
			{
				Status = LoginStatus.LoggedOut;
			}

			if( (oldStatus != Status) && (Status != LoginStatus.Processing) )
			{
				var handlers = StatusChangedEvent;
				if(handlers != null)
				{
					StatusChangedEvent();
				}
			}
		}
	}
}
