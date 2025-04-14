using Newtonsoft.Json;
namespace NWebPacket
{

	public interface IJsonPacketBase
    {
        public void ReturnInstance();
    }

    public interface IJsonPacketRequest : IJsonPacketBase
    {

    }

    public interface IJsonPacketResponse : IJsonPacketBase
    {

    }

    [System.Serializable]
    public abstract class CJsonPacketTemplateRequest<TEMPLATE> : CObjectInstancePoolBase<TEMPLATE>, IJsonPacketRequest  where TEMPLATE : CJsonPacketTemplateRequest<TEMPLATE>
    {       
        public void ReturnInstance()
        {
            InstancePoolReturn(this as TEMPLATE);
        }
    }

    [System.Serializable]
    public abstract class CJsonPacketTemplateResponse<TEMPLATE> : CObjectInstancePoolBase<TEMPLATE>, IJsonPacketResponse where TEMPLATE : CJsonPacketTemplateResponse<TEMPLATE>
    {
		//---------------------------------------------------
		public void ReturnInstance()
        {
            InstancePoolReturn(this as TEMPLATE);
        }
    }

}




