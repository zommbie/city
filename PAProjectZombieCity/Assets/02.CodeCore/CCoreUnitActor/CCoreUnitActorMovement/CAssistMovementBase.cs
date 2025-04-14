using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventMoveHandler
{
    public void IEventMoveArriveDest();
}

public class CAssistMovementBase : CAssistUnitActorBase, IEventMoveHandler
{    
    private float m_fMoveScale = 1f;   // 게임 속도와 같이 외부에서 지정한 값
    private float m_fMoveSpeed = 1f; public float GetAssistMoveSpeed() { return m_fMoveSpeed; }   // 해당 객체의 이동 속도 

    private UnityAction<Vector3> m_delMoveFinish = null;
    private CUnitActorBase    m_pOwnerActor = null;
    private IEventMoveHandler m_pEventMoveHandler = null;
    private CMovementBase     m_pCurrentMovement = null;        public bool IsMoving { get { return m_pCurrentMovement == null ? false : true; } }
    private Dictionary<int, CMovementBase> m_mapMovementInstance = new Dictionary<int, CMovementBase>();
    //-------------------------------------------------------------------------------------------
    protected override void OnAssistInitialize(CUnitActorBase pOwner)
    {
        base.OnAssistInitialize(pOwner);
        m_pOwnerActor = pOwner;
    }

    protected override void OnAssistFixedUpdate(float fDelta)
    {
        base.OnAssistFixedUpdate(fDelta);
        UpdateAssistMovement(fDelta);    
    }
    //-------------------------------------------------------------------------------------------
   
    public void SetAssistMovementEventHandler(IEventMoveHandler pEventMoveHandler)
    {
        m_pEventMoveHandler = pEventMoveHandler;
    }

    public void SetAssistMovementScale(float fScale)
    {
        m_fMoveScale = fScale;
    }


    public void IEventMoveArriveDest()
    {
        PrivAssistMoveStop();
    }

    //--------------------------------------------------------------------------------------------
    protected void ProtAssistMoveAdd(int eMoveInstance, CMovementBase pMovementInstance)
    {     
        pMovementInstance.InterMovementInitialize(this, m_pOwnerActor.transform);
        m_mapMovementInstance[eMoveInstance] = pMovementInstance;
    }

    protected void ProtAssistMoveStartToPosition(int eMoveInstance, Vector3 vecDestPosition, UnityAction<Vector3> delFinish)
    {
        PrivAssistMovementReset();
        CMovementBase pMovementInstance = FindAssistMovementInstance(eMoveInstance);
        if (pMovementInstance != null)
        {
            m_delMoveFinish = delFinish;
            m_pCurrentMovement = pMovementInstance;
            m_pCurrentMovement.InterMovementStartToPosition(vecDestPosition);
           
        }
    }

    protected void ProtAssistMoveStartToDirection(int eMoveInstance, Vector3 vecDestDirection, UnityAction<Vector3> delFinish)
    {
        PrivAssistMovementReset();
        CMovementBase pMovementInstance = FindAssistMovementInstance(eMoveInstance);
        if (pMovementInstance != null)
        {
            m_delMoveFinish = delFinish;
            m_pCurrentMovement = pMovementInstance;
            m_pCurrentMovement.InterMovementStartToDirection(vecDestDirection);
        }
    }

    protected void ProtAssistMoveStartToObject(int eMoveInstance, CUnitActorBase pTargetActor, UnityAction<Vector3> delFinish)
    {
        PrivAssistMovementReset();
        CMovementBase pMovementInstance = FindAssistMovementInstance(eMoveInstance);
        if (pMovementInstance != null)
        {
            m_delMoveFinish = delFinish;
            m_pCurrentMovement = pMovementInstance;
            m_pCurrentMovement.InterMovementStartToObject(pTargetActor.transform);
        }
    }

    //-----------------------------------------------------------------------------------------
    private void UpdateAssistMovement(float fDelta)
    {
        if (m_pCurrentMovement == null) return;

        float fMoveDelta = m_fMoveScale * m_fMoveSpeed * fDelta;
        m_pCurrentMovement.InterMovementUpdate(fDelta, fMoveDelta);

    }

    private CMovementBase FindAssistMovementInstance(int eMoveInstance)
    {
        CMovementBase pFindMovement = null;
        m_mapMovementInstance.TryGetValue(eMoveInstance, out pFindMovement);
        return pFindMovement;
    }

    private void PrivAssistMovementReset()
    {
        m_pCurrentMovement = null;
    }

    private void PrivAssistMoveStop()
    {
        PrivAssistMovementReset();
        m_delMoveFinish?.Invoke(m_pOwnerActor.transform.position);
      
    }
    //-------------------------------------------------------------------------------------------
    protected virtual void OnAssistMovementStart() { }
    protected virtual void OnAssistMovementStop() { }
}
