using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class PlaneCard : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image m_selecedIndicator;
    [SerializeField] Image m_planeIcon;

    int m_id;
    string m_bundleName;

    public event Action<string, int> ClickedEvent;

    public void Initialize(AssetBundle bundle, string bundleName, string spriteFile, int id, bool selected)
    {
        m_planeIcon.overrideSprite = bundle.LoadAsset<Sprite>(spriteFile);
        m_id = id;
        m_bundleName = bundleName;

        SetSelected(selected);
    }


    public int GetId()
    {
        return m_id;
    }
    

    public void SetSelected(bool selected)
    {
        m_selecedIndicator.gameObject.SetActive(selected);
    }


    public void OnPointerClick(PointerEventData data)
    {
        if(ClickedEvent != null)
        {
            ClickedEvent(m_bundleName, m_id);
        }
    }
}
