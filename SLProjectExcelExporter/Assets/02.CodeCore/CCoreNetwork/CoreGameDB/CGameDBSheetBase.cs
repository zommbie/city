using System.Collections.Generic;
using System;
using UnityEngine;
using System.Reflection;

// [주의] 암호화 객체는 맴버로 int / uint / float / bool / string 만 가져야 한다.
//        멤버로 클레스를 지원하고 있으나 가급적이면 구조체 스타일로 암호화 하자.

public abstract class CGameDBSheetBase
{
	private class SEncryptClass
	{		
		public Dictionary<string, CEncryptionValueRoot> MapMember   = new Dictionary<string, CEncryptionValueRoot>();
		public CMultiDictionary<string, SEncryptClass>  ArrayMember = new CMultiDictionary<string, SEncryptClass>();
	}

	private SEncryptClass m_pEncryptClass = new SEncryptClass();
	
    //---------------------------------------------------------------------------
	protected void ProtGameDBSheetEncrypt<TEMPLATE>(TEMPLATE pInstance, bool bReset = false) where TEMPLATE : class
	{
		if (bReset)
		{
			m_pEncryptClass = new SEncryptClass();
		}

		if (pInstance == null) return;

		PrivGameDBEncryptRecursive(m_pEncryptClass, pInstance);
		OnGameDBSheetUpdate();
	}

	protected void ProtGameDBSheetDecrypt<TEMPLATE>(TEMPLATE pInstance) where TEMPLATE : class
	{
		if (pInstance == null) return;
		PrivGameDBDecryptRecursive(m_pEncryptClass, pInstance);
	}

	//----------------------------------------------------------------------------
	private void PrivGameDBEncryptRecursive<TEMPLATE>(SEncryptClass pEnClass, TEMPLATE pInstance) where TEMPLATE : class
	{
		Type pInstanceType = pInstance.GetType();
		FieldInfo[] aFieldInfo = pInstanceType.GetFields();

		for (int i = 0; i < aFieldInfo.Length; i++)
		{
			FieldInfo pFieldInfo = aFieldInfo[i];

			if (pFieldInfo.DeclaringType != pInstanceType)  // 최 하위 맴버변수만 암호화 하고 상위 클레스 변수는 하지 않는다.
			{
				continue;
			}

			if (pFieldInfo.FieldType.BaseType == typeof(System.Array))
			{
				PrivGameDBEncryptArray(pEnClass.ArrayMember, pFieldInfo, pInstance);
			}
			else if (pFieldInfo.FieldType.BaseType == typeof(System.Object) && pFieldInfo.FieldType != typeof(string))
			{
				PrivGameDBEncryptObject(pEnClass.ArrayMember, pFieldInfo, pInstance);
			}
			else 
			{
				PrivGameDBEncryptValue(pEnClass.MapMember, pFieldInfo, pInstance);
			}
		}
	}
	//--------------------------------------------------------------------------
	private void PrivGameDBDecryptRecursive<TEMPLATE>(SEncryptClass pEnClass, TEMPLATE pInstance) where TEMPLATE : class
	{		
		Type pInstanceType = pInstance.GetType();
		FieldInfo[] aFieldInfo = pInstanceType.GetFields();

		for (int i = 0; i < aFieldInfo.Length; i++)
		{
			FieldInfo pFieldInfo = aFieldInfo[i];

			if (pFieldInfo.DeclaringType != pInstanceType)  // 최 하위 맴버변수만 암호화 하고 상위 클레스 변수는 하지 않는다.
			{
				continue;
			}

			if (pFieldInfo.FieldType.BaseType == typeof(System.Array))
			{
				PrivGameDBDecryptArray(pEnClass.ArrayMember, pFieldInfo, pInstance);
			}
			else if (pFieldInfo.FieldType.BaseType == typeof(System.Object) && pFieldInfo.FieldType != typeof(string))
			{
				PrivGameDBDecryptObject(pEnClass.ArrayMember, pFieldInfo, pInstance);
			}
			else
			{
				PrivGameDBDecryptValue(pEnClass.MapMember, pFieldInfo, pInstance);
			}
		}
	}

