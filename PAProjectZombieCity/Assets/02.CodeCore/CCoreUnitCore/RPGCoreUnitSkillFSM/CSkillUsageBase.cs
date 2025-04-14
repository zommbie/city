using System.Collections;
using System.Collections.Generic;

// 액션 RPG등에서는 스킬 사용시 타겟이 특정되므로 해당 내용으로 확장 가능 
// 스킬을 구동 시키기 위한 값
public abstract class CSkillUsageBase 
{
    public uint SkillID;
    public int  SkillLevel;
    public int  SkillType;

    public int   ValueInt;
    public float ValueFloat;
}


public abstract class CSkillContainerBase
{
    public uint SkillID;
    public int  SkillType;
    public int  SkillLevel;

    public CSkillConditionList SkillCondition = new CSkillConditionList();
    public List<CStateSkillBase> SkillState = new List<CStateSkillBase>();
}

public abstract class CSkillTaskEventBase
{
    
}