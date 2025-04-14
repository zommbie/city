using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class CStageCamera2DTopDownBase : CStageCameraCurveMoveBase
{
    [SerializeField]
    private float OffsetY = 0f;
    [SerializeField]
    private float OffsetZ = 0f;

    private Transform m_pFollowObject = null;
    private bool m_bFollowEnable = false;
	//----------------------------------------------------------------------

    protected override void OnUnityFixedUpdate(float fFixedDeltaTime)
    {
        base.OnUnityFixedUpdate(fFixedDeltaTime);
        if (m_pFollowObject && m_bFollowEnable)
        {
            PrivStageCameraUpdateFollow();
        }
    }

    //-----------------------------------------------------------------------

    public void DoStageCameraTopDownFollowObject(Transform pFollowTransform)
	{
        m_bFollowEnable = true;
        m_pFollowObject = pFollowTransform;
	}

    public void SetStageCameraFollowEnable(bool bEnable)
    {
        m_bFollowEnable = bEnable;
    }

    //-----------------------------------------------------------------------
    private void PrivStageCameraUpdateFollow()
    {
        Vector3 vecCamera = transform.position;
        Vector3 vecFollow = m_pFollowObject.position;
        vecCamera.x = vecFollow.x;
        vecCamera.z = vecFollow.z + OffsetZ;
        vecCamera.y = vecFollow.y + OffsetY;

        transform.position = vecCamera;
    }
}
 