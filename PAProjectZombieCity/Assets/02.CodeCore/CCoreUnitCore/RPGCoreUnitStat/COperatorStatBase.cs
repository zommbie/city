using System.Collections;
using System.Collections.Generic;

public abstract class COperatorStatBase : COperatorBase
{
    private IEventStatHandler m_pEventStatHandler = null;
    private Dictionary<int, Dictionary<int, CStatBase>> m_mapStatInstance = new Dictionary<int, Dictionary<int, CStatBase>>();
    //-------------------------------------------------------------------
    public void InitializeStatOperator(IEventStatHandler pEventStatHandler)
    {
        m_pEventStatHandler = pEventStatHandler;
        OnStatOperatorInitialize(pEventStatHandler);
    }

    internal void InterStatOperatorStatChange(CStatBase pStat, float fValue)
    {
        m_pEventStatHandler?.IEventStatUpdate(GetOperatorUnit().GetUnitInstanceID(), pStat.GetStatType(), fValue);
        OnStatOperatorStatChange(pStat, fValue);
    }

    //--------------------------------------------------------------------
    protected void ProtStatOperatorAdd(int eCategory, CStatBase pStatInstance)
    {
        pStatInstance.InterStatInitilize(this);

        Dictionary<int, CStatBase> mapStatInstance = null;
        if (m_mapStatInstance.ContainsKey(eCategory))
        {
            mapStatInstance = m_mapStatInstance[eCategory];
        }
        else
        {
            mapStatInstance = new Dictionary<int, CStatBase>();
            m_mapStatInstance[eCategory] = mapStatInstance;
        }

        mapStatInstance[pStatInstance.GetStatType()] = pStatInstance;
    }

    protected CStatBase FindStat(int eCategory, int eStatID)
    {
        CStatBase pFindStat = null;
        if (m_mapStatInstance.ContainsKey(eCategory))
        {
            Dictionary<int, CStatBase> mapStatInstance = m_mapStatInstance[eCategory];
            if (mapStatInstance.ContainsKey(eStatID))
            {
                pFindStat = mapStatInstance[eStatID];
            }
        }

        return pFindStat;
    }

    //---------------------------------------------------------------------
    protected virtual void OnStatOperatorInitialize(IEventStatHandler pEventStatHandler) {  }
    protected virtual void OnStatOperatorStatChange(CStatBase pStat, float fValue) { }

}
