using UnityEngine;
using System.Collections.Generic;

public class Ammo : MonoBehaviour
{
    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType ammoType;
        public int ammoAmount;
    }

    [SerializeField] private AmmoSlot[] ammoSlots;

    public int GetCurrentAmmo(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot == null)
        {
            Debug.LogWarning($"Ammo type {ammoType} not found in ammo slots.");
            return 0;
        }
        return slot.ammoAmount;
    }

    public void RemoveAmmo(AmmoType ammoType)
    {
        GetAmmoSlot(ammoType).ammoAmount--;
    }

    public void AddAmmo(AmmoType ammoType, int amount)
    {
        AmmoSlot slot = GetAmmoSlot(ammoType);
        if (slot != null)
        {
            slot.ammoAmount += amount;
        }
        else
        {
            Debug.LogWarning($"Ammo type {ammoType} not found in ammo slots.");
        }
    }

    private AmmoSlot GetAmmoSlot(AmmoType ammoType)
    {
        foreach (var slot in ammoSlots)
        {
            if (slot.ammoType == ammoType)
                return slot;
        }
        return null;
    }
}