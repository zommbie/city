using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

// 네비 메시 기반으로 이동하는 객체 

public abstract class CAssistMovementNavMeshBaseOld : CAssistMovementBaseOld
{
    [SerializeField]
    private bool RotationLock = true;
    [SerializeField]
    private bool EnableAccelation = false;
    [SerializeField]
    private NavMeshAgent MoveAgent = null;      protected NavMeshAgent GetAssistMovementMeshAgent() { return MoveAgent; }
    [SerializeField]
    private float StuckAdjustAngle = 30f;    // 방향 이동시 수직면에 멈출경우 임의의 각도만큼 방향을 변경한다.

    [SerializeField]
    private int RecursiveCount = 10;        // // 지나치게 작은 수치로 계산을 반복하는 것을 막기 위한 안전 카운터. 밀집도에 따른 계산 부하를 유의할 것

    private Vector3 m_vecUnitDirection = Vector3.zero;
    private Vector3 m_vecMoveOrigin = Vector3.zero;
    private UnityAction<int> m_delFinish = null;
    //---------------------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitActorBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        if (RotationLock)
        {
            MoveAgent.updateRotation = false;
        }        
    }

    //protected override void OnAssistUnitState(CUnitBaseOld.EUnitState eState)
    //{
    //    base.OnAssistUnitState(eState);
    //    if (eState == CUnitBaseOld.EUnitState.Spawning)
    //    {
    //        this.enabled = true;
    //    }
    //    else if (eState == CUnitBaseOld.EUnitState.DeathStart)
    //    {
    //        enabled = false;
    //    }
    //}

    protected override void OnAssistMovementUpdate(float fDelta, float fMoveScale, EMoveType eMoveStatus)
    {
        base.OnAssistMovementUpdate(fDelta, fMoveScale, eMoveStatus);
        if (eMoveStatus == EMoveType.ToPosition)
        {
            UpdateAssistMoveDestinationFinish();
        }
    }

    //----------------------------------------------------------------------------------
    public override Vector3 GetAssistMovementPosition() 
    {
        return MoveAgent.transform.position;
    }

    public override Quaternion GetAssistMovementRotation()
    {      
        return MoveAgent.transform.rotation;
    }

    public override Vector3 GetAssistMovementDirection()
    {
        return m_vecUnitDirection;
    }

    public override void SetAssistMovementDirection(Vector3 vecDirection) 
    {
        m_vecUnitDirection = vecDirection;
    }

    //--------------------------------------------------------------------------------
    protected void ProtAssistMovementForceMove(Vector3 vecMoveDirection, Vector3 vecUnitDirection, float fMoveLength) // 에이전트를 방향으로 밀었을때. 주로 케릭터를 컨트롤러 방향으로 이동 시킬때
    {
        if (MoveAgent.isOnNavMesh == false)
        {
            return;
        }

        MoveAgent.velocity = Vector3.zero;
        MoveAgent.speed = 0;
        MoveAgent.isStopped = true;
        
        vecMoveDirection.Normalize();
        m_vecUnitDirection = vecUnitDirection;
        m_vecMoveOrigin = MoveAgent.transform.position;

        float fMoveValue = fMoveLength * GetAssistMoveScale();
        RecursiveAssistMovementDirection(vecMoveDirection, fMoveValue, RecursiveCount);
    }

    protected void ProtAssistMovementPosition(Vector3 vecPosition, float fMoveSpeedPerSecond,  float fStopOffset, UnityAction<int> delFinish) // 원거리 유닛등은 사거리만큼 offset을 지정한다.
    {
        if (MoveAgent.isOnNavMesh == false)
        {
            return;
        }


        MoveAgent.velocity = Vector3.zero;
        MoveAgent.isStopped = false;
        MoveAgent.stoppingDistance = fStopOffset;
        MoveAgent.speed = fMoveSpeedPerSecond * GetAssistMoveScale();       

        Vector3 vecDirection = vecPosition - MoveAgent.transform.position;
        vecDirection.Normalize();
        m_vecUnitDirection = vecDirection;
        m_delFinish = delFinish;

        m_vecMoveOrigin = MoveAgent.transform.position;
        if(MoveAgent.SetDestination(vecPosition) == false)
        {
            m_delFinish?.Invoke(-1);
        }
    }

    protected bool ProtAssistMovementApproach(Vector3 vecApproachPosition, float fMoveLength)
    {
        if (MoveAgent.isOnNavMesh == false)
        {
            return false;
        }


        bool bArrive = false;
        fMoveLength *= GetAssistMoveScale();
        Vector3 vecPosition = MoveAgent.transform.position;
        Vector3 vecDirection = vecApproachPosition - vecPosition;
        vecDirection.Normalize();

        m_vecUnitDirection = vecDirection;

        float fDistance = Vector3.Distance(vecPosition, vecApproachPosition);

        if (fDistance <= fMoveLength)
        {
            MoveAgent.Warp(vecApproachPosition);
            bArrive = true;
        }
        else
        {
            MoveAgent.Move(vecDirection * fMoveLength);
            bArrive = false;
        }
       
        return bArrive;
    }

    protected void ProtAssistMovementWarp(Vector3 vecPosition)
    {
        if (MoveAgent.isOnNavMesh == false)
        {
            return;
        }


        if (MoveAgent.Warp(vecPosition))
        {
            
        }
        else
        {

        }
        
    }

    protected void ProtAssistMovementVelocity(Vector3 vecDirectionLength)
    {
        MoveAgent.velocity = vecDirectionLength;
    }

    protected void ProtAssistMovementEnable(bool bEnable)
    {
        MoveAgent.isStopped = !bEnable;
    }

    //--------------------------------------------------------------------------
    private float RecursiveAssistMovementDirection(Vector3 vecDirection, float fLength, int iRecursiveCount)
    {
        float fMovedLength = 0;
        if (iRecursiveCount < 0)  
        {
            Debug.LogWarningFormat("[Movement] RecursiveCount Low !!!");
            return 0f;
        }

        iRecursiveCount--;

        Vector3 vecDirectionLength = vecDirection * fLength;
        Vector3 vecPositionPrev = transform.position;
        MoveAgent.Move(vecDirectionLength);
        Vector3 vecPositionAfter = transform.position;

        float fAfterLength = Vector3.Distance(vecPositionAfter, vecPositionPrev);
        float fRemainLength = fLength - fAfterLength;
        if (fRemainLength <= c_MoveThreshold)
        {
            fMovedLength = fRemainLength;
            return fMovedLength;
        }

        if (fAfterLength >= c_MoveThreshold && fAfterLength < fLength - c_MoveThreshold)
        {
             fMovedLength = RecursiveAssistMovementDirection(vecDirection, fRemainLength, iRecursiveCount);
        }
        else if (fAfterLength > 0)
        {
            bool bLeft = Random.Range(0, 1) == 1 ? true : false; // 임시조치 ToDo : NavMeshHit 이용하여 방향을 산출하여 밀어줄것
            float fAdjustAngle = 0f;
            if (bLeft)
            {
                fAdjustAngle = StuckAdjustAngle * iRecursiveCount;
            }
            else
            {
                fAdjustAngle = -StuckAdjustAngle * iRecursiveCount;
            }
            Vector3 vecNewDirection = CMath.CalculateRotateVectorDirection(vecDirection, fAdjustAngle);
            fMovedLength = RecursiveAssistMovementDirection(vecNewDirection, fRemainLength, iRecursiveCount);
        }
        return fMovedLength;
    }

    private void UpdateAssistMoveDestinationFinish()
    {
        if (MoveAgent.remainingDistance == 0f && MoveAgent.pathPending == false)
        {
            PrivAssistMoveDestinationFinish();
        }       
    }   

    private void PrivAssistMoveDestinationFinish()
    {
       
    }

    //----------------------------------------------------------------
}
