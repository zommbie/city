using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어가 조작할 수 있다. 조작 내용은 즉시 반영되며 원격으로 다른 클라에게 전달된다. 
// 자신의 조작 내용이 돌아오면 현재 상태를 비교해서 보간을 실시한다.  (플레이 롤백이 발생 할수 있다)
// 데디케이트 서버에 접속한 클라 포지션 
public abstract class CNetControllerAutonomousClientBase : CNetControllerBase
{
    
}
