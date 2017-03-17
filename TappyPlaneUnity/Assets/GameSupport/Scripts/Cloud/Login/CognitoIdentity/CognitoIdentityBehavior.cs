using UnityEngine;
using UnityEngine.Assertions;
using Amazon.CognitoIdentity;
using System;

namespace GameSupport
{
    public class CognitoIdentityBehavior : MonoBehaviour
    {
        [SerializeField] string m_identityPoolId;
        [SerializeField] AWSRegionEnum m_region;

		private static readonly string COGNITO_GUESTNAMES_CACHE_KEY = "CognitoIdentityManager:GuestNames";

        CognitoAWSCredentials m_credentials;
        LoginStatus m_status;
		GuestIdCache m_guestIdCache;

        public event Action StatusUpdatedEvent;

        public string Id { get { return m_credentials.GetCachedIdentityId(); } }
        public bool IsAuthenticated { get { return m_credentials.LoginsCount > 0; } }
        public CognitoAWSCredentials Credentials { get { return m_credentials; } }


        public LoginStatus Status
        {
            get { return m_status; }

            set
            {
                if(value != m_status)
                {
                    m_status = value;

                    var handlers = StatusUpdatedEvent;
                    if(handlers != null)
                    {
                        handlers();
                    }
                }
            }
        }


        public void Awake()
        {
            if(string.IsNullOrEmpty(m_identityPoolId))
            {
                Status = LoginStatus.Uninitialized;
                return;
            }
            
            m_credentials = new CognitoAWSCredentials(m_identityPoolId, AWSRegionHelpers.GetByEnum(m_region));
            m_credentials.IdentityChangedEvent += HandleIdentityChangedEvent;

			string key = COGNITO_GUESTNAMES_CACHE_KEY + ":" + m_identityPoolId;
			m_guestIdCache = new GuestIdCache(key);
			m_guestIdCache.RetrieveGuestIds();

            Logout();
        }


		public void LoginAsGuest(string guestName)
        {
            Assert.IsFalse(string.IsNullOrEmpty(guestName));

            if(Status == LoginStatus.Uninitialized)
            {
                Debug.LogWarning("Attempting to login when CognitoIdentity is not initialized.");
                return;
            }

 			Logout();

			//there is an id for this guest
			string cachedId = m_guestIdCache.GetIdByName(guestName);
			if(cachedId != null)
			{
				m_credentials.CacheIdentityId(cachedId);
			}

			RefreshIdentity(guestName);
        }

 
        public void Logout()
        {
            Debug.Log("Cognito Identity: Clearing credentials");
            m_credentials.Clear();
            Status = LoginStatus.LoggedOut;
        }


		public void AddAuthentication(string provider, string authToken)
		{
			Debug.Log("Cognito Identity: Adding login provider: " + provider + " auth: " + authToken + " to Cognito Id: " + m_credentials.GetCachedIdentityId());
			m_credentials.AddLogin(provider, authToken);
			string guestName = m_guestIdCache.GetNameById(m_credentials.GetCachedIdentityId());
			RefreshIdentity(guestName);
		}


		void RefreshIdentity(string guestName)
        {
            Debug.Log("Cognito Identity: Refreshing Identity - " + guestName);
			m_credentials.GetCredentialsAsync( (val) => { HandleGetIdentityIdCallback(val, guestName);}, null);
            Status = LoginStatus.Processing;
        }


		void HandleGetIdentityIdCallback(AmazonCognitoIdentityResult<Amazon.Runtime.ImmutableCredentials> result, string guestName)
        {
            if(result.Exception != null)
            {
                Debug.LogError(result.Exception);
                Status = LoginStatus.LoggedOut;
                return;
            }

            Debug.Log("Current Identity is now: " + m_credentials.GetCachedIdentityId());

			if(guestName != null)
			{
				if(IsAuthenticated)
				{
					m_guestIdCache.ClearGuestById(m_credentials.GetCachedIdentityId());
				}
				else
				{
					m_guestIdCache.Add (guestName, m_credentials.GetCachedIdentityId());
				}
				m_guestIdCache.StoreGuestIds();
			}

			Status = result.Response == null ? LoginStatus.LoggedOut : LoginStatus.LoggedIn;
		}


		//Note: This event does not happen on the main thread.  Therefore, you cannot interact with any Unity objects
        void HandleIdentityChangedEvent(object sender, CognitoAWSCredentials.IdentityChangedArgs e)
        {
            Debug.Log("Cognito Identity: Identity changed to " + e.NewIdentityId + " from " + e.OldIdentityId);
        }
    }
}
