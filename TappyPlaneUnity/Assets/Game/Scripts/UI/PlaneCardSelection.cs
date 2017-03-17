using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using GameSupport;

namespace Amazon
{
    public class PlaneCardSelection : MonoBehaviour
    {
        [SerializeField] GridLayoutGroup m_grid;
        [SerializeField] PlaneCard m_cardPrefab;

        ScrollRect m_scrollRect;
        bool m_justInitialized;

        void Awake()
        {
            m_scrollRect = GetComponent<ScrollRect>();
        }


        void Update()
        {
            if(m_justInitialized)
            {
                m_scrollRect.horizontalNormalizedPosition = 0.0f;
                m_justInitialized = false;
            }
        }

        public void Initialize()
        {
            PlaneManager gamePreferences = Services.Get<PlaneManager>();

            int current = gamePreferences.GetCurrentPlaneId();

            Dictionary<string, PlayerPrefabItemList> prefabData = gamePreferences.GetPrefabData();
            foreach(var entry in prefabData)
            {
                var items = entry.Value.playerPrefabItems;
                for(int i = 0; i < items.Count; ++i)
                {
                    PlaneCard card = GameObject.Instantiate<PlaneCard>(m_cardPrefab);
                    card.transform.SetParent(m_grid.transform, false);
                    card.Initialize(entry.Value.Bundle, entry.Key, items[i].iconFile, items[i].id, current == items[i].id);
                    card.ClickedEvent += HandleCardClicked;
                }
            }

            m_justInitialized = true;
        }


        void HandleCardClicked(string bundleName, int id)
        {
            Services.Get<PlaneManager>().SetCurrentPlaneId(bundleName, id);

            PlaneCard[] cards = m_grid.GetComponentsInChildren<PlaneCard>();
            for(int i=0; i<cards.Length; ++i)
            {
                int cardId = cards[i].GetId();
                cards[i].SetSelected(cardId == id);
            }
        }
    }
}