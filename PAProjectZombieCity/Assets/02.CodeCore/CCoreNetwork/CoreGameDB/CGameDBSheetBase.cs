using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine;
using System.Reflection;

// [주의] 암호화 객체는 맴버로 int / uint / ulong /  float / bool / string / array / List 만 가져야 한다.
//        내부적으로 많은 메모리를 할당하고 계산하므로 케시 프로세스를 활용하여 입출력 타이밍을 계산해야 한다.
//        
public abstract class CGameDBSheetBase
{
	private class SEncryptClass
	{
        public CEncryptionValueRootBase MemberSingleValue = null;
        public Dictionary<string, CEncryptionValueRootBase>       MemberMapValue    = new Dictionary<string, CEncryptionValueRootBase>();
		public CMultiDictionary<string, SEncryptClass>            MemberMapObject   = new CMultiDictionary<string, SEncryptClass>();
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

		RecursiveGameDBEncrypt(m_pEncryptClass, pInstance);
		OnGameDBSheetUpdate();
	}

	protected void ProtGameDBSheetDecrypt<TEMPLATE>(TEMPLATE pInstance) where TEMPLATE : class
	{
		if (pInstance == null) return;
		RecursiveGameDBDecrypt(m_pEncryptClass, pInstance);
	}

	//----------------------------------------------------------------------------
	private void RecursiveGameDBEncrypt<TEMPLATE>(SEncryptClass pEnClass, TEMPLATE pInstance) where TEMPLATE : class
	{
        if (pInstance == null) return;

		Type pInstanceType = pInstance.GetType();

        if (pInstanceType.BaseType == typeof(System.ValueType))
        {
            PrivGameDBEncryptValue(pEnClass, pInstance);
        }
        else
        {
            PrivGameDBEncryptObject(pEnClass, pInstance);
        }      
	}

    private void PrivGameDBEncryptObject<TEMPLATE>(SEncryptClass pEnClass,  TEMPLATE pInstance) where TEMPLATE : class
    {
        Type pInstanceType = pInstance.GetType();
        FieldInfo[] aFieldInfo = pInstanceType.GetFields();

        for (int i = 0; i < aFieldInfo.Length; i++)
        {
            FieldInfo pFieldInfo = aFieldInfo[i];

            if (pFieldInfo.DeclaringType != pInstanceType)  // 최 하위 맴버변수만 암호화 하고 상위 클레스 변수는 하지 않는다. 대부분 상위 변수는 식별용 정보이기 때문
            {
                continue;
            }

            if (pFieldInfo.FieldType.BaseType == typeof(System.Array))
            {
                PrivGameDBEncryptMemberArray(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.BaseType == typeof(System.ValueType) || pFieldInfo.FieldType == typeof(string))
            {
                PrivGameDBEncryptMemberValue(pEnClass.MemberMapValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.IsGenericType == false && pFieldInfo.FieldType.BaseType == typeof(System.Object))
            {
                PrivGameDBEncryptMemberObject(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.IsGenericType && pFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                PrivGameDBEncryptMemberList(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
        }
    }

    private void PrivGameDBEncryptValue(SEncryptClass pEnClass, object pInstance)
    {
        Type pInstanceType = pInstance.GetType();
        if (pInstanceType == typeof(int))
        {
            pEnClass.MemberSingleValue = new EnInt();
            ((EnInt)pEnClass.MemberSingleValue).Value = (int)(pInstance);
        }
        else if (pInstanceType == typeof(uint))
        {
            pEnClass.MemberSingleValue = new EnUInt();
            ((EnUInt)pEnClass.MemberSingleValue).Value = (uint)(pInstance);
        }
        else if (pInstanceType == typeof(ulong))
        {
            pEnClass.MemberSingleValue = new EnULong();
            ((EnULong)pEnClass.MemberSingleValue).Value = (ulong)(pInstance);
        }
        else if (pInstanceType == typeof(bool))
        {
            pEnClass.MemberSingleValue = new EnBool();
            ((EnBool)pEnClass.MemberSingleValue).Value = (bool)(pInstance);
        }
        else if (pInstanceType == typeof(float))
        {
            pEnClass.MemberSingleValue = new EnFloat();
            ((EnFloat)pEnClass.MemberSingleValue).Value = (float)(pInstance);
        }
        else if (pInstanceType == typeof(string))
        {
            pEnClass.MemberSingleValue = new EnString();
            ((EnString)pEnClass.MemberSingleValue).Value = (string)(pInstance);
        }
        else
        {
            Debug.LogError("[GameDB] Invalid FieldType. Must be int / uint/ ulong / float / bool / string.");
        }
    }

	//-----------------------------------------------------------------------------
	private void PrivGameDBEncryptMemberValue(Dictionary<string, CEncryptionValueRootBase> Member, FieldInfo pFieldInfo, object pInstance)
	{
		CEncryptionValueRootBase pEncryptValue = null;
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
            else if (pFieldInfo.FieldType == typeof(ulong))
            {
                pEncryptValue = new EnULong();
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
				Debug.LogError("[GameDB] Invalid FieldType. Must be int / uint/ ulong / float / bool / string.");
			}
			Member[strValueName] = pEncryptValue;
		}

		if (pFieldInfo.FieldType == typeof(int))
		{
			if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Int)
			{
				((EnInt)pEncryptValue).Value = (int)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(uint))
		{
			if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.UInt)
			{
				((EnUInt)pEncryptValue).Value = (uint)pFieldInfo.GetValue(pInstance);
			}
		}
        else if (pFieldInfo.FieldType == typeof(ulong))
        {
            if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.ULong)
            {
                ((EnULong)pEncryptValue).Value = (ulong)pFieldInfo.GetValue(pInstance);
            }
        }
        else if (pFieldInfo.FieldType == typeof(bool))
		{
			if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Bool)
			{
				((EnBool)pEncryptValue).Value = (bool)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(float))
		{
			if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Float)
			{
				((EnFloat)pEncryptValue).Value = (float)pFieldInfo.GetValue(pInstance);
			}
		}
		else if (pFieldInfo.FieldType == typeof(string))
		{
			if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.String)
			{
				((EnString)pEncryptValue).Value = (string)pFieldInfo.GetValue(pInstance);
			}
		}
	}

