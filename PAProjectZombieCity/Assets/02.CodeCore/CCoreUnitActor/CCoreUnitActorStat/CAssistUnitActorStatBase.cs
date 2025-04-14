using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 실제 스텟 처리와 계산은 COperatorStat에서 하고 어시스트는 출력정보만 저장한다. 
// UI 입출력등 

public abstract class CAssistUnitActorStatBase : CAssistUnitActorBase
{
    protected class SUnitActorStatValue
    {
        public int   StatType;
        public float StatValueFloat;
        public int   StatValueInt;
    }

    private Dictionary<int, SUnitActorStatValue> m_mapUnitActorStat = new Dictionary<int, SUnitActorStatValue>();
    //-----------------------------------------------------------------------
    protected void ProtActorStatAddValue(int eStatType)
    {
        if (m_mapUnitActorStat.ContainsKey(eStatType) == false)
        {
            SUnitActorStatValue pStatValue = new SUnitActorStatValue();
            pStatValue.StatType = eStatType;
            m_mapUnitActorStat[eStatType] = pStatValue;
        }
    }

    protected SUnitActorStatValue FindActorStateValue(int eStatType)
    {
        SUnitActorStatValue pFindStat = null;
        m_mapUnitActorStat.TryGetValue(eStatType, out pFindStat);
        return pFindStat;
    }
    //------------------------------------------------------------------------
}
