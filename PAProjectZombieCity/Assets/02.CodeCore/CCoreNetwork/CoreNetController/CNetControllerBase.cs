using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 세션을 통해 전달된 내용을 중계하는 객체로 유닛을 Possession 할 수 있다.
// IFun엔진과 같은 외부 아키텍처를 중계
public abstract class CNetControllerBase : CObjectInstancePoolBase<CNetControllerBase>
{
    private uint m_hNetworkID = 0;         public uint GetNetworkID() { return m_hNetworkID; }
    //-------------------------------------------------------------------------


}
