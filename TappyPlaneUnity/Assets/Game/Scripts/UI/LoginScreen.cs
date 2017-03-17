using UnityEngine;
using UnityEngine.UI;
using GameSupport;

public enum LoginScreenMode
{
	WaitingForUser,
	LoggingIn,
	Loading
}

public class LoginScreen : UIScreenBase 
{
	[SerializeField] GameObject m_allLoginButtons;
	[SerializeField] Button m_playAsGuestButton;
	[SerializeField] Button m_facebookLoginButton;
	[SerializeField] GameObject m_loadingIndicator;
	

	protected override void OnInitialize(UIScreenArgs args)
	{
		m_playAsGuestButton.onClick.AddListener(HandlePlayAsGuestButtonClicked);
		m_facebookLoginButton.onClick.AddListener(HandleFacebookLoginButtonClicked);
	}


	protected override void OnReleaseResources()
	{
	}


	public void SetMode(LoginScreenMode mode)
	{
		switch(mode)
		{
		case LoginScreenMode.Loading:
			ShowLoginButtons(false);
			ShowLoadingIndicator(true);
			break;

		case LoginScreenMode.LoggingIn:
			ShowLoginButtons(false);
			ShowLoadingIndicator(true);
			break;

		case LoginScreenMode.WaitingForUser:
			ShowLoginButtons(true);
			ShowLoadingIndicator(false);
			break;
		}
	}


	public void HandlePlayAsGuestButtonClicked()
	{
		Services.Get<LoginManager>().LoginAsGuest("guest");
		SetMode (LoginScreenMode.LoggingIn);
	}


	public void HandleFacebookLoginButtonClicked()
	{
		Services.Get<LoginManager>().Login(SocialNetwork.Facebook);
		SetMode (LoginScreenMode.LoggingIn);
	}


	void ShowLoginButtons(bool value)
	{
		m_allLoginButtons.gameObject.SetActive (value);
	}


	void ShowLoadingIndicator(bool value)
	{
		m_loadingIndicator.gameObject.SetActive (value);
	}
}
