using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CBuffTaskBase 
{
    public enum EBuffTaskEvent  // 하위 클레스에서 Size뒤에 넘버로 정의 해서 사용할 것
    {
        Start,      // 버프 시작
        End,        // 다양한 이유로 종료
        Expire,     // 버프 지속시간 만료 이후 End호출
        Erase,      // 다양한 이유로 제거됨 이후 End호출

        EventOverTime,

        //-----------------------------
        DamageTo,
        DamageFrom,
        HealTo,
        HealFrom,
        CrowdControlTo,
        CrowdControlFrom,
        SkillTo,
        SkillFrom,
        BuffTo,
        BuffFrom,

        TriggerEnter, // 소유자와 접촉 했을때

    }

    private CBuffTaskTargetListBase    m_pBuffTaskTargetList = null;
    private CBuffTaskConditionList     m_pBuffTaskConditionList = null;
    private List<IUnitEventBuff> m_listTaskTargetNote = new List<IUnitEventBuff>();
    //--------------------------------------------------------------------------------
    internal void InterBuffTaskEvent(CBuffBase pBuffInstance, IUnitEventBuff pOwner, IUnitEventBuff pOrigin, int iArg, float fArg, params object[] aParams)
    {
        if (m_pBuffTaskTargetList != null)
        {
            int iResult = m_pBuffTaskConditionList.InterBuffTaskCondition(pOwner, pOrigin);
            if (iResult == 0)
            {
                PrivBuffTaskEvent(pBuffInstance, pOwner, pOrigin, iArg, fArg, aParams);
                OnBuffTaskCondition(iResult);
            }
        }
        else
        {
            PrivBuffTaskEvent(pBuffInstance, pOwner, pOrigin, iArg, fArg, aParams);
        }
    }

    internal void InterBuffTaskReset()
    {
        OnBuffTaskReset();
    }

    //---------------------------------------------------------------------------------
    private void PrivBuffTaskEvent(CBuffBase pBuffInstance, IUnitEventBuff pOwner, IUnitEventBuff pOrigin, int iArg, float fArg, params object[] aParams)
    {
        m_listTaskTargetNote.Clear();

        if (m_pBuffTaskTargetList != null)
        {
            m_pBuffTaskTargetList.InterBuffTaskTarget(pOwner, pOrigin, m_listTaskTargetNote);
        }
        else
        {
            m_listTaskTargetNote.Add(pOwner);
        }

        OnBuffTaskEvent(pBuffInstance, pOwner, pOrigin, m_listTaskTargetNote, iArg, fArg, aParams);
    }

    //----------------------------------------------------------------------------------
    protected virtual void OnBuffTaskReset() { }
    protected virtual void OnBuffTaskCondition(int iResult) { }
    //----------------------------------------------------------------------------------
    public void InstanceBuffTaskTargetList(CBuffTaskTargetListBase pBuffTaskTargetList)          { m_pBuffTaskTargetList = pBuffTaskTargetList; }
    public void InstanceBuffTaskConditionList(CBuffTaskConditionList pBuffTaskConditionList)     { m_pBuffTaskConditionList = pBuffTaskConditionList; }
    //-----------------------------------------------------------------------------------
    protected virtual void OnBuffTaskEvent(CBuffBase pBuffInstance, IUnitEventBuff pOwner,  IUnitEventBuff pOrigin, List<IUnitEventBuff> pListTaskTarget, int iArg, float fArg, params object[] aParams) { }
}
