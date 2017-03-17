using UnityEngine;
using System;
using System.Collections;

public class TappyPlayer : MonoBehaviour 
{
	[Header("Physics")]
	[SerializeField] float m_jump = 300.0f;
	[SerializeField] Vector2 m_minVelocity = new Vector2(0.0f, -20.0f);
	[SerializeField] Vector2 m_maxVelocity = new Vector2(0.0f,  20.0f);

	[Header("Smoke Effect")]
	[SerializeField] ParticleSystem m_smokeEffect;

	[Header("Plane Tilt")]
	[SerializeField] SpriteRenderer m_planeSprite;
	[SerializeField] float m_minRotation = -10.0f;
	[SerializeField] float m_maxRotation = 10.0f;

	[Header("Death Animation")]
	[SerializeField] float m_deathRotationSpeed = 180.0f;


	Vector2 m_jumpForce;
	Rigidbody2D m_rigidBody;
	Collider2D m_collider;
	bool m_alive = false;

	public event Action PlayerDyingEvent;
	public event Action PlayerDeathEvent;
	public event Action<int> PlayerScoreEvent;


	void Awake()
	{
		m_jumpForce = new Vector2(0.0f, m_jump);
		m_rigidBody = GetComponent<Rigidbody2D>();
		m_collider = GetComponentInChildren<Collider2D>();
		Pause();
	}


	public void Update()
	{
		if(!m_alive)
		{
			return;
		}

		//Handle Plane Tilt
		float t = (Mathf.Clamp(m_rigidBody.velocity.y, -1.0f, 1.0f) + 1.0f ) * 0.5f;
		float zRot = Mathf.Lerp(m_minRotation, m_maxRotation, t);

		Vector3 rotation = m_planeSprite.transform.localEulerAngles;
		rotation.z = zRot;
		m_planeSprite.transform.localEulerAngles = rotation;
	}


	public void Flap()
	{
		if(!m_alive)
		{
			return;
		}

		m_rigidBody.AddForce(m_jumpForce);

		Vector2 currentVelocity = m_rigidBody.velocity;
		m_rigidBody.velocity = Vector2.Max(m_minVelocity, Vector2.Min(m_maxVelocity, currentVelocity));
	}


	public void Run()
	{
		m_alive = true;
		m_rigidBody.isKinematic = false;
		m_collider.enabled = true;
		m_planeSprite.transform.localEulerAngles = Vector3.zero;
		m_planeSprite.enabled = true;
		m_smokeEffect.Play();
	}


	public void Pause()
	{
		m_alive = false;
		m_rigidBody.isKinematic = true;
		m_planeSprite.enabled = true;
		m_smokeEffect.Stop();
	}
		

	public void Stop()
	{
		m_alive = false;
		m_rigidBody.isKinematic = true;
		m_rigidBody.velocity = Vector3.zero;
		transform.position = Vector3.zero;
		m_planeSprite.transform.localEulerAngles = Vector3.zero;
		m_planeSprite.enabled = false;
		m_smokeEffect.Stop();
	}


	void OnCollisionEnter2D(Collision2D coll) 
	{
		StartCoroutine(DeathAnimation());
	}


	void OnTriggerEnter2D(Collider2D other) 
	{
		if(m_alive && PlayerScoreEvent != null)
		{
			SpawnEntity entity = other.GetComponentInParent<SpawnEntity>();
			if(entity != null)
			{
				PlayerScoreEvent(entity.GetPoints());
			}
		}
	}


	IEnumerator DeathAnimation()
	{
		m_alive = false;
		m_collider.enabled = false;

		if(PlayerDyingEvent != null)
		{
			PlayerDyingEvent();
		}

        Camera gameCamera = Camera.main;
		while(gameCamera.IsVisible(m_planeSprite.bounds))
		{
			Vector3 rotation = m_planeSprite.transform.localEulerAngles;
			rotation.z += m_deathRotationSpeed * Time.deltaTime;
			m_planeSprite.transform.localEulerAngles = rotation;
			yield return null;
		}

		if(PlayerDeathEvent != null)
		{
			PlayerDeathEvent();
		}
	}
}
