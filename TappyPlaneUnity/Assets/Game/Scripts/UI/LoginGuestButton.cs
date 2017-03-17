using UnityEngine;
using UnityEngine.UI;
using GameSupport;

namespace Amazon
{
    public class LoginGuestButton : Button
    {
        override protected void Start()
        {
            base.Start();

            if(Application.isEditor)
            {
                return;
            }

            onClick.AddListener(HandleButtonClicked);

            CognitoIdentityBehavior identity = Services.Get<CognitoIdentityBehavior>();
            identity.StatusUpdatedEvent += HandleIdentityStatusUpdated;
            HandleIdentityStatusUpdated();
        }


        override protected void OnDestroy()
        {
            if(!Application.isEditor)
            {
                CognitoIdentityBehavior identity = Services.Get<CognitoIdentityBehavior>();
                if(identity != null)
                {
                    identity.StatusUpdatedEvent -= HandleIdentityStatusUpdated;
                }
            }

            base.OnDestroy();
        }


        void HandleButtonClicked()
        {
            if(Application.isEditor)
            {
                return;
            }

            CognitoIdentityBehavior identity = Services.Get<CognitoIdentityBehavior>();
            identity.LoginAsGuest("guest");
        }


        private void HandleIdentityStatusUpdated()
        {
            CognitoIdentityBehavior identity = Services.Get<CognitoIdentityBehavior>();
            if(identity.Status == LoginStatus.LoggedOut)
            {
                interactable = true;
            }
            else
            {
                interactable = false;
            }
        }
    }
}