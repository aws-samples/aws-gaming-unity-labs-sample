using UnityEngine;
using UnityEngine.UI;
using GameSupport;

namespace Amazon
{
    public class LoginUserButton : Button
    {
        override protected void Start()
        {
            base.Start();
            onClick.AddListener(HandleButtonClicked);
        }


        void HandleButtonClicked()
        {
            Debug.LogError("User login not implemented");
        }
    }
}