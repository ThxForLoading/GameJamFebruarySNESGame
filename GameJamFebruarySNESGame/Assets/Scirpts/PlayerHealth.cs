using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable, IHealable
{

    public int maxHp = 6;
    public int hp;
    [SerializeField] private ResolverFrameDriver frameDriver;

    private PlayerKnockback knockback;

    private GameObject audioManager;

    void Awake()
    {
        hp = maxHp;
        knockback = GetComponent<PlayerKnockback>();
    }

    private void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("AudioManager");

        if (audioManager == null) Debug.Log("Audiomanager was not found, playing no audio");
    }

    public bool CanHeal => hp < maxHp;

    public void Heal(int amount)
    {
        if (amount <= 0) return;
        hp = Mathf.Min(maxHp, hp + amount);

        if (hp >= 3)
            frameDriver.SetCategory("Full");
        else if (hp == 2)
            frameDriver.SetCategory("Yellow");
        else
            frameDriver.SetCategory("Red");

        Debug.Log($"Healed by {amount}. HP is now: {hp}/{maxHp}");
    }

    public void TakeDamage(int amount, Vector2 hitDirection)
    {
        if (audioManager != null) audioManager.GetComponent<AudioManager>().PlayHitAudio();
        hp = Mathf.Max(0, hp - amount);
        Debug.Log($"Player took {amount} damage. HP: {hp}");

        if (knockback != null)
            knockback.Trigger(hitDirection);

        if (hp >= 3)
            frameDriver.SetCategory("Full");
        else if (hp == 2)
            frameDriver.SetCategory("Yellow");
        else
            frameDriver.SetCategory("Red");

        if (hp == 0)
        {
            Debug.Log("Player died");
            // respawn player?
        }
    }

}