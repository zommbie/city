using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 스킬 사용자 전달해주는 정보로 스킬 발동에 필요한 다양한 정보를 담고 있다.

public class SSkillUsage
{
    //-------------------------------------------------------------------------   
    public uint             SkillID = 0;
    public int              SkillLevel = 0;
    public Vector3          OriginPosition = Vector3.zero;                          // 시전자 위치
    public Vector3          OriginDirection = Vector3.zero;                         // 시전자 방향
    public Vector3          TargetPosition;                                         // 목표 위치 
    public IUnitEventSkill  SkillOwner = null;                                      // 사용자 
    public List<IUnitEventSkill>  SkillTargetList = new List<IUnitEventSkill>();    // 대상 (없을 수 있음)
}

public struct SSkillProperty
{
    public uint PropertyID;
    public float ProperyRate;
}