    private void PrivGameDBEncryptMemberObject(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.Name;
		SEncryptClass pChildEncryptClass = new SEncryptClass();
		MemberArray.Add(strClassName, pChildEncryptClass);
		object pChildInstance = pFieldInfo.GetValue(pInstance);
		RecursiveGameDBEncrypt(pChildEncryptClass, pChildInstance);
	}

	private void PrivGameDBEncryptMemberArray(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		object[] aArrayInstance = (object[])pFieldInfo.GetValue(pInstance);

		if (aArrayInstance == null) return;

		string strClassName = pFieldInfo.Name;
		List<SEncryptClass> pListEnClass = MemberArray[strClassName];

		if (aArrayInstance.Length > pListEnClass.Count)
		{
			int Count = aArrayInstance.Length - pListEnClass.Count;
			for (int i = 0; i < Count; i++)
			{
                if (aArrayInstance[i] != null)
                {
                    pListEnClass.Add(new SEncryptClass());
                }
            }
		}
		else if (aArrayInstance.Length < pListEnClass.Count)
		{
			int Count = pListEnClass.Count - aArrayInstance.Length;
			for (int i = 0; i < Count; i++)
			{
                if (aArrayInstance[i] != null)
                {
                    pListEnClass.RemoveAt(pListEnClass.Count - 1);
                }
            }
		}

		for (int i = 0; i < pListEnClass.Count; i++)
		{
            RecursiveGameDBEncrypt(pListEnClass[i], aArrayInstance[i]);
        }
    }

