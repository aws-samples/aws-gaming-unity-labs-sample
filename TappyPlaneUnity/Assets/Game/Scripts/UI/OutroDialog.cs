using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using GameSupport;

namespace Amazon
{ 
    public class OutroArgs : UIScreenArgs
    {
        public int score;

        public OutroArgs(int inScore)
        {
            score = inScore;
        }
    }


    public class OutroDialog : UIScreenBase 
    {
	    [Header("Score")]
	    [SerializeField] Text m_score;
	    [SerializeField] Text m_best;

        [Header("Buttons")]
        [SerializeField] Button m_guestLoginButton;
        [SerializeField] Button m_userLoginButton;
        [SerializeField] Button m_leaderboardsButton;
        [SerializeField] Button m_storeButton;
        [SerializeField] Button m_playButton;

        [Header("Plane Selector")]
        [SerializeField] PlaneCardSelection m_planeSelector;

        Animator m_animator;

	    public event Action PlayButtonClickedEvent;
        public event Action LeaderboardsButtonClickedEvent;
        public event Action StoreButtonClickedEvent;


        void Awake()
	    {
		    m_animator = GetComponent<Animator>();
	    }


        protected override void OnInitialize(UIScreenArgs args)
        {
            OutroArgs outroArgs = args as OutroArgs;
            Assert.IsNotNull(args);

            int best = PlayerPrefs.GetInt("BestScore");
            if(outroArgs.score > best)
            {
                best = outroArgs.score;
                PlayerPrefs.SetInt("BestScore", best);
            }

            m_score.text = outroArgs.score.ToString();
            m_best.text = best.ToString();

            m_animator.SetTrigger("Show");

            m_playButton.onClick.AddListener(HandlePlayButtonClicked);
            m_leaderboardsButton.onClick.AddListener(HandleLeaderboardsButtonClicked);
            m_storeButton.onClick.AddListener(HandleStoreButtonClicked);

            m_planeSelector.Initialize();
        }


        protected override void OnReleaseResources()
        {
            m_playButton.onClick.RemoveAllListeners();
            m_leaderboardsButton.onClick.RemoveAllListeners();
            m_storeButton.onClick.RemoveAllListeners();
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


        public void HandleStoreButtonClicked()
        {
            if(StoreButtonClickedEvent != null)
            {
                StoreButtonClickedEvent();
            }
        }
    }
}