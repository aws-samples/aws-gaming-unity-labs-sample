using UnityEngine;
using GameSupport;
using System.Collections.Generic;

namespace Amazon
{

    public enum TappyGameState
    {
        LoggingIn,
        PreGame,
        Playing,
        PostGame
    }


    public class TappyGame : MonoBehaviour
    {
        [SerializeField] SpawnManager m_obstacles;
        [SerializeField] SpawnManager m_bonusPoints;
        [SerializeField] HorizontalScroller m_sky;
        [SerializeField] HorizontalScroller m_ground;

        int m_score;
        TappyGameState m_state = TappyGameState.PostGame;
        HUD m_hud;
        TappyPlayer m_player;
        int m_currentPlayerId;


        void Start()
        {
            Services.Get<LoginManager>().StatusChangedEvent += HandleLoginStatusChangedEvent;

            CreatePlayer();

            m_hud = Services.Get<UIManager>().SetHUD(UIScreenType.HUD, null) as HUD;

            StartLogin();
        }


        void Update()
        {
            if (m_state == TappyGameState.PostGame)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (m_state == TappyGameState.PreGame)
                {
                    StartGame();
                }

                m_player.Flap();
            }
        }


        void CreatePlayer()
        {
            if(m_player != null)
            {
                DestroyPlayer();
            }

            PlaneManager planeManager = Services.Get<PlaneManager>();
            m_player = planeManager.InstantiatePlayer();

            m_player.PlayerDeathEvent += HandlePlayerDeathEvent;
            m_player.PlayerDyingEvent += HandlePlayerDyingEvent;
            m_player.PlayerScoreEvent += HandlePlayerScoreEvent;
        }


        void DestroyPlayer()
        {
            if(m_player == null)
            {
                return;
            }

            m_player.PlayerDeathEvent -= HandlePlayerDeathEvent;
            m_player.PlayerDyingEvent -= HandlePlayerDyingEvent;
            m_player.PlayerScoreEvent -= HandlePlayerScoreEvent;

            GameObject.Destroy(m_player.gameObject);

            m_player = null;
        }


        void StartLogin()
        {
            LoginManager loginManager = Services.Get<LoginManager>();
            CognitoIdentityBehavior cognito = Services.Get<CognitoIdentityBehavior>();
            if(loginManager.Status != LoginStatus.LoggedIn && cognito.Status != LoginStatus.Uninitialized)
            {
                UIManager uiManager = Services.Get<UIManager>();
                LoginScreen login = uiManager.GetTopScreen() as LoginScreen;
                if(login == null)
                {
                    login = uiManager.PushScreen(UIScreenType.LoginDialog, null) as LoginScreen;
                }
                login.SetMode(LoginScreenMode.WaitingForUser);
            }
            else
            {
                FinishLogin();
            }
        }

        void FinishLogin()
        {
            Services.Get<UIManager>().PopScreen();

            IntroDialog intro = Services.Get<UIManager>().PushScreen(UIScreenType.IntroDialog, null) as IntroDialog;
            intro.PlayButtonClickedEvent += HandlePlayButtonClickedEvent;
            intro.LeaderboardsButtonClickedEvent += HandleLeaderboardsButtonClickedEvent;
        }

        void StartPreGame()
        {
            m_state = TappyGameState.PreGame;

            if(m_currentPlayerId != Services.Get<PlaneManager>().GetCurrentPlaneId())
            {
                CreatePlayer();
            }

            m_score = 0;
            m_sky.Run();
            m_ground.Run();

            m_player.Pause();

            m_hud.ShowGetReady();

            Services.Get<UIManager>().PopScreen();
        }


        void StartGame()
        {
            m_state = TappyGameState.Playing;

            m_player.Run();
            m_obstacles.Run();
            m_bonusPoints.Run();

            m_hud.Play();

            Services.Get<MobileAnalyticsBehavior>().RecordCustomEvent("StartLevel", null, null);
        }


        void StopGame()
        {
            m_state = TappyGameState.PostGame;

            m_player.Stop();
            m_obstacles.Stop();
            m_bonusPoints.Stop();
            m_sky.Stop();
            m_ground.Stop();

            OutroDialog outro = Services.Get<UIManager>().PushScreen(UIScreenType.OutroDialog, new OutroArgs(m_score)) as OutroDialog;
            outro.PlayButtonClickedEvent += HandlePlayButtonClickedEvent;
            outro.LeaderboardsButtonClickedEvent += HandleLeaderboardsButtonClickedEvent;
            outro.StoreButtonClickedEvent += HandleStoreButtonClickedEvent;
        }


        private void HandleLoginStatusChangedEvent()
        {
            LoginManager loginManager = Services.Get<LoginManager>();
            Debug.Log(loginManager.Status);
            switch(loginManager.Status)
            {
                case LoginStatus.LoggedIn:
                    FinishLogin();
                    break;
                case LoginStatus.LoggedOut:
                    StartLogin();
                    break;
            }
        }


        void HandlePlayerDeathEvent()
        {
            StopGame();
        }


        void HandlePlayerDyingEvent()
        {
            m_obstacles.Pause();
            m_bonusPoints.Pause();
            m_sky.Pause();
            m_ground.Pause();

            Dictionary<string, double> metrics = new Dictionary<string, double>();
            metrics.Add("Score", m_score);
            Services.Get<MobileAnalyticsBehavior>().RecordCustomEvent("EndOfLevel", null, metrics);
        }

        void HandlePlayerScoreEvent(int amount)
        {
            m_score += amount;
            m_hud.SetScore(m_score);
        }


        void HandlePlayButtonClickedEvent()
        {
            StartPreGame();
        }


        void HandleLeaderboardsButtonClickedEvent()
        {
            Debug.LogError("Leaderboards not implemented.");
        }


        void HandleStoreButtonClickedEvent()
        {
            Debug.LogError("Store not implemented.");
        }
    }
}