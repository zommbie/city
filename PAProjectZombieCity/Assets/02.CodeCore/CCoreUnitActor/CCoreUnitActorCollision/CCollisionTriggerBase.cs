using UnityEngine;

// 트리거 오브젝트를 랩핑하여 기능적으로 분화 한다.
// 릿지드 바디가 없는 일반 총알같은 충돌체에 사용된다.

public interface IEventCollisionTrigger
{
    public void IEventCollisionTriggerEnter(CCollisionTriggerBase pEnterTrigger);
    public void IEventCollisionTriggerExit(CCollisionTriggerBase pExitTrigger);
    public void IEventCollisionTriggerRefresh(CCollisionTriggerBase pRefreshTrigger);
}

[RequireComponent(typeof(CapsuleCollider))]
public abstract class CCollisionTriggerBase : CMonoBase
{
    [SerializeField]
    private IEventCollisionTrigger m_pEventReceiver = null; public IEventCollisionTrigger GetCollisionTrigger() { return m_pEventReceiver; }

    private CapsuleCollider m_pCapsuleCollider = null;
    //---------------------------------------------------------------
    internal void InterCollisionTriggerInitialize(IEventCollisionTrigger pEventReceiver)
    {
        m_pEventReceiver = pEventReceiver;
        m_pCapsuleCollider = GetComponent<CapsuleCollider>();
        m_pCapsuleCollider.isTrigger = true;
        OnCollisionTriggerInitialize();
    }
    //--------------------------------------------------------------
    public TEMPLATE GetCollisionTriggerOwner<TEMPLATE>() where TEMPLATE : class, IEventCollisionTrigger 
    {
        return m_pEventReceiver as TEMPLATE;
    }

    //---------------------------------------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        CCollisionTriggerBase pClashCollisionTrigger = collision.gameObject.GetComponent<CCollisionTriggerBase>();
        if (pClashCollisionTrigger != null)
        {
            m_pEventReceiver?.IEventCollisionTriggerEnter(pClashCollisionTrigger);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        CCollisionTriggerBase pClashCollisionTrigger = collision.gameObject.GetComponent<CCollisionTriggerBase>();
        if (pClashCollisionTrigger != null)
        {
            m_pEventReceiver?.IEventCollisionTriggerExit(pClashCollisionTrigger);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        CCollisionTriggerBase pClashCollisionTrigger = collision.gameObject.GetComponent<CCollisionTriggerBase>();
        if (pClashCollisionTrigger != null)
        {
            m_pEventReceiver?.IEventCollisionTriggerRefresh(pClashCollisionTrigger);
        }
    }
    //------------------------------------------------------------------
    protected virtual void OnCollisionTriggerInitialize() { }

}
