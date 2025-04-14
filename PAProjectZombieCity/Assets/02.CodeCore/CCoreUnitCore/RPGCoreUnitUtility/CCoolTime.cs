using System.Collections.Generic;
using System;

public class CCoolTime 
{
	private class SCoolTimeInfo : CObjectInstancePoolBase<SCoolTimeInfo>
	{
		public string strCoolTimeName;
		public float  fCoolTime = 0;
	}

	private LinkedList<SCoolTimeInfo> m_listCoolTime = new LinkedList<SCoolTimeInfo>();
	private event Action<string, float> m_eventCoolTimeRemainUpdate;
	//------------------------------------------------------
	public void CooltimeReceiverAdd(Action<string, float> delCoolTimeRemainUpdate)
	{
		m_eventCoolTimeRemainUpdate += delCoolTimeRemainUpdate;
	}

	public void CoolTimeReceiverDelete(Action<string, float> delCoolTimeRemainUpdate)
	{
		m_eventCoolTimeRemainUpdate -= delCoolTimeRemainUpdate;
	}

	public float GetCoolTime(string strCoolTimeName)
	{
		float fCoolTime = 0;
		SCoolTimeInfo pCoolTime = FindCoolTimeInfo(strCoolTimeName);
		if (pCoolTime != null)
		{
			fCoolTime = pCoolTime.fCoolTime;
		} 
		return fCoolTime;
	}

	public void SetCoolTime(string strCoolTimeName, float fCoolTime)
	{
		SCoolTimeInfo pCoolTime = FindCoolTimeInfo(strCoolTimeName);
		if (pCoolTime != null)
		{
			if (pCoolTime.fCoolTime < fCoolTime)
			{
				pCoolTime.fCoolTime = fCoolTime;
			}
		}
		else
		{
			pCoolTime = SCoolTimeInfo.InstancePoolMake<SCoolTimeInfo>();
			pCoolTime.fCoolTime = fCoolTime;
			pCoolTime.strCoolTimeName = strCoolTimeName;
			m_listCoolTime.AddLast(pCoolTime);
		}
		PrivCoolTimeStart(strCoolTimeName, fCoolTime);
	}

	public void SetCoolTimeReduce(string strCoolTimeName, float fReduceValue)
	{
		SCoolTimeInfo pCoolTime = FindCoolTimeInfo(strCoolTimeName);
		if (pCoolTime != null)
		{
			pCoolTime.fCoolTime -= fReduceValue;
			if (pCoolTime.fCoolTime <= 0)
			{
				PrivCoolTimeEnd(pCoolTime);
			}
		}
	}

	//------------------------------------------------------
	public void UpdateCoolTime(float fDelta)
	{
		LinkedList<SCoolTimeInfo>.Enumerator it = m_listCoolTime.GetEnumerator();
		List<SCoolTimeInfo> pListDelete = new List<SCoolTimeInfo>();
		while(it.MoveNext())
		{
			it.Current.fCoolTime -= fDelta;
			if (it.Current.fCoolTime <= 0)
			{
				pListDelete.Add(it.Current);
			}
			else
			{
				m_eventCoolTimeRemainUpdate?.Invoke(it.Current.strCoolTimeName, it.Current.fCoolTime);
			}
		}

		for (int i = 0; i < pListDelete.Count; i++)
		{
			PrivCoolTimeEnd(pListDelete[i]);
		}
	}

	//------------------------------------------------------
	private void PrivCoolTimeEnd(SCoolTimeInfo pCoolTime)
	{
		m_listCoolTime.Remove(pCoolTime);
		SCoolTimeInfo.InstancePoolReturn(pCoolTime);
		m_eventCoolTimeRemainUpdate?.Invoke(pCoolTime.strCoolTimeName, 0);
	}

	private void PrivCoolTimeStart(string strCoolTimeName, float fCoolTime)
	{
		m_eventCoolTimeRemainUpdate?.Invoke(strCoolTimeName, fCoolTime);
	}

	private SCoolTimeInfo FindCoolTimeInfo(string strCoolTimeName)
	{
		SCoolTimeInfo pCoolTimeInfo = null;
		foreach(SCoolTimeInfo pItem in m_listCoolTime)
		{
			if (pItem.strCoolTimeName == strCoolTimeName)
			{
				pCoolTimeInfo = pItem;
				break;
			}
		}

		return pCoolTimeInfo;
	}
}
