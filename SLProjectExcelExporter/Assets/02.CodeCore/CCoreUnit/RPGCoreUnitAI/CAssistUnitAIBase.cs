using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AI는 각 스킬 페이즈 단위로 작동하며 하나의 페이즈는 조건 중족시 다른 페이즈로 넘어간다. 
// 각 페이즈는 다수의 스킬 AI로 구성되며 이동 방침과 타겟 방침을 정하여 유닛을 제어하게 된다.


public interface IAssistEventAI
{
    public void IAIEventCrushUnit(CUnitBase pCrushUnit);
  //  public void IAIEventCrushObstacle(CCollisionCheckerObstacleBase pCrushObstacle);
}

public abstract class CAssistUnitAIBase : CAssistUnitBase, IAssistEventAI
{


    //------------------------------------------------------------------
    public void IAIEventCrushUnit(CUnitBase pCrushUnit) 
    {
        OnAssistUnitAICrushUnit(pCrushUnit);
    }

    //public void IAIEventCrushObstacle(CCollisionCheckerObstacleBase pCrushObstacle)
    //{
    //    OnAssistUnitAICrushObstacle(pCrushObstacle);
    //}

    //--------------------------------------------------------------
    protected virtual void OnAssistUnitAICrushUnit(CUnitBase pCrushUnit) { }
 //   protected virtual void OnAssistUnitAICrushObstacle(CCollisionCheckerObstacleBase pCrushObstacle) { }
}
