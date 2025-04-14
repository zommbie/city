using System.Collections.Generic;
using UnityEngine;


abstract public class CManagerUnitBase : CManagerTemplateBase<CManagerUnitBase>
{
    private class SPhaseOutInfo
    {
        public CUnitBase PhaseUnit = null;
        public float     PhaseOutDuration = 0f;
        public float     PhaseRemain;
    }
   
    private List<SPhaseOutInfo> m_listPhaseOutRemoveNote = new List<SPhaseOutInfo>();

    private Dictionary<uint, CUnitBase>     m_mapUnitInstance = new Dictionary<uint, CUnitBase>();
    private Dictionary<uint, SPhaseOutInfo> m_mapUnitPhaseOut = new Dictionary<uint, SPhaseOutInfo>();
   
    //---------------------------------------------------------------
    protected override void OnUnityUpdate()
    {
        base.OnUnityUpdate();
        UpdateMgrUnitPhaseOut(Time.deltaTime);
    }

    protected override void OnManagerSceneReset(string strSceneName)
    {
        PrivMgrUnitClear();

    }
    //--------------------------------------------------------------
    public float GetMgrUnitPhaseOutRemainTime(uint hUnitInstanceID)
    {
        float fRemainTime = 0;
        if (m_mapUnitPhaseOut.ContainsKey(hUnitInstanceID))
        {
            fRemainTime = m_mapUnitPhaseOut[hUnitInstanceID].PhaseRemain;
        }
        return fRemainTime;
    }

    //---------------------------------------------------------------
    protected void ProtMgrUnitRegist(CUnitBase pRegistUnit)
    {
        uint hInstanceID = pRegistUnit.GetUnitInstanceID();
        if (m_mapUnitInstance.ContainsKey(hInstanceID))
        {
            //Error!
        }
        else 
        {
            m_mapUnitInstance[hInstanceID] = pRegistUnit;
            OnMgrUnitRegist(pRegistUnit);
        }
    }
    
    protected void ProtMgrUnitUnRegist(CUnitBase pUnRegistUnit)
    {
        uint hInstanceID = pUnRegistUnit.GetUnitInstanceID();
        if (m_mapUnitInstance.ContainsKey(hInstanceID))
        {
            m_mapUnitInstance.Remove(hInstanceID);
        }
        else
        {
            //Error!
        }

        if (m_mapUnitPhaseOut.ContainsKey(hInstanceID))
        {
            m_mapUnitPhaseOut.Remove(hInstanceID);
        }
    }

    protected void ProtMgrUnitPhaseOut(CUnitBase pPhaseOutUnit, float fDuration)
    {
        uint hInstanceID = pPhaseOutUnit.GetUnitInstanceID();
        if (m_mapUnitPhaseOut.ContainsKey(hInstanceID))
        {
            //Error!
        }
        else
        {
            SPhaseOutInfo pPhaseOutInfo = new SPhaseOutInfo();
            pPhaseOutInfo.PhaseOutDuration = fDuration;
            pPhaseOutInfo.PhaseRemain = fDuration;
            pPhaseOutInfo.PhaseUnit = pPhaseOutUnit;

            m_mapUnitPhaseOut[hInstanceID] = pPhaseOutInfo;
        }
    }    

    
    
    //--------------------------------------------------------------
    protected CUnitBase FindMgrUnitInstance(uint hInstanceID)
    {
        CUnitBase pUnit = null;
        m_mapUnitInstance.TryGetValue(hInstanceID, out pUnit);
        return pUnit;
    }

    protected Dictionary<uint, CUnitBase>.Enumerator IterMgrUnitList()
    {    
        return m_mapUnitInstance.GetEnumerator();
    }
    //-------------------------------------------------------------
    private void UpdateMgrUnitPhaseOut(float fDelta)
    {
        if (m_mapUnitPhaseOut.Count == 0) return;

        m_listPhaseOutRemoveNote.Clear();
        Dictionary<uint, SPhaseOutInfo>.Enumerator it = m_mapUnitPhaseOut.GetEnumerator();
        while(it.MoveNext())
        {
            SPhaseOutInfo pPhaseOutInfo = it.Current.Value;
            pPhaseOutInfo.PhaseRemain -= fDelta;
            if (pPhaseOutInfo.PhaseRemain <= 0)
            {
                m_listPhaseOutRemoveNote.Add(pPhaseOutInfo);
            }
        }

        for (int i = 0; i < m_listPhaseOutRemoveNote.Count; i++)
        {
            SPhaseOutInfo pPhaseOutInfo = m_listPhaseOutRemoveNote[i];
            m_mapUnitPhaseOut.Remove(pPhaseOutInfo.PhaseUnit.GetUnitInstanceID());
            OnMgrUnitPhaseIn(pPhaseOutInfo.PhaseUnit);
        }
    }

    private void PrivMgrUnitClear()
    {
        m_listPhaseOutRemoveNote.Clear();       
        m_mapUnitInstance.Clear();
        m_mapUnitPhaseOut.Clear();
    }

    //-------------------------------------------------------------
    protected virtual void OnMgrUnitUnRegist(CUnitBase pUnRegistUnit) { }
    protected virtual void OnMgrUnitRegist(CUnitBase pRegistUnit) { }   
    protected virtual void OnMgrUnitPhaseIn(CUnitBase pPhaseInUnit) { }
}
