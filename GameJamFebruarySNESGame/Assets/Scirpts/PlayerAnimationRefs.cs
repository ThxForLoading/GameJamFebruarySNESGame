using UnityEngine;

public class PlayerAnimationRefs : MonoBehaviour
{
    [SerializeField] private Animator animatorOnChild;

    public Animator Animator => animatorOnChild;

    void Awake()
    {
        if (!animatorOnChild)
            animatorOnChild = GetComponentInChildren<Animator>(true);
    }
}
