using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class EnemyEntity : SpawnEntity 
{
	[Header("Warning Indicator")]
	[SerializeField] SpriteRenderer m_indicator;

	IEnumerator m_coroutine;


	public override void Run(Camera camera)
	{
		base.Run(camera);

		m_coroutine = ShowIndicator(camera);
		StartCoroutine(m_coroutine);
	}


	public override void Stop()
	{
		if(m_coroutine != null)
		{
			StopCoroutine(m_coroutine);
			m_coroutine = null;
		}

		m_indicator.gameObject.SetActive(false);

		base.Stop();
	}


	IEnumerator ShowIndicator(Camera camera) 
	{
		m_active = false;

		//position the indicator at the side of the screen
		Vector3 p = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth, 0, camera.nearClipPlane));
		Vector3 current = m_indicator.transform.position;
		current.x = p.x - m_indicator.bounds.extents.x;
		m_indicator.transform.position = current;

		//show the indicator
		m_indicator.gameObject.SetActive(true);
		yield return new WaitForSeconds(1.0f);

		//hide the indicator and start the enemy
		m_indicator.gameObject.SetActive(false);
		m_active = true;
	}

}
