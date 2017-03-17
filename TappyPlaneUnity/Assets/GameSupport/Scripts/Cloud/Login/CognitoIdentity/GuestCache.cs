using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

namespace GameSupport
{
	[Serializable]
	public struct Guest
	{
		public string name;
		public string id;

		public Guest(string inName, string inId) 
		{
			name = inName;
			id = inId;
		}
	}


	[Serializable]
	public class Guests
	{
		public List<Guest> m_guestIds = new List<Guest>();


		public int Count { get { return m_guestIds.Count; } }


		public void Add(string guestName, string guestId)
		{
			int index = m_guestIds.FindIndex((val) => { return (val.id == guestId); });
			if(index == -1)
			{
				m_guestIds.Add(new Guest(guestName, guestId));
			}
		}


		public string GetIdByName(string guestName)
		{
			if(string.IsNullOrEmpty(guestName))
			{
				return null;
			}

			for(int i=0; i<m_guestIds.Count; ++i)
			{
				if(m_guestIds[i].name == guestName)
				{
					return m_guestIds[i].id;
				}
			}

			return null;
		}


		public string GetNameById(string guestId)
		{
			if(string.IsNullOrEmpty(guestId))
			{
				return null;
			}

			for(int i=0; i<m_guestIds.Count; ++i)
			{
				if(m_guestIds[i].id == guestId)
				{
					return m_guestIds[i].name;
				}
			}

			return null;
		}


		public void ClearGuestById(string guestId)
		{
			if(string.IsNullOrEmpty(guestId))
			{
				Debug.LogError("Attempting to clear a guest id with an empty id!");
				return;
			}

			int index = m_guestIds.FindIndex( (val) => { return (val.id == guestId); });
			if(index != -1)
			{
				m_guestIds.RemoveAt (index);
			}
		}
	}

	[Serializable]
	public class GuestIdCache 
	{
		Guests m_guests = new Guests();
		string m_key;


		public GuestIdCache(string key)
		{
			m_key = key;
		}


		public void Add(string guestName, string guestId)
		{
			if(string.IsNullOrEmpty(guestName) || string.IsNullOrEmpty(guestId) )
			{
				Debug.LogError("Attempting to name an id using " + guestName +  " " + guestId);
				return;
			}

			m_guests.Add(guestName, guestId);
		}


		public string GetIdByName(string guestName)
		{
			return m_guests.GetIdByName(guestName);
		}


		public string GetNameById(string guestId)
		{
			return m_guests.GetNameById(guestId);
		}


		public void ClearGuestById(string guestId)
		{
			m_guests.ClearGuestById(guestId);
		}


		public void RetrieveGuestIds()
		{
			if (PlayerPrefs.HasKey(m_key))
			{
				string value = PlayerPrefs.GetString(m_key);

				Debug.Log("CognitoGuestCache: Guest Ids retrieved: " + value);

				if(!string.IsNullOrEmpty(value))
				{
					m_guests = JsonUtility.FromJson<Guests>(value);
				}
			}
		}


		public void StoreGuestIds()
		{
			if(m_guests.Count > 0)
			{
				string value = JsonUtility.ToJson(m_guests);

				Debug.Log("CognitoGuestCache: Guest Ids stored: " + value);

				PlayerPrefs.SetString(m_key, value);
			}
			else
			{
				PlayerPrefs.DeleteKey (m_key);
			}
		}
	}
}
