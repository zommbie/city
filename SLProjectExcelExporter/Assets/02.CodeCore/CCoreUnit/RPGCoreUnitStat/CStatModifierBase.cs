using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 원본 스텟에 장비나 패시브, 버프등에 의해 수정되는 값.  스텟은 이 체인을 참조하여 최종 값을 산출한다.
public abstract class CStatModifierBase 
{
    private uint m_hModifierType = 0;         public uint GetModifierType() { return m_hModifierType; }
    
    //---------------------------------------------------
    internal float InterStatModifierRefresh(CStatBase pOwnerStat)
    {
        return OnStatModifierRefresh(pOwnerStat);
    }
    //---------------------------------------------------
    protected void ProtStatModifierType(uint hModifierType)
    {
        m_hModifierType = hModifierType;
    }

    //-------------------------------------------------
    protected virtual float OnStatModifierRefresh(CStatBase pOwnerStat) { return 0; }
}
 