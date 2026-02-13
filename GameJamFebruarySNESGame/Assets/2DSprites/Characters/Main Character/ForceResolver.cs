using UnityEngine;
using UnityEngine.U2D.Animation;

[DefaultExecutionOrder(10000)]
public class ForceResolverResolve : MonoBehaviour
{
    [SerializeField] SpriteResolver resolver;

    void LateUpdate()
    {
        resolver.ResolveSpriteToSpriteRenderer();
    }
}
