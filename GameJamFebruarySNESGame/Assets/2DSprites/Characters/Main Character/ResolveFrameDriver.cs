using UnityEngine;
using UnityEngine.U2D.Animation;

public class ResolverFrameDriver : MonoBehaviour
{
    [SerializeField] private SpriteResolver resolver;
    [SerializeField] private string category = "Full";
    [SerializeField] private string[] labels;

    public int frame;

    private void LateUpdate()
    {
        frame = Mathf.Clamp(frame, 0, labels.Length - 1);
        resolver.SetCategoryAndLabel(category, labels[frame]);
        resolver.ResolveSpriteToSpriteRenderer();
    }

    public void SetCategory(string newCategory)
    {
        category = newCategory;
    }
}
