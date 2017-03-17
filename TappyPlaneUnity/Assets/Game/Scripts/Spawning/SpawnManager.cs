using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour 
{
	[Header("Timing")]
	[SerializeField] float m_spawnFrequency = 1.0f;
	[SerializeField] bool m_spawnOnStart;

    [Header("Prefabs")]
    [SerializeField] List<SpawnEntity> m_entityPrefabs;

	bool m_running;
	float m_lastSpawnTime;
    Camera m_camera;
	List<SpawnEntity> m_active = new List<SpawnEntity>();
	List<SpawnEntity> m_available = new List<SpawnEntity>();


	void Awake()
	{
        Assert.IsTrue(m_entityPrefabs.Count > 0);
	}


    void Start()
    {
        m_camera = Camera.main;

        for(int i=0; i<m_entityPrefabs.Count; ++i)
        {
            SpawnEntity entity = GameObject.Instantiate<SpawnEntity>(m_entityPrefabs[i]);
            entity.transform.SetParent(transform);
            m_available.Add(entity);
        }

    }


	void Update()
	{
		if(m_running)
		{
			SpawnEntities();
			UpdateEntities();
		}
	}


	public void Run()
	{
		m_running = true;
		if(m_spawnOnStart)
		{
			m_lastSpawnTime = Time.time - m_spawnFrequency;
		}
		else
		{
			m_lastSpawnTime = Time.time;
		}
	}


	public void Pause()
	{
		m_running = false;

		for(int i=m_active.Count-1; i>=0; --i)
		{
			SpawnEntity current = m_active[i];
			current.Pause();
		}
	}


	public void Stop()
	{
		m_running = false;

		while(m_active.Count > 0)
		{
			SpawnEntity current = m_active[0];
			current.Stop();

			m_active.RemoveAt(0);
			m_available.Add(current);
		}
	}


	void SpawnEntities()
	{
		if( m_available.Count <= 0 || 
			Time.time < m_lastSpawnTime + m_spawnFrequency )
		{
			return;
		}
			
		int selected = Random.Range(0, m_available.Count);
		SpawnEntity current = m_available[selected];

		m_available.Remove(current);
		m_active.Add(current);

		current.Run(m_camera);

		m_lastSpawnTime = Time.time;
	}


	void UpdateEntities()
	{
		for(int i=m_active.Count-1; i>=0; --i)
		{
			SpawnEntity current = m_active[i];

			if(current.IsFinished(m_camera))
			{
				current.Stop();
				m_active.RemoveAt(i);
				m_available.Add(current);
				continue;
			}
		}
	}
}
