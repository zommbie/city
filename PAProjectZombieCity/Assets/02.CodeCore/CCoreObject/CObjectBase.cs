using System.Collections.Generic;
using System.Reflection;
// CObjectBase 
// [개요] Mono가 아닌 일반 메모리 오브젝트를 표현한다. 
// 1) Struct가 아닌 객체는 GC에 부담을 주므로 별도로 관리할 필요가 있다.
// 2) 필요시 할당을 하고 사용한 객체는 반환을 받는 간단한 메모리 풀 구조이다. (메모리 단편화 때문에 삭제하지 않는다)
// 3) 패킷 관리용 객체나 리소스 핸들등 런타임 할당 객체에 적합하다. 
// 4) [주의] 상속객체는 반환 전에 모든 레퍼런스를 초기화 해야 한다.(OnPoolObjectDeactivate 에 기술할것) 죽은 메모리 풀 때문에 GC가 안될 수도 있다.

abstract public class CObjectBase : object
{
    private static uint g_ObjectInstanceID = 100;
    private uint m_hObjectInstanceID = 0;  protected virtual uint GetObjectInstanceID() { return m_hObjectInstanceID; }

    //----------------------------------------------------------------
    public TEMPLATE CopyInstance<TEMPLATE>() where TEMPLATE : CObjectBase
	{
        TEMPLATE pNewInstance = MemberwiseClone() as TEMPLATE;

        FieldInfo[] aFieldInfo = pNewInstance.GetType().GetFields();
        for (int i = 0; i < aFieldInfo.Length; i++)
        {
            FieldInfo pFieldInfo = aFieldInfo[i];
            CObjectBase pMemeber = pFieldInfo.GetValue(this) as CObjectBase;
            if (pMemeber != null)
            {
                pFieldInfo.SetValue(pMemeber, pMemeber.CopyInstance());
            }
        }

        return pNewInstance;
    }

    //------------------------------------------------------------------
    public CObjectBase CopyInstance()
    {
        CObjectBase pNewInstance = MemberwiseClone() as CObjectBase;
        return pNewInstance;
    }

    public CObjectBase()
    {
        m_hObjectInstanceID = g_ObjectInstanceID;
        g_ObjectInstanceID++;
    }
}

public abstract class CObjectInstancePoolBase<TEMPLATE> : CObjectBase where TEMPLATE : CObjectInstancePoolBase<TEMPLATE>
{
    private static List<TEMPLATE> g_PoolInstance = new List<TEMPLATE>();
    private bool   m_bInstanceActive = false;

    public static INSTANCE InstancePoolMake<INSTANCE>() where INSTANCE : TEMPLATE, new()
    {        
        INSTANCE pNewObject = SearchObjectInstance<INSTANCE>();        
        pNewObject.m_bInstanceActive = true;
        pNewObject.OnObjectPoolActivate();    
        return pNewObject;
    }

    public static void InstancePoolReturn(TEMPLATE pDeactiveInstance)
    {
        pDeactiveInstance.m_bInstanceActive = false;
        pDeactiveInstance.OnObjectPoolDeactivate();
    }

    //-------------------------------------------------------------------------------------------------------
    private static INSTANCE SearchObjectInstance<INSTANCE>() where INSTANCE : TEMPLATE, new()
    {
        INSTANCE pSearchInstance = null;

        for (int i = 0; i < g_PoolInstance.Count; i++)
        {
            INSTANCE pInstaqnce = g_PoolInstance[i] as INSTANCE;
            if (pInstaqnce != null)
            {
                if (pInstaqnce.m_bInstanceActive == false)
                {
                    pSearchInstance = g_PoolInstance[i] as INSTANCE;
                    break;
                }
            }
        }

        if (pSearchInstance == null) 
        {
            pSearchInstance = new INSTANCE();
            g_PoolInstance.Add(pSearchInstance);
        }

        return pSearchInstance; 
    }
    //---------------------------------------------------------------
    protected virtual void OnObjectPoolActivate() { }
    protected virtual void OnObjectPoolDeactivate() { }
}
