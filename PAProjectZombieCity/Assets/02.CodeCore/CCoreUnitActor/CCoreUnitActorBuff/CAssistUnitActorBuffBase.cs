using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CAssistUnitActorBuffBase : CAssistUnitActorBase 
{
    protected struct SActorBuffInfo
    {
        public uint  BuffID;
        public int   BuffType;
        public float RemainTime;
        public int   StackCount;
        public float BuffPower;
    }

    private Dictionary<uint, SActorBuffInfo> m_mapBuffInfo = new Dictionary<uint, SActorBuffInfo>();
    //-------------------------------------------------------------------------------------------------



    //-----------------------------------------------------------------------------------------------
    protected void ProtUnitActorBuffAdd(uint hBuffID, int eBuffType, float fRemainTime, int iStackCount, float fBuffPower)
    {
        SActorBuffInfo rBuffInfo = new SActorBuffInfo();
        rBuffInfo.BuffID = hBuffID;
        rBuffInfo.BuffType = eBuffType;
        rBuffInfo.RemainTime = fRemainTime;
        rBuffInfo.StackCount = iStackCount;
        rBuffInfo.BuffPower = fBuffPower;
    }

    protected void ProtUnitActorBuffRemove()
    {

    }

    protected void ProtUnitActorBuffRemainTime(float fRemainTime)
    {

    }

}
