using System;
using UnityEngine;
using UnityEngine.UI;
using GameSupport;

namespace Amazon
{
    public class HUD : UIScreenBase
    {
        [SerializeField] Text m_score;
        [SerializeField] GameObject m_getReady;


        protected override void OnInitialize(UIScreenArgs args)
        {
            SetScore(0);
        }


        protected override void OnReleaseResources()
        {
        }


        public void ShowGetReady()
        {
            SetScore(0);
            m_getReady.SetActive(true);

        }


        public void HideGetReady()
        {
            m_getReady.SetActive(false);
        }


        public void Play()
        {
            HideGetReady();
        }


        public void SetScore(int score)
        {
            m_score.text = score.ToString();
        }
    }
}