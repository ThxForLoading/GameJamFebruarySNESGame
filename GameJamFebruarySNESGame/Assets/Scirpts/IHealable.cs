using UnityEngine;

public interface IHealable
{
    void Heal(int amount);
    bool CanHeal { get; }
}
