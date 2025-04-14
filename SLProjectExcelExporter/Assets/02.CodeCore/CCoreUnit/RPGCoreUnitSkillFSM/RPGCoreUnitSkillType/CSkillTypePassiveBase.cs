using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시전 즉시 버프 태스크를 발동 시킨다.
public abstract class CSkillTypePassiveBase : CSkillTypeBase
{

    private CSkillTaskContainer m_pSkillTaskContainer = null;
    //-----------------------------------------------------------------------
    internal void InterSkillPassiveProcess()
    {

    }

    //----------------------------------------------------------------------
    public void InstanceSkillTask(CSkillTaskContainer pTaskContainer) { m_pSkillTaskContainer = pTaskContainer; }
}
