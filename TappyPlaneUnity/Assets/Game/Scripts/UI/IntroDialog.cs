using UnityEngine;
using UnityEngine.UI;
using System;
using GameSupport;

namespace Amazon
{
    public class IntroDialog : UIScreenBase
    {
        [SerializeField] Button m_playButton;
        [SerializeField] Button m_leaderboardsButton;


        public event Action PlayButtonClickedEvent;
        public event Action LeaderboardsButtonClickedEvent;


        protected override void OnInitialize(UIScreenArgs args)
        {
            m_playButton.onClick.AddListener(HandlePlayButtonClicked);
            m_leaderboardsButton.onClick.AddListener(HandleLeaderboardsButtonClicked);
        }


        protected override void OnReleaseResources()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_leaderboardsButton.onClick.RemoveAllListeners();
        }


        public void HandlePlayButtonClicked()
        {
            if(PlayButtonClickedEvent != null)
            {
                PlayButtonClickedEvent();
            }
        }


        public void HandleLeaderboardsButtonClicked()
        {
            if(LeaderboardsButtonClickedEvent != null)
            {
                LeaderboardsButtonClickedEvent();
            }
        }
    }
}