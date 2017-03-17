using UnityEngine;
using System.Collections.Generic;
using System;


public class Services : SingletonPersistentMonoBehaviour<Services>
{
	Dictionary<System.Type, System.Object> m_services = new Dictionary<System.Type, System.Object>();

	protected override void OnAwake ()
	{
		MonoBehaviour[] services = GetComponentsInChildren<MonoBehaviour>();
		for(int i=0; i<services.Length; ++i)
		{
			Set(services[i]);
		}
	}

	public static U Get<U>() where U : class
	{
		if(m_destroyed)
		{
            Debug.LogWarning("Services: Calling Get when the object has already been destroyed.");
			return null;
		}
			
		Type type = typeof(U);

		if( Instance.m_services.ContainsKey(type) )
		{
			return Instance.m_services[type] as U;
		}
			
		return null;
	}
		
	public static void Set<U>(U service) where U : class
	{
		if(m_destroyed)
		{
            Debug.LogWarning("Services: Calling Set when the object has already been destroyed.");
            return;
		}
			
		Type type = service.GetType();

		Instance.m_services[type] = service;
	}
}

