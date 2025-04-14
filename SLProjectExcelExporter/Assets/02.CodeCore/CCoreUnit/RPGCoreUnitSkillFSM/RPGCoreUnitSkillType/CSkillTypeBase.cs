using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 스킬의  실행단위를 결정 
// 다수의 State로 구성되어 있으며 순차적으로 작동 

public abstract class CSkillTypeBase 
{
    private uint m_hSkillID = 0; public uint GetSkillID() { return m_hSkillID; } public void SetSkillID(uint hSkillID) { m_hSkillID = hSkillID; }
  
    //-------------------------------------------------
    
}
