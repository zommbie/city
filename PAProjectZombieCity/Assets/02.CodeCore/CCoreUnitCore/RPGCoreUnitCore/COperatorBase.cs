using System.Collections;
using System.Collections.Generic;

public abstract class COperatorBase 
{
    private CUnitCoreBase m_pUnitCoreOwner = null;   public CUnitCoreBase GetOperatorUnit() { return m_pUnitCoreOwner; }
    //---------------------------------------------------------------------
    internal void InterOperatorInitialize(CUnitCoreBase pUnitCore)
    {
        m_pUnitCoreOwner = pUnitCore;
        OnOperatorInitialize(pUnitCore);
    }

    internal void InterOperatorStateChange(int eState, int iValue, float fValue)
    {
        OnOperatorStateChange(eState, iValue, fValue);
    }

    internal void InterOperatorUpdate(float fDelta)
    {
        OnOperatorUpdate(fDelta);
    }

    internal void InterOperatorUpdatelate()
    {
        OnOperatorUpdatelate();
    }

    internal void InterOperatorReset()
    {
        OnOperatorReset();
    }
    //----------------------------------------------------------------------
    protected virtual void OnOperatorInitialize(CUnitCoreBase pUnitCore) { }
    protected virtual void OnOperatorStateChange(int eState, int iValue, float fValue) { }
    protected virtual void OnOperatorUpdate(float fDelta) { }
    protected virtual void OnOperatorUpdatelate() { }
    protected virtual void OnOperatorReset() { }
}
