using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ActivateDarkness : MonoBehaviour
{

    public SpellHandler spellHandler;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trying to fill the void");
        spellHandler = other.GetComponentInParent<SpellHandler>();
        spellHandler.EnableDarkness();
    }
}
