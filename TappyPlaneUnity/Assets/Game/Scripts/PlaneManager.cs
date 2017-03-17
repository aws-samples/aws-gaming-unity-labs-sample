using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


[Serializable]
public class AssetList
{
    public List<string> planePacks;
}


public class PlaneManager : MonoBehaviour
{
    [SerializeField] string m_S3BucketUrl;
    [SerializeField] string m_assetlist;

    static int DEFAULT_PLANEID = 0;
    static string DEFAULT_BUNDLE = "baseplanepack";

    int m_currentPlaneId = DEFAULT_PLANEID;
    string m_currentPlaneBundle = DEFAULT_BUNDLE;

    Dictionary<string, PlayerPrefabItemList> m_prefabdata = new Dictionary<string, PlayerPrefabItemList>();

    void Awake()
    {
        LoadBundle(DEFAULT_BUNDLE);
    }

    void Start()
    {
       StartCoroutine(RetrieveAssetBundles());
    }

    public int GetCurrentPlaneId()
    {
        return m_currentPlaneId;
    }


    public void SetCurrentPlaneId(string bundleName, int id)
    {
        m_currentPlaneId = id;
        m_currentPlaneBundle = bundleName;
    }


    public Dictionary<string, PlayerPrefabItemList> GetPrefabData()
    {
        return  m_prefabdata;
    }


    public TappyPlayer InstantiatePlayer()
    {
        AssetBundle bundle = m_prefabdata[m_currentPlaneBundle].Bundle;
        string planeFile = GetPlanePrefab();

        GameObject prefab = bundle.LoadAsset<GameObject>(planeFile);
        if(prefab == null)
        {
            Debug.LogError("The bundle " + m_currentPlaneBundle + " does not contain " + planeFile);
            return null;
        }

        return GameObject.Instantiate<TappyPlayer>(prefab.GetComponent<TappyPlayer>());
    }


    string GetPlanePrefab()
    {
        PlayerPrefabItemList prefabData = m_prefabdata[m_currentPlaneBundle];
        for(int i = 0; i < prefabData.playerPrefabItems.Count; ++i)
        {
            if(prefabData.playerPrefabItems[i].id == m_currentPlaneId)
            {
                return prefabData.playerPrefabItems[i].prefabFile;
            }
        }

        return null;
    }


    void LoadBundle(string bundleName)
    {
        AssetBundle bndl = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, bundleName));
        if(bndl == null)
        {
            Debug.Log("Failed to load AssetBundle: " + bundleName);
            return;
        }

        bndl.LoadAllAssets();

        PlayerPrefabItemList list = bndl.LoadAsset<PlayerPrefabItemList>("PlaneList");
        list.Bundle = bndl;
        m_prefabdata[bundleName] = list;
    }


    IEnumerator RetrieveAssetBundles()
    {
        //retrieve the asset list
        if(string.IsNullOrEmpty(m_S3BucketUrl) || string.IsNullOrEmpty(m_assetlist))
        {
            Debug.LogWarning("S3 data not specified, new planes will not be downloaded from the cloud.");
            yield break;
        }

        string url = m_S3BucketUrl + m_assetlist;
        UnityWebRequest wrq = UnityWebRequest.Get(url);
        yield return wrq.Send();

        if(wrq.isError)
        {
            Debug.LogError(wrq.error);
            yield break;
        }
        
        //download the bundle
        AssetList list = JsonUtility.FromJson<AssetList>(wrq.downloadHandler.text);
        for(int i=0; i<list.planePacks.Count; ++i)
        {
            string bundleName = list.planePacks[i];

            string bundleURL = m_S3BucketUrl + bundleName;
            UnityWebRequest www = UnityWebRequest.GetAssetBundle(bundleURL);
            yield return www.Send();

            if(www.isError)
            {
                Debug.Log(www.error);
                continue;
            }
            
            AssetBundle bundle = ((DownloadHandlerAssetBundle)www.downloadHandler).assetBundle;
            PlayerPrefabItemList itemList = bundle.LoadAsset<PlayerPrefabItemList>("PlaneList");
            itemList.Bundle = bundle;
            m_prefabdata[bundleName] = itemList;
        }
    }
}


