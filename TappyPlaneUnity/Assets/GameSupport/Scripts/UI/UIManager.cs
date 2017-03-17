using UnityEngine;
using System.Collections.Generic;

namespace GameSupport
{
	[System.Serializable]
	public class UIScreenPrefabFile
	{
		public UIScreenType uiScreenType;
		public string fileName;
	}

	public class UIManager : MonoBehaviour
	{
		[SerializeField] List<UIScreenPrefabFile> m_uiPrefabs;

		Dictionary<UIScreenType, string> m_prefabFiles = new Dictionary<UIScreenType, string>();

		Stack<UIScreenBase> m_screenStack = new Stack<UIScreenBase>();
		UIScreenBase m_hud;
		UIScreenBase m_transitionScreen;

		const int MIN_SCREEN_PANEL_DEPTH = 10;

		int m_transitionPanelDepth = 300;
		int m_currentPanelDepth = MIN_SCREEN_PANEL_DEPTH;


		void Awake()
		{
			InitializePrefabFiles();
		}


		public void ReleaseResources()
		{
			PopAllScreens();
			SetHUD(UIScreenType.None, null);
			SetTransitionScreen(UIScreenType.None, null);
		}


		public UIScreenBase PushScreen(UIScreenType screenType, UIScreenArgs args)
		{
			UIScreenBase screen = LoadScreen(screenType);
			if(screen == null)
			{
				return null;
			}
			
			m_currentPanelDepth = IncrementScreenPanelDepth(screen, m_currentPanelDepth);

			m_screenStack.Push(screen);
			screen.Initialize(args);

			return screen;
		}


		public void PopScreen()
		{
			if(m_screenStack.Count == 0)
				return;

			UIScreenBase old = m_screenStack.Peek();
			m_screenStack.Pop();

			old.ReleaseResources();

			old.transform.SetParent(null);
			GameObject.Destroy(old.gameObject);
		}


		public void PopAllScreens()
		{
			while(m_screenStack.Count > 0)
			{
				PopScreen();
			}

			m_currentPanelDepth = MIN_SCREEN_PANEL_DEPTH;
		}


		public void PopAllButTopScreen()
		{
			if(m_screenStack.Count <= 1)
				return;

			UIScreenBase topScreen = m_screenStack.Peek();
			m_screenStack.Pop();

			PopAllScreens();

			m_screenStack.Push(topScreen);
		}


		public void HidePreviousScreen()
		{
			if(m_screenStack.Count <= 1)
				return;

			UIScreenBase topScreen = m_screenStack.Peek();
			m_screenStack.Pop();

			UIScreenBase previousScreen = m_screenStack.Peek();
			previousScreen.Hide();

			m_screenStack.Push(topScreen);
		}


		public UIScreenBase GetTopScreen()
		{
			if(m_screenStack.Count > 0)
			{
				return m_screenStack.Peek();
			}

			return null;
		}


		public UIScreenBase SetHUD(UIScreenType screenType, UIScreenArgs args)
		{
			if(m_hud != null)
			{
				m_hud.ReleaseResources();

				m_hud.transform.parent = null;
				m_hud.gameObject.SetActive(false);
				GameObject.Destroy(m_hud.gameObject);
			}

			if(screenType == UIScreenType.None)
			{
				m_hud = null;
				return null;
			}

			m_hud = LoadScreen(screenType);
			if(m_hud == null)
			{
				return null;
			}

			m_hud.Initialize(args);
			return m_hud;
		}


		public UIScreenBase GetHUD()
		{
			return m_hud;
		}
		

		public UIScreenBase SetTransitionScreen(UIScreenType screenType, UIScreenArgs args)
		{
			if(m_transitionScreen != null)
			{
				m_transitionScreen.ReleaseResources();

				m_transitionScreen.transform.parent = null;
				m_transitionScreen.gameObject.SetActive(false);
				GameObject.Destroy(m_transitionScreen.gameObject);
			}

			if(screenType == UIScreenType.None)
			{
				m_transitionScreen = null;
				return null;
			}

			m_transitionScreen = LoadScreen (screenType) as UIScreenBase;
			if(m_transitionScreen == null)
			{
				return null;
			}

			IncrementScreenPanelDepth(m_transitionScreen, m_transitionPanelDepth);

			m_transitionScreen.Initialize(args);

			return m_transitionScreen;
		}


		public UIScreenBase GetTransitionScreen()
		{
			return m_transitionScreen;
		}


		int IncrementScreenPanelDepth(UIScreenBase screen, int increment)
		{
			int maxPanelDepth = 0;
			int minPanelDepth = 0;

			Canvas[] canvases = screen.gameObject.GetComponentsInChildren<Canvas>();
			for(int i=0; i<canvases.Length; ++i)
			{
				int depth = canvases[i].sortingOrder;
				maxPanelDepth = System.Math.Max (maxPanelDepth, depth);
				minPanelDepth = System.Math.Min (minPanelDepth, depth);
			}

			// if there are negative panel depths all the panels need to be moved forward
			// enough to ensure that the negative are on top
			if(minPanelDepth < 0)
			{
				increment += System.Math.Abs(minPanelDepth);
			}

			for(int i=0; i<canvases.Length; ++i)
			{
				canvases[i].sortingOrder += increment;
			}
			
			return maxPanelDepth + increment + 1;
		}


		UIScreenBase LoadScreen(UIScreenType screenType)
		{
			string fileName;
			m_prefabFiles.TryGetValue(screenType, out fileName);
			if(fileName == null)
			{
				Debug.LogError("Attempting to load screen '" + screenType.ToString() + "' which does not exist in UIManagerData!");
				return null;
			}

			UIScreenBase prefab = Resources.Load<UIScreenBase>(fileName);
			if(prefab == null)
			{
				Debug.LogError("Could not load the ui prefab file: " + fileName);
				return null;
			}

			UIScreenBase screen = GameObject.Instantiate(prefab) as UIScreenBase;

			GameObject go = screen.gameObject;
			go.transform.SetParent(gameObject.transform);
			go.SetActive(true);

			return screen;
		}

		
		void InitializePrefabFiles()
		{
			for(int i=0; i<m_uiPrefabs.Count; ++i)
			{
				UIScreenPrefabFile screenData = m_uiPrefabs[i];


                if (screenData.uiScreenType == UIScreenType.None)
                {
                    Debug.LogWarning("The UIManager has a screen specified for Screen Type 'None'!");
                    continue;
                }

                if (string.IsNullOrEmpty(screenData.fileName))
				{
					Debug.LogWarning("The UIManager has an empty filename specified for one of its screens!");
					continue;
				}
				
				if(m_prefabFiles.ContainsKey(screenData.uiScreenType))
				{
					Debug.LogWarning("The UIManager contains a duplicate entry for " + screenData.uiScreenType.ToString());
					continue;
				}
				
				m_prefabFiles[screenData.uiScreenType] = screenData.fileName;
			}
		}
	}
}