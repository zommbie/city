using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 언리얼 엔진의 네트워크롤을 기본 모델로 하였으나 언리얼이 각 객체에 대한 롤을 정하고 객체 내부에서
// 처리하는 로직에 비해 유니티 버전에서는 메니저를 통해 리플리케이션되어 각 객체를 통제하는 것으로 변경하였다. 
// 이를 통해 각 객체의 이벤트 생성과 전달이 중앙집중식으로 관리 될 수 있다. 

public abstract class CManagerNetControllerBase : CManagerTemplateBase<CManagerNetControllerBase>
{
    public enum ENetworkRole
    {
        Authority,               // 유닛을 소유하고 있고 클라이언트를 통제한다.
        AutonomousProxy,         // 입력용 객체를 가지고 있으며 나머지는 모두 Remote객체로 Authority의 통제를 받는다.
    }

    private ENetworkRole m_eNetworkRole = ENetworkRole.Authority;
    private Dictionary<uint, CNetControllerBase> m_mapSessionCtrIntance = new Dictionary<uint, CNetControllerBase>();
    //------------------------------------------------------------------------



    //-----------------------------------------------------------------------
    protected void ProtMgrNetControllerAdd(CNetControllerBase pNetController)
    {

    }

}
