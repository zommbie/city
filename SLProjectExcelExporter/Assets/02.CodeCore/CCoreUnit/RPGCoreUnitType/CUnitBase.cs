using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// 유닛의 상태가 변경되어야 할 경우 메니저에게 통보해야 하며 메니저는 해당 상황을 네트워크등으로 전송하여 동기화 한 후 inter함수로 인스턴스를 갱신한다.
//
//
public abstract class CUnitBase : CMonoBase
{
  
    public enum EUnitState
    {
        None,
        Loaded,         // 메모리 풀에서 로드되었다. 기타 컴포넌트나 동적 이펙트등을 추가로 로딩하는 단계
        Spawning,       // 상호작용은 불가능이나 월드에 출력이 된다.
        PhaseOut,       // 모습이 보이고 업데이트를 받으나 상호작용을 하지 않는다.
        PhaseIn,        // PhaseOut에서 변경되는 형태로 에니메이션이나 이펙트가 끝나면 Alive로 이행한다.
        Alive,          // 상호작용을 할 수 있다.
        DeathStart,     // 죽는 과정이 시작되었다.
        Dead,           // 죽어서 잔해가 남아있다. 
        Remove,         // 게임에서 제거 되어 메모리 풀로 회수된 상태이다.        
    }

    //---------------------------------------------------------------------------    
    private EUnitState              m_eUnitState = EUnitState.None;              public EUnitState GetUnitState() { return m_eUnitState; }
    //--------------------------------------------------------------------------
    private bool m_bAlive = false;              public bool IsAlive { get { return m_bAlive; } }              // 유닛이 다른 객체와 상호 작용이 가능한 상태
    private bool m_bCrowdControl = false;       public bool IsCrowdControl { get { return m_bCrowdControl; } }// 스턴 넉백등에 의해 제어 불가일 경우 
    private bool m_bLoadStatic = false;         public bool IsLoadStatic { get { return m_bLoadStatic; } }    // Scene에서 로딩된 객체이며 메모리를 삭제하지 않는다.
    private uint m_hUnitInstanceID = 0;         public uint GetUnitInstanceID() { return m_hUnitInstanceID; } // 네트워크 게임의 경우 세션 ID를 사용한다.
    private uint m_hUnitTableID = 0;            public uint GetUnitTableID() { return m_hUnitTableID; }       // 전사 마법사등 클레스 식별자등 
    //--------------------------------------------------------------------------
    protected void ProtUnitStateLoadedDynamic(UnityAction delFinish)
    {
        SetMonoActive(false);
        m_bAlive = false;
        m_bLoadStatic = false;
        OnUnitStateLoaded(() => {
            m_eUnitState = EUnitState.Loaded;
            delFinish?.Invoke();
        });
    }

    protected void ProtUnitStateLoadedStatic(UnityAction delFinish)
    {
        SetMonoActive(false);
        m_bAlive = false;
        m_bLoadStatic = true;
        OnUnitStateLoaded(() =>
        {
            m_eUnitState = EUnitState.Loaded;
            delFinish?.Invoke();
        });
    }

    protected void ProtUnitStateSpawning(UnityAction delFinish)
    {
        SetMonoActive(true);
        m_eUnitState = EUnitState.Spawning;
        OnUnitStateSpawning(delFinish);
    }

    protected void ProtUnitStateAlive()      // 활성화 될때 마다 호출. 한 게임에서 여러번 호출 될 수 있다.
    {
        m_eUnitState = EUnitState.Alive;
        m_bAlive = true;
        OnUnitStateAlive();
    }
    protected void ProtUnitPhaseOut(UnityAction delFinish)
    {
        m_bAlive = false;
        m_eUnitState = EUnitState.PhaseOut;
        OnUnitStatePhaseOut(delFinish);
    }

    protected void ProtUnitPhaseIn(UnityAction delFinish)
    {
        m_bAlive = true;
        m_eUnitState = EUnitState.PhaseIn;
        OnUnitStatePhaseIn(delFinish);
    }

    protected void ProtUnitDeathStart(UnityAction delFinish)
    {
        m_bAlive = false;
        m_eUnitState = EUnitState.DeathStart;
        OnUnitStateDeathStart(delFinish);
    }

    protected void ProtUnitDead()
    {
        m_bAlive = false;
        m_eUnitState = EUnitState.Dead;       
        OnUnitStateDead();
    }

    protected void ProtUnitRemove()
    {
        m_eUnitState = EUnitState.Remove;
        SetMonoActive(false);
        OnUnitStateRemove();
    }

    //---------------------------------------------------------------------------
    protected void ProtUnitGameStart()  // 스테이지별로 한번만 호출될다 
    {
        OnUnitGameStart();
    }

    protected void ProtUnitGameEnd()
    {
        OnUnitGameEnd();
    }

    //---------------------------------------------------------------------------
    internal void InterUnitReset()
    {
        m_eUnitState = EUnitState.Loaded;
        m_bAlive = false;
        OnUnitReset();
    }

    internal void InterUnitUpdate(float fDelta)
    {
        OnUnitUpdate(fDelta);
    }

    //-----------------------------------------------------------------------------
    public void InitializeUnitInstance(uint hUnitInstanceID, uint hUnitTableID)
    {
        m_hUnitInstanceID = hUnitInstanceID;
        m_hUnitTableID = hUnitTableID;
        OnUnitInitialize();
    }

    //---------------------------------------------------------------------------
    protected virtual void OnUnitUpdate(float fDelta) { }
    protected virtual void OnUnitReset() { }
    protected virtual void OnUnitInitialize() { }
    //--------------------------------------------------------------------------
    protected virtual void OnUnitStateLoaded(UnityAction delFinish) { delFinish?.Invoke(); }
    protected virtual void OnUnitStateSpawning(UnityAction delFinish) { delFinish?.Invoke(); }
    protected virtual void OnUnitStatePhaseOut(UnityAction delFinish) { delFinish?.Invoke(); }
    protected virtual void OnUnitStatePhaseIn(UnityAction delFinish) { delFinish?.Invoke(); }
    protected virtual void OnUnitStateDeathStart(UnityAction delFinish) { delFinish?.Invoke(); }
  
    protected virtual void OnUnitStateAlive() { }
    protected virtual void OnUnitStateDead() { }
    protected virtual void OnUnitStateRemove() { }

    protected virtual void OnUnitGameStart() { }
    protected virtual void OnUnitGameEnd() { }
}
