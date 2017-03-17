using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct PlayerPrefabItem
{
    public string iconFile;
    public string prefabFile;
    public int id;
}

public class PlayerPrefabItemList : ScriptableObject
{
    public List<PlayerPrefabItem> playerPrefabItems;

    public AssetBundle Bundle { get; set; }
}
