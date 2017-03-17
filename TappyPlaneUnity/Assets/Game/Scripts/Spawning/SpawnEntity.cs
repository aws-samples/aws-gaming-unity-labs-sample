using UnityEngine;
using System.Collections.Generic;

public class SpawnEntity : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] float m_speed = 1.0f;
    [SerializeField] float m_minY = -0.5f;
    [SerializeField] float m_maxY = 0.5f;

    [Header("Scoring")]
    [SerializeField] int m_points;
    [SerializeField] bool m_removeOnScoring;

    List<Renderer> m_renderers = new List<Renderer>();
    bool m_wasVisible;
    bool m_finished = false;
    protected bool m_active = false;


    public Bounds RenderBounds { get; set; }


	void Awake()
	{
		GetComponentsInChildren<Renderer>(m_renderers);
	}


    void Start()
    {
        Bounds bounds = new Bounds();
        for(int i=0; i<m_renderers.Count; ++i)
        {
            bounds.Encapsulate(m_renderers[i].bounds);
        }
        RenderBounds = bounds;
        gameObject.SetActive(false);
    }


	void Update()
	{
		if(m_active)
		{
			transform.position += Vector3.right * -m_speed * Time.deltaTime;
		}
	}


	public virtual void Run(Camera camera)
	{
        gameObject.SetActive(true);
		m_active = true;

		Vector3 position = transform.position;
        position.x = camera.FrustumWidth(0) / 2.0f + RenderBounds.extents.x;
        position.y = Random.Range(m_minY, m_maxY);
		transform.position = position;
	}


	public virtual void Pause()
	{
		m_active = false;
	}


	public virtual void Stop()
	{
		m_active = false;
		m_wasVisible = false;
		m_finished = false;

        gameObject.SetActive(false);
	}


	public int GetPoints()
	{
		if(m_removeOnScoring)
		{
			m_finished = true;
		}

		return m_points;
	}


	public bool IsFinished(Camera camera)
	{
		if(m_finished)
		{
			return true;
		}

		for(int i=0; i<m_renderers.Count; ++i)
		{
			if(camera.IsVisible(m_renderers[i].bounds))
			{
				m_wasVisible = true;
				return false;
			}
		}

		if(!m_wasVisible)
		{
			return false;
		}
			
		return true;
	}
}
