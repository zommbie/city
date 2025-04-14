using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CUnitAIBase : CUnitFormationBase
{
    public enum EUnitControlType
    {
        None,
        Player,             // 플레이어 직접조정
        FriendAI,           // 아군 AI에 의해 조정
        FriendPossess,      // 아군 AI에 의해 조정. 정신지배등으로 적군이 아군이 된 경우
        EnemyAI,            // 적군 AI에 의해 조정.
        EnemyPossess,       // 적군 AI에 의해 조정. 정신지배등으로 아군이 적군이 된 경우
    }
  

    private EUnitControlType m_eUnitControlType = EUnitControlType.None; public EUnitControlType GetUnitControlType() { return m_eUnitControlType; }
    private CAssistUnitAIBase m_pAssistAI = null;
    //--------------------------------------------------------
    protected override void OnUnitSkillCrushObject(IUnitEventSkill pOtherObject)
    {
        CUnitBase pUnit = pOtherObject as CUnitBase;
        if (pUnit != null)
        {
            m_pAssistAI.IAIEventCrushUnit(pUnit);
        }
    }
    
    //protected override void OnUnitCollisionCrushObstacle(CCollisionCheckerObstacleBase pCrushObstacle) 
    //{
    //    base.OnUnitCollisionCrushObstacle(pCrushObstacle);
    //    m_pAssistAI.IAIEventCrushObstacle(pCrushObstacle);
    //}

    //----------------------------------------------------------
    protected virtual void InstanceUnitAI(CAssistUnitAIBase pAIInstance) { m_pAssistAI = pAIInstance; }
}
