using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 인스턴스가 스폰 될 때 리모트 클라이언트에 SimulateProxy를 생성한다.

public abstract class CUnitInstanceBase : CUnitAuthorityBase
{
    [SerializeField][Header("[UnitAssist]")]
    private CAssistUnitAIBase   AIInstance = null;
    [SerializeField]
    private CAssistUnitBuffBase BuffInstance = null;
    [SerializeField]
    private CAssistShapeAnimationBase AnimationInstance = null;
    [SerializeField]
    private CAssistShapeSocketBase SocketInstance = null;
    [SerializeField]
    private CAssistMovementBase MovementInstance = null;
    [SerializeField]
    private CFiniteStateMachineUnitSkillBase FSMInstance = null;
    [SerializeField]
    private CAssistUnitStatBase StatInstance = null;
    [SerializeField]
    private CAssistUnitSummonBase SummonInstance = null;
    [SerializeField]
    private CAssistUnitFormationBase FormationInstance = null;
    [SerializeField]
    private CAssistUnitCurveBase CurveInstance = null;
    [SerializeField]
    private CAssistUnitEffectBase EffectInstance = null;

   
    //----------------------------------------------------------------
    protected override void OnUnitInitialize()
    {
        base.OnUnitInitialize();
        PrivUnitInitialize();
    }

    protected override sealed void ProtUnitAssistAdd(CAssistUnitBase pAssistUnit)
    {
        base.ProtUnitAssistAdd(pAssistUnit);
    }

    //---------------------------------------------------------------
    public CEffectBase GetUnitEffect(string strEffectName) 
    {
        return EffectInstance.GetAssistUnitEffect(strEffectName); 
    }

    public void SetUnitEffectDisable(string strEffectName)
    {
        EffectInstance.SetAssistUnitEffectDisable(strEffectName);
    }

    //----------------------------------------------------------------
    protected virtual void InstanceUnitCurve(CAssistUnitCurveBase pAssistUnitCurve) { }
    protected virtual void InstanceUnitEffect(CAssistUnitEffectBase pAssistUnitEffect) { }
    //----------------------------------------------------------------
    private void PrivUnitInitialize()
    {
        InstanceUnitAI(AIInstance);
        InstanceUnitBuff(BuffInstance);
        InstanceUnitShapeAnimation(AnimationInstance, SocketInstance);
        InstanceUnitMovement(MovementInstance);
        InstanceUnitFSM(FSMInstance);
        InstanceUnitStat(StatInstance);
        InstanceUnitSummon(SummonInstance);
        InstanceUnitFormation(FormationInstance);
        InstanceUnitCurve(CurveInstance);
        InstanceUnitEffect(EffectInstance);

        ProtUnitAssistAdd(AIInstance);
        ProtUnitAssistAdd(BuffInstance);
        ProtUnitAssistAdd(AnimationInstance);
        ProtUnitAssistAdd(SocketInstance);
        ProtUnitAssistAdd(MovementInstance);
  //      ProtUnitAssistAdd(FSMInstance);
        ProtUnitAssistAdd(StatInstance);
        ProtUnitAssistAdd(SummonInstance);
        ProtUnitAssistAdd(FormationInstance);
        ProtUnitAssistAdd(CurveInstance);
        ProtUnitAssistAdd(EffectInstance);
    }


    //-----------------------------------------------------------------

}