    private void PrivGameDBEncryptMemberList(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
    {
        IList pListType = (IList)pFieldInfo.GetValue(pInstance);

        if (pListType == null) return;

        string strClassName = pFieldInfo.Name;
        List<SEncryptClass> pListEnClass = MemberArray[strClassName];

        if (pListType.Count > pListEnClass.Count)
        {
            int Count = pListType.Count - pListEnClass.Count;
            for (int i = 0; i < Count; i++)
            {
                pListEnClass.Add(new SEncryptClass());
            }
        }
        else if (pListType.Count < pListEnClass.Count)
        {
            int Count = pListEnClass.Count - pListType.Count;
            for (int i = 0; i < Count; i++)
            {
                pListEnClass.RemoveAt(pListEnClass.Count - 1);
            }
        }

        for (int i = 0; i < pListType.Count; i++)
        {
            RecursiveGameDBEncrypt(pListEnClass[i], pListType[i]);
        }
    }

    //--------------------------------------------------------------------------------
    private void RecursiveGameDBDecrypt<TEMPLATE>(SEncryptClass pEnClass, TEMPLATE pInstance) where TEMPLATE : class
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
                PrivGameDBDecryptMemberArray(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.BaseType == typeof(System.ValueType) || pFieldInfo.FieldType == typeof(string))
            {
                PrivGameDBDecryptMemberValue(pEnClass.MemberMapValue, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.IsGenericType == false && pFieldInfo.FieldType.BaseType == typeof(System.Object))
            {
                PrivGameDBDecryptMemberObject(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
            else if (pFieldInfo.FieldType.IsGenericType && pFieldInfo.FieldType.GetGenericTypeDefinition() == typeof(List<>))
            {
                PrivGameDBDecryptMemberList(pEnClass.MemberMapObject, pFieldInfo, pInstance);
            }
        }
    }

    private void PrivGameDBDecryptMemberValue(Dictionary<string, CEncryptionValueRootBase> Member, FieldInfo pFieldInfo, object pInstance)
	{
		CEncryptionValueRootBase pEncryptValue = null;
		string strValueName = pFieldInfo.Name;
		if (Member.ContainsKey(strValueName))
		{
			pEncryptValue = Member[strValueName];
		}

		if (pEncryptValue != null)
		{
			if (pFieldInfo.FieldType == typeof(int))
			{
				if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Int)
				{
					pFieldInfo.SetValue(pInstance, ((EnInt)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(uint))
			{
				if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.UInt)
				{
					pFieldInfo.SetValue(pInstance, ((EnUInt)pEncryptValue).Value);
				}
			}
            else if (pFieldInfo.FieldType == typeof(ulong))
            {
                if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.ULong)
                {
                    pFieldInfo.SetValue(pInstance, ((EnULong)pEncryptValue).Value);
                }
            }
            else if (pFieldInfo.FieldType == typeof(bool))
			{
				if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Bool)
				{
					pFieldInfo.SetValue(pInstance, ((EnBool)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(float))
			{
				if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.Float)
				{
					pFieldInfo.SetValue(pInstance, ((EnFloat)pEncryptValue).Value);
				}
			}
			else if (pFieldInfo.FieldType == typeof(string))
			{
				if (pEncryptValue.ValueType == CEncryptionValueRootBase.EEncryptValueType.String)
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

	private void PrivGameDBDecryptMemberObject(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.Name;
		List<SEncryptClass> pListEnClass = null;
		SEncryptClass pEncryptClass = null;
		if (MemberArray.ContainsKey(strClassName))
		{
			pListEnClass = MemberArray[strClassName];
			if (pListEnClass.Count == 1)
			{
				pEncryptClass = pListEnClass[0];
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

		RecursiveGameDBDecrypt(pEncryptClass, pNewInstance);
	}

	private void PrivGameDBDecryptMemberArray(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
	{
		string strClassName = pFieldInfo.Name;
		List<SEncryptClass> pListEnClass = null;
		if (MemberArray.ContainsKey(strClassName))
		{
			pListEnClass = MemberArray[strClassName];
		}
		else
		{
			Debug.LogError(string.Format("[GameDB] Invalid Decrypt class Name : {0}", strClassName));
			return;
		}

		Type pElementType = pFieldInfo.FieldType.GetElementType();
		object[] aNewInstance = (object [])pFieldInfo.GetValue(pInstance); 

		if (aNewInstance == null)
		{
			aNewInstance = (object[])Array.CreateInstance(pElementType, pListEnClass.Count);
			pFieldInfo.SetValue(pInstance, aNewInstance);
		}

		for (int i = 0; i < pListEnClass.Count; i++)
		{
			if (aNewInstance[i] == null)
			{
				aNewInstance[i] = Activator.CreateInstance(pElementType);
			}
			RecursiveGameDBDecrypt(pListEnClass[i], aNewInstance[i]);
		}
	}

    private void PrivGameDBDecryptMemberList(CMultiDictionary<string, SEncryptClass> MemberArray, FieldInfo pFieldInfo, object pInstance)
    {
        string strClassName = pFieldInfo.Name;
        List<SEncryptClass> pListEnClass = null;
        if (MemberArray.ContainsKey(strClassName))
        {
            pListEnClass = MemberArray[strClassName];
        }
        else
        {
            Debug.LogError(string.Format("[GameDB] Invalid Decrypt class Name : {0}", strClassName));
            return;
        }

        Type [] aArgumentType = pFieldInfo.FieldType.GetGenericArguments();
        Type pArgumentType = null;

        if (aArgumentType.Length == 0) return;

        pArgumentType = aArgumentType[0];  // 다중 제너릭은 허용 하지 않는다.

        IList pListInstance = (IList)pFieldInfo.GetValue(pInstance);
        pListInstance.Clear();

        if (pArgumentType.BaseType == typeof(System.Object))
        {
            for (int i = 0; i < pListEnClass.Count; i++)
            {
                object pNewInstance = Activator.CreateInstance(pArgumentType);
                pListInstance.Add(pNewInstance);

                RecursiveGameDBDecrypt(pListEnClass[i], pNewInstance);
            }
        }
        else if (pArgumentType.BaseType == typeof(System.ValueType))
        {
            for (int i = 0; i < pListEnClass.Count; i++)
            {
                if (pArgumentType == typeof(int))
                {
                    EnInt pValue = pListEnClass[i].MemberSingleValue as EnInt;
                    pListInstance.Add(pValue.Value);
                }
                else if (pArgumentType == typeof(uint))
                {
                    EnUInt pValue = pListEnClass[i].MemberSingleValue as EnUInt;
                    pListInstance.Add(pValue.Value);
                }
                else if (pArgumentType == typeof(ulong))
                {
                    EnULong pValue = pListEnClass[i].MemberSingleValue as EnULong;
                    pListInstance.Add(pValue.Value);
                }
                else if (pArgumentType == typeof(bool))
                {
                    EnBool pValue = pListEnClass[i].MemberSingleValue as EnBool;
                    pListInstance.Add(pValue.Value);
                }
                else if (pArgumentType == typeof(float))
                {
                    EnFloat pValue = pListEnClass[i].MemberSingleValue as EnFloat;
                    pListInstance.Add(pValue.Value);
                }
                else if (pArgumentType == typeof(string))
                {
                    EnString pValue = pListEnClass[i].MemberSingleValue as EnString;
                    pListInstance.Add(pValue.Value);
                }
            }
        }
    }

    //---------------------------------------------------------------------------------------------
    protected virtual void OnGameDBSheetUpdate() { }
}

