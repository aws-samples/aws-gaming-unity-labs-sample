using UnityEngine;

namespace GameServices
{
	public class Rotate : MonoBehaviour 
	{
		[SerializeField] Vector3 m_speed;
		[SerializeField] bool m_rotating;

		void Update()
		{
			if(!m_rotating)
			{
				return;
			}

			transform.Rotate(m_speed * Time.deltaTime);
		}

		public void SetRotating(bool value)
		{
			m_rotating = value;
		}
	}
}