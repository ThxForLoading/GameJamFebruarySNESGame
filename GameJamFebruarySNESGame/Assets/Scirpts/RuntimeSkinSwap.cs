using UnityEngine;
using UnityEngine.U2D.Animation;

public class RuntimeSkinSwap : MonoBehaviour
{
    [SerializeField] private SpriteLibrary spriteLibrary; // component on character
    [SerializeField] private SpriteLibraryAsset fullhealthSkin;
    [SerializeField] private SpriteLibraryAsset damagedSkin;
    [SerializeField] private SpriteLibraryAsset lowHealthSkin;

    public void UseFullHealth() => SetSkin(fullhealthSkin);
    public void UseDamaged() => SetSkin(damagedSkin);

    public void UseLowHealth() => SetSkin(lowHealthSkin);

    private void SetSkin(SpriteLibraryAsset skin)
    {
        spriteLibrary.spriteLibraryAsset = skin;

        // Force resolvers to refresh immediately (important if you swap mid-animation)
        foreach (var resolver in GetComponentsInChildren<SpriteResolver>(true))
            resolver.ResolveSpriteToSpriteRenderer();
    }
}