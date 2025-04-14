using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CUnitAssistBase : CUnitAIBase
{
    private Dictionary<string, CAssistUnitBase> m_mapAssistInstance = new Dictionary<string, CAssistUnitBase>();
    //------------------------------------------------------------------------------
    protected override void OnUnitGameStart()
    {
        base.OnUnitGameStart();
        PrivUnitAssistGameStartEnd(true);
    }

    protected override void OnUnitGameEnd()
    {
        base.OnUnitGameEnd();
        PrivUnitAssistGameStartEnd(false);
    }

    protected override void OnUnitStateLoaded(UnityAction delFinish)
    {        
        base.OnUnitStateLoaded(() => {
            PrivUnitAssistUnitState(EUnitState.Loaded);
            delFinish?.Invoke();
        });
    }

    protected override void OnUnitStateSpawning(UnityAction delFinish)
    {
        base.OnUnitStateSpawning(() => {
            PrivUnitAssistUnitState(EUnitState.Spawning);
            delFinish?.Invoke();
        });
    }

    protected override void OnUnitStatePhaseOut(UnityAction delFinish)
    {
        base.OnUnitStatePhaseOut(() => {
            PrivUnitAssistUnitState(EUnitState.PhaseOut);
            delFinish?.Invoke();
        });
    }

    protected override void OnUnitStatePhaseIn(UnityAction delFinish)
    {        
        base.OnUnitStatePhaseIn(() => {
            PrivUnitAssistUnitState(EUnitState.PhaseIn);
            delFinish?.Invoke();
        });
    }

    protected override void OnUnitStateDeathStart(UnityAction delFinish)
    {
        base.OnUnitStateDeathStart(delFinish);
        PrivUnitAssistUnitState(EUnitState.DeathStart);
    }

    protected override void OnUnitStateDead()
    {
        PrivUnitAssistUnitState(EUnitState.Dead);
        base.OnUnitStateDead();
    }

    protected override void OnUnitStateAlive()
    {
        PrivUnitAssistUnitState(EUnitState.Alive);
        base.OnUnitStateAlive();
    }

    protected override void OnUnitStateRemove()
    {
        PrivUnitAssistUnitState(EUnitState.Remove);
        base.OnUnitStateRemove();
    }
    //-------------------------------------------------
    private void PrivUnitAssistUnitState(EUnitState eState)
    {
        Dictionary<string, CAssistUnitBase>.Enumerator it = m_mapAssistInstance.GetEnumerator();
        while (it.MoveNext())
        {
            it.Current.Value.InterAssistUnitState(eState);
        }
    }

    private void PrivUnitAssistGameStartEnd(bool bStart)
    {
        Dictionary<string, CAssistUnitBase>.Enumerator it = m_mapAssistInstance.GetEnumerator();
        while (it.MoveNext())
        {
            if (bStart)
            {
                it.Current.Value.InterAssistUnitGameStart();
            }
            else
            {
                it.Current.Value.InterAssistUnitGameEnd();
            }
        }
    }


    //----------------------------------------------------
    protected virtual void ProtUnitAssistAdd(CAssistUnitBase pAssistInstance)
    {
        string strName = pAssistInstance.GetType().Name;
        if (m_mapAssistInstance.ContainsKey(strName))
        {
            //Error!
        }
        else
        {
            m_mapAssistInstance.Add(strName, pAssistInstance);
            pAssistInstance.InterAssistInitialize(this);
        }
    }
}
