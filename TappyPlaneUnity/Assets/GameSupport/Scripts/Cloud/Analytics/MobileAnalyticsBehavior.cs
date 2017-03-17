using UnityEngine;
using UnityEngine.Assertions;
using Amazon.MobileAnalytics.MobileAnalyticsManager;
using System.Collections.Generic;

namespace GameSupport
{
    public class MobileAnalyticsBehavior : MonoBehaviour
    {
        [SerializeField] CognitoIdentityBehavior m_cognitoIdentity;
        [SerializeField] string m_analyticsAppId;
        [SerializeField] AWSRegionEnum m_region;

        MobileAnalyticsManager m_analyticsManager;


        public void Start()
        {
            if(m_cognitoIdentity == null || string.IsNullOrEmpty(m_analyticsAppId))
            {
                Debug.LogWarning("MobileAnalytics: Analytics manager is not initialized");
                return;
            }

            m_analyticsManager = MobileAnalyticsManager.GetOrCreateInstance(m_analyticsAppId, 
                m_cognitoIdentity.Credentials,
                AWSRegionHelpers.GetByEnum(m_region));
        }


        public void RecordCustomEvent(string name, Dictionary<string, string> attributes, Dictionary<string, double> metrics)
        {
            Assert.IsFalse(string.IsNullOrEmpty(name));

            if(m_analyticsManager == null)
            {
                return;
            }

            CustomEvent evt = new CustomEvent(name);

            if(attributes != null)
            {
                foreach(var att in attributes)
                {
                    evt.AddAttribute(att.Key, att.Value);
                }
            }

            if(metrics != null)
            {
                foreach(var metric in metrics)
                {
                    evt.AddMetric(metric.Key, metric.Value);
                }
            }

            m_analyticsManager.RecordEvent(evt);
        }


        public void RecordMonetizationEvent(MonetizationEvent evt)
        {
            Assert.IsNotNull(evt);

            if(m_analyticsManager == null)
            {
                return;
            }

            m_analyticsManager.RecordEvent(evt);
        }


        void OnApplicationFocus(bool focus)
        {
            if(m_analyticsManager != null)
            {
                if(focus)
                {
                    m_analyticsManager.ResumeSession();
                }
                else
                {
                    m_analyticsManager.PauseSession();
                }
            }
        }
    }
}
