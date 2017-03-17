using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

public class HorizontalScroller : MonoBehaviour 
{
	[SerializeField] float m_speed;

    List<SpriteRenderer> m_sprites;
	bool m_running;


    void Awake()
    {
        m_sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        Assert.IsTrue(m_sprites.Count > 0);
    }


	void Start()
    {
        LayoutSprites();
	}


	void Update()
	{
		if(m_running)
		{
            Vector3 delta = new Vector3(Time.deltaTime * -m_speed, 0.0f, 0.0f);

			for(int i=0; i<m_sprites.Count; ++i)
            {
                m_sprites[i].transform.Translate(delta);
            }

            if (!Camera.main.IsVisible(m_sprites[0].bounds))
            {
                SpriteRenderer currentSprite = m_sprites[0];
                m_sprites.RemoveAt(0);

                SpriteRenderer lastSprite = m_sprites[m_sprites.Count - 1];
                float newX = lastSprite.transform.position.x +lastSprite.bounds.extents.x + currentSprite.bounds.extents.x;
                
                Vector3 position = currentSprite.transform.position;
                position.x = newX;
                currentSprite.transform.position = position;

                m_sprites.Add(currentSprite);
            }
        }
	}


    void LayoutSprites()
    {
        //spawn enough tiles to fill the screen width
        Camera camera = Camera.main;
        float visibleWidth = camera.FrustumWidth(0.0f);

        float tileWidth = 0;
        for(int i=0; i<m_sprites.Count; ++i)
        {
            tileWidth += m_sprites[i].bounds.extents.x * 2.0f;
        }

        int tilesNeeded = (int)System.Math.Ceiling(visibleWidth / tileWidth) + 1;

        int spritesPerTile = m_sprites.Count;
        for(int i=0; i<tilesNeeded-1; ++i)
        {
            for(int j=0; j<spritesPerTile; ++j)
            {
                SpriteRenderer sprite = GameObject.Instantiate<SpriteRenderer>(m_sprites[j]);
                sprite.transform.SetParent(transform);
                sprite.transform.position = m_sprites[j].transform.position;
                m_sprites.Add(sprite);
            }
        }

        //layout the sprites to fill the screen width
        float startPosition = -visibleWidth / 2.0f + m_sprites[0].bounds.extents.x;
        Vector3 spritePosition = m_sprites[0].transform.position;
        spritePosition.x = startPosition;
        m_sprites[0].transform.position = spritePosition;

        for (int i=1; i<m_sprites.Count; ++i)
        {
            startPosition = startPosition + m_sprites[i-1].bounds.extents.x + m_sprites[i].bounds.extents.x;
            spritePosition = m_sprites[i].transform.position;
            spritePosition.x = startPosition;
            m_sprites[i].transform.position = spritePosition;
        }
    }


	public void Run()
	{
		m_running = true;
	}


	public void Pause()
	{
		m_running = false;
	}


	public void Stop()
	{
		m_running = false;
	}
}
