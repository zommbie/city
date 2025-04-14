using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

// 리플레이 케릭터등 유니티 출력만 담당하는 객체
// 직접 스킬을 사용하는등의 주동적인 작동은 없다 (CUnitCombatBase)에 구현 
// 케릭터의 기본적인 상태 관리및 Assist 이벤트 핸들링 

public abstract class CUnitActorBase : CMonoBase
{
    public enum EActorStatus
    {
        None,
        Loading,        // 객체가 로드중이다. 기타 컴포넌트나 동적 이펙트를 추가로 로딩하고 있다,
        Spawning,       // 상호작용은 불가능하나 월드에 출력이 된 상태다.
        PhaseOut,       // 모습이 보이고 업데이트를 받으나 상호작용을 하지 않는다.
        PhaseIn,        // PhaseOut에서 변경되는 형태로 에니메이션이나 이펙트가 끝나면 Alive로 이행한다.
        Alive,          // 상호작용을 할 수 있다.
        DeathStart,     // 죽는 과정이 시작되었다.
        Dead,           // 죽어서 잔해가 남아있다. 
        Remove,         // 게임에서 제거 되어 메모리 풀로 회수된 상태이다.      
    }

    private uint m_hUnitActorTableID = 0; public uint GetUnitActorTableID() { return m_hUnitActorTableID; }
    private ulong m_hUnitActorIntanceID = 0; public ulong GetUnitActorInstanceID() { return m_hUnitActorIntanceID; }
    private List<CAssistUnitActorBase> m_listAssistUnitActor = new List<CAssistUnitActorBase>();
    //------------------------------------------------------------------
    internal void InterUnitActorInitialize(ulong hUnitCharInstanceID, uint hTableID)
    {
        ProtUnitActorInitialize(hUnitCharInstanceID, hTableID);
    }

    internal void InterUnitActorRemove()
    {
        PrivUnitActorAssistRemove();
        OnUnitActorRemove();
    }

    //-------------------------------------------------------------------
    protected void ProtUnitActorStatus(EActorStatus eStaus, UnityAction delFinish = null)
    {
        for (int i = 0; i < m_listAssistUnitActor.Count; i++)
        {
            m_listAssistUnitActor[i].InterUnitActorStatus(eStaus);
        }

        OnUnitActorStatus(eStaus, delFinish);
    }

    protected void ProtUnitActorReset()
    {
        m_hUnitActorTableID = 0;
        m_hUnitActorIntanceID = 0;
        m_listAssistUnitActor.Clear();
        OnUnitActorReset();
    }

    protected void ProtUnitActorAssistRegist(CAssistUnitActorBase pAssistInstance)
    {
        m_listAssistUnitActor.Add(pAssistInstance);
        pAssistInstance.InterUnitActorInitialize(this);
        OnUnitActorAssistRegist(pAssistInstance);
    }

    protected void ProtUnitActorInitialize(ulong hUnitCharInstanceID, uint hTableID)
    {
        ProtUnitActorReset();
        m_hUnitActorIntanceID = hUnitCharInstanceID;
        m_hUnitActorTableID = hTableID;
        OnUnitActorInitialize();
    }

    //-------------------------------------------------------------
    private void PrivUnitActorAssistRemove()
    {
        for (int i = 0; i < m_listAssistUnitActor.Count; i++)
        {
            m_listAssistUnitActor[i].InterUnitActorRemove();
        }
    }

    //--------------------------------------------------------------
    protected virtual void OnUnitActorInitialize() { }
    protected virtual void OnUnitActorRemove() { }
    protected virtual void OnUnitActorStatus(EActorStatus eCharStatus, UnityAction delFinish) { delFinish?.Invoke(); }
    protected virtual void OnUnitActorAssistRegist(CAssistUnitActorBase pAssistInstance) { }
    protected virtual void OnUnitActorReset() { }
}
