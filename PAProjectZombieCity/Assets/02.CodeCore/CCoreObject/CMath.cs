using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CMath 
{

    /// <summary>
    /// 평면과 두 백터 사이의 교점을 구하는 함수 . vecPlaneNormal는 보통 Trasform.up으로 구해보자 
    /// </summary>
    public static Vector3 CalculateContactPoint(Vector3 vecPlaneNormal, Vector3 vecPlaneDot, Vector3 vecStart, Vector3 vecEnd)
	{
		Vector3 vecLineNormal = (vecEnd - vecStart).normalized;
		return vecStart + vecLineNormal * Vector3.Dot(vecPlaneNormal, vecPlaneDot - vecStart) / Vector3.Dot(vecPlaneNormal, vecLineNormal);
	}

    /// <summary>
    /// 해당 디랙션 백터를 360도로 표현해준다.  VecAxis = Vector3.right를 많이 사용함.  
    /// </summary>
    public static float CalculateAngle360(Vector3 vecDirection, Vector3 vecAxis)
    {
        return Quaternion.FromToRotation(vecAxis, vecDirection).eulerAngles.z;
    }
  
   
    public static Vector3 CalculateRotateVectorDirection(Vector3 vecDirection, float fRotateAngle)
    {
        return Quaternion.AngleAxis(fRotateAngle, Vector3.up) * vecDirection;
    }

    public static Vector3 CalculateRotateVectorDirectionRandom(Vector3 vecDirection, float fAngleMin, float fAngleMax)
    {
        Vector3 vecResult = vecDirection;
        float fAngle = Random.Range(fAngleMin, fAngleMax);
        vecResult = CalculateRotateVectorDirection(vecDirection, fAngle);
        return vecResult;
    }

    public static float CalculateAngle180(Vector3 vStart, Vector3 vEnd)
    {
        Vector3 v = vEnd - vStart;
        return Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
    }
}
