using UnityEngine;

public class ZCSceneAttacherBattleStagePrototype : ZCSceneAttacherBase
{


    //------------------------------------------------------------
    protected override void OnUnityStart()
    {
        ProtSceneAttacherBasicLoad(() => {
            PrivSceneAttacherWorkFinish();
        });
    }

    //--------------------------------------------------------------
    private void PrivSceneAttacherWorkFinish()
    {



    }
}