	//-----------------------------------------------------------------------------
	private void PrivGameDBEncryptValue(Dictionary<string, CEncryptionValueRoot> Member, FieldInfo pFieldInfo, object pInstance)
	{
		CEncryptionValueRoot pEncryptValue = null;
		string strValueName = pFieldInfo.Name;
		if (Member.ContainsKey(strValueName))
		{
			pEncryptValue = Member[strValueName];
		}
		else
		{
			if (pFieldInfo.FieldType == typeof(int))
			{
				pEncryptValue = new EnInt();
			}
			else if (pFieldInfo.FieldType == typeof(uint))
			{
				pEncryptValue = new EnUInt();
			}
			else if (pFieldInfo.FieldType == typeof(bool))
			{
				pEncryptValue = new EnBool();
			}
			else if (pFieldInfo.FieldType == typeof(float))
			{
				pEncryptValue = new EnFloat();
			}
			else if (pFieldInfo.FieldType == typeof(string))
			{
				pEncryptValue = new EnString();
			}
			else
			{
				Debug.LogError("[GameDB] Invalid FieldType. Must be int / float / bool / string.");
			}
			Member[strValueName] = pEncryptValue;
		}

		if (pFieldInfo.FieldType == typeof(int))
		{
			if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Int)
			{
				((EnInt)pEncryptValue).Value = (int)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(uint))
		{
			if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.UInt)
			{
				((EnUInt)pEncryptValue).Value = (uint)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(bool))
		{
			if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Bool)
			{
				((EnBool)pEncryptValue).Value = (bool)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(float))
		{
			if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Float)
			{
				((EnFloat)pEncryptValue).Value = (float)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(string))
		{
			if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.String)
			{
				((EnString)pEncryptValue).Value = (string)pFieldInfo.GetValue(pInstance);
			}
		}
	}

	private void PrivGameDBEncryptObject(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.FieldType.Name;
		SEncryptClass pChildEncryptClass = new SEncryptClass();
		MemberArray.Add(strClassName, pChildEncryptClass);
		object pChildInstance = pFieldInfo.GetValue(pInstance);
		PrivGameDBEncryptRecursive(pChildEncryptClass, pChildInstance);
	}

	private void PrivGameDBEncryptArray(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		object[] aArrayInstance = (object[])pFieldInfo.GetValue(pInstance);

		if (aArrayInstance == null) return;

		string strClassName = pFieldInfo.FieldType.Name;
		List<SEncryptClass> pListEnClass = MemberArray[strClassName];

		if (aArrayInstance.Length > pListEnClass.Count)
		{
			int Count = aArrayInstance.Length - pListEnClass.Count;
			for (int i = 0; i < Count; i++)
			{
				pListEnClass.Add(new SEncryptClass());
			}
		}
		else if (aArrayInstance.Length < pListEnClass.Count)
		{
			int Count = pListEnClass.Count - aArrayInstance.Length;
			for (int i = 0; i < Count; i++)
			{
				pListEnClass.RemoveAt(pListEnClass.Count - 1);
			}
		}

		for (int i = 0; i < aArrayInstance.Length; i++)
		{
			PrivGameDBEncryptRecursive(pListEnClass[i], aArrayInstance[i]);
		}
	}

	//--------------------------------------------------------------------------------
	private void PrivGameDBDecryptValue(Dictionary<string, CEncryptionValueRoot> Member, FieldInfo pFieldInfo, object pInstance)
	{
		CEncryptionValueRoot pEncryptValue = null;
		string strValueName = pFieldInfo.Name;
		if (Member.ContainsKey(strValueName))
		{
			pEncryptValue = Member[strValueName];
		}

		if (pEncryptValue != null)
		{
			if (pFieldInfo.FieldType == typeof(int))
			{
				if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Int)
				{
					pFieldInfo.SetValue(pInstance, ((EnInt)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(uint))
			{
				if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.UInt)
				{
					pFieldInfo.SetValue(pInstance, ((EnUInt)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(bool))
			{
				if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Bool)
				{
					pFieldInfo.SetValue(pInstance, ((EnBool)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(float))
			{
				if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.Float)
				{
					pFieldInfo.SetValue(pInstance, ((EnFloat)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(string))
			{
				if (pEncryptValue.p_ValueType == CEncryptionValueRoot.EEncryptValueType.String)
				{
					pFieldInfo.SetValue(pInstance, ((EnString)pEncryptValue).Value);
				}
			}
		}
		else
		{
			// 어레이에  null 할당되었을 경우 
			Debug.LogWarning(string.Format("[GameDB] Invalid Decrypt value Name : {0}", strValueName));
		}
	}

	private void PrivGameDBDecryptObject(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.FieldType.Name;
		List<SEncryptClass> listEnClass = null;
		SEncryptClass pEncryptClass = null;
		if (MemberArray.ContainsKey(strClassName))
		{
			listEnClass = MemberArray[strClassName];
			if (listEnClass.Count == 1)
			{
				pEncryptClass = listEnClass[0];
			}
			else
			{
				Debug.LogError(string.Format("[GameDB] Invalid Decrypt class Count : {0}", strClassName));
			}
		}
		else
		{
			Debug.LogError(string.Format("[GameDB] Invalid Decrypt class Name : {0}", strClassName));
			return;
		}

		object pNewInstance = pFieldInfo.GetValue(pInstance);
		if (pNewInstance == null)
		{
			pNewInstance = Activator.CreateInstance(pFieldInfo.FieldType);
			pFieldInfo.SetValue(pInstance, pNewInstance);
		}

		PrivGameDBDecryptRecursive(pEncryptClass, pNewInstance);
	}

	private void PrivGameDBDecryptArray(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.FieldType.Name;
		List<SEncryptClass> listEnClass = null;
		if (MemberArray.ContainsKey(strClassName))
		{
			listEnClass = MemberArray[strClassName];
		}
		else
		{
			Debug.LogError(string.Format("[GameDB] Invalid Decrypt class Name : {0}", strClassName));
			return;
		}

		Type pElementType = pFieldInfo.FieldType.GetElementType();
		object[] aNewInstance = (object [])pFieldInfo.GetValue(pInstance); 
		if (aNewInstance == null || aNewInstance.Length != listEnClass.Count)
		{
			aNewInstance = (object[])Array.CreateInstance(pElementType, listEnClass.Count);
			pFieldInfo.SetValue(pInstance, aNewInstance);
		}

		for (int i = 0; i < listEnClass.Count; i++)
		{
			if (aNewInstance[i] == null)
			{
				aNewInstance[i] = Activator.CreateInstance(pElementType);
			}
			PrivGameDBDecryptRecursive(listEnClass[i], aNewInstance[i]);
		}
	}

    //---------------------------------------------------------------------------------------------
    protected virtual void OnGameDBSheetUpdate() { }
    protected virtual void OnGameDBSheetRefresh() { }
    protected virtual void OnGameDBSheetLocalDB() { }
}

