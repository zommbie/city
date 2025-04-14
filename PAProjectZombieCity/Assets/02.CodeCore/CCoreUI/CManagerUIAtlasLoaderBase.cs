using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

//모든 아틀라스는 메모리에 상주한다. 규모와 갯수를 통재해야 한다.
//

public abstract class CManagerUIAtlasLoaderBase : CManagerTemplateBase<CManagerUIAtlasLoaderBase>
{
	private List<SpriteAtlas> m_listSpriteAtlasInstance = new List<SpriteAtlas>();
	private Dictionary<int, Dictionary<string, Sprite>> m_mapAtlasSprite = new Dictionary<int, Dictionary<string, Sprite>>();
	//--------------------------------------------------------
	protected void ProtAtlasLoaderAdd(int eCategory, SpriteAtlas pAtlas) // 모든 스프라이트는 Clone이며 메모리에 상주한다.
    {
		Dictionary<string, Sprite> mapAtlasSprite = FindOrAllocAtlasCategory(eCategory);
		PrivAtlasLoaderAdd(mapAtlasSprite, pAtlas);
		m_listSpriteAtlasInstance.Add(pAtlas);
	}

	public Sprite FindAtlasSprite(int eCategory, string strSpriteName) 
	{
		Sprite pFindSprite = null;
		Dictionary<string, Sprite> mapAtlasSprite = FindOrAllocAtlasCategory(eCategory);
		if (mapAtlasSprite.ContainsKey(strSpriteName))
		{
			pFindSprite = mapAtlasSprite[strSpriteName];
		}
        else
        {
            Debug.LogError($"[CManagerUIAtlasLoaderBase] Sprite Name : {strSpriteName} is not valid. Sprite is null.");
        }
		return pFindSprite;
	}

	//-------------------------------------------------------------
	private void PrivAtlasLoaderAdd(Dictionary<string, Sprite> mapAtlasSprite, SpriteAtlas pAtlas)
	{
		Sprite[] aSprite = new Sprite[pAtlas.spriteCount];
		pAtlas.GetSprites(aSprite);

		for (int i = 0; i < aSprite.Length; i++)
		{
			mapAtlasSprite[RemoveCloneObjectName(aSprite[i].name)] = aSprite[i];
		}
	}

	private Dictionary<string, Sprite> FindOrAllocAtlasCategory(int eCategory)
	{
		Dictionary<string, Sprite> pAtlasCategory = null;
		if (m_mapAtlasSprite.ContainsKey(eCategory))
		{
			pAtlasCategory = m_mapAtlasSprite[eCategory];
		}
		else
		{
			pAtlasCategory = new Dictionary<string, Sprite>();
			m_mapAtlasSprite[eCategory] = pAtlasCategory;	
		}

		return pAtlasCategory;
	}
} 
