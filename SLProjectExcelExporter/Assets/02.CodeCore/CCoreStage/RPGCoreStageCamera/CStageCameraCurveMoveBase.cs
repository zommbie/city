using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class CStageCameraCurveMoveBase : CStageCameraBase
{
    [SerializeField]
    private AnimationCurve MoveCurve = null;

    private float m_fMoveTime = 1f;
    private float m_fCurrentMoveTime = 1f;
    private Vector3 m_vecStartPosition = Vector3.zero;
    private Vector3 m_vecDestPosition = Vector3.zero;
    private Vector3 m_vecDirectionLength = Vector3.zero;
    private UnityAction m_delFinish = null;
    private bool m_bUpdateMove = false;
	//------------------------------------------------------------
	protected override void OnUnityUpdate(float fDeltaTime)
	{


	}
	//--------------------------------------------------------------
    private void UpdateCameraPosition()
    {

    }

	//-----------------------------------------------------------
	protected void ProtStageCameraMovePositionCurve(float fMoveTime, Vector3 vecDestPosition, UnityAction delFinish)
    {
        if (m_vecDestPosition == vecDestPosition)
        {
            delFinish?.Invoke();
        }

        m_delFinish = delFinish;
        m_fMoveTime = fMoveTime;
        m_fCurrentMoveTime = 0;
        m_vecStartPosition = transform.position;
        m_vecDestPosition = vecDestPosition;
        m_vecDirectionLength = m_vecDestPosition - m_vecStartPosition;
        m_bUpdateMove = true;

        OnStageCameraMoveStart(vecDestPosition);

        if (fMoveTime == 0)
        {
			PrivStageCameraArriveDestination();
        }
	}

    protected void ProtStageCameraMoveDirection(Vector3 vecDirectionLength)
    {
        transform.position += vecDirectionLength;
        OnStageCameraMoveDirection(vecDirectionLength);
    }

	//----------------------------------------------------------------
	private void PrivStageCameraArriveDestination()
    {
        m_bUpdateMove = false;
        transform.position = m_vecDestPosition;
		m_delFinish?.Invoke();
		OnStageCameraMoveFinish(m_vecDestPosition);
	}

    //----------------------------------------------------------------
    protected virtual void OnStageCameraMoveStart(Vector3 vecDest) { }
    protected virtual void OnStageCameraMoveFinish(Vector3 vecDest) { }
    protected virtual void OnStageCameraMoveDirection(Vector3 vecDirectionLength) { }
}
