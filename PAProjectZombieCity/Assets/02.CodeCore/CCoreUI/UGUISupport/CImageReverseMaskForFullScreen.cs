using System.Collections;
using UnityEngine;

using UnityEngine.Rendering;
public class CImageReverseMaskForFullScreen : CImage
{
    private Material m_pMaterialCopy = null;
    //--------------------------------------------------------------------
    protected override void Start()
    {
        base.Start();
        StartCoroutine(Fix());
    }

    private IEnumerator Fix()
    {
        yield return null;
        maskable = false;
        maskable = true;
    }

    public override Material materialForRendering { get 
        { 
            Material pMaterial = new Material(base.materialForRendering);
            pMaterial.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
            return pMaterial;
        } 
    }


}
