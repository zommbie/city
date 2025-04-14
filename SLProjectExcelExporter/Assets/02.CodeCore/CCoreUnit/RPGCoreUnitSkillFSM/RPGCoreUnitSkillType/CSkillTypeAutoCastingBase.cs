using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 주기적으로 특정 스킬을 사용한다. 
// 글로벌 쿨타임에 영향을 받지 않는다.
public abstract class CSkillTypeAutoCastingBase : CSkillTypeActiveBase
{
    private bool m_bAutoCastEnable = false;   
    //-------------------------------------------------
    internal void InterSkillTypeUpdateAutoCasting()
    {
        if (m_bAutoCastEnable)
        {
            OnSkillTypeUpdateAutoCasting();
        }
    }
   
    //--------------------------------------------------
    protected virtual void OnSkillTypeUpdateAutoCasting() { }
}
