using UnityEngine;
using UnityEngine.U2D.Animation;

public class SkinSwap : MonoBehaviour
{
    [SerializeField] private SpriteLibrary spriteLibrary;

    public void SetSkin(SpriteLibraryAsset newSkin)
    {
        spriteLibrary.spriteLibraryAsset = newSkin;

        foreach(var r in GetComponentsInChildren<SpriteResolver>(true))
        {
            r.ResolveSpriteToSpriteRenderer();
        }
    }
}
