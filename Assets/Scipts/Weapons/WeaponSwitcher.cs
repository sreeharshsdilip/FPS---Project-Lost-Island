using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField]
    private int selectedWeapon = 0;

    [SerializeField]
    private GameObject[] weapons;
    
    private bool[] weaponAvailable;

    private void Start()
    {
        weaponAvailable = new bool[weapons.Length];
        weaponAvailable[0] = true; // Make first weapon available by default
        SelectWeapon();
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        HandleNumberKeyInput();
        HandleScrollWheelInput();

        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

    private void HandleNumberKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && weaponAvailable[0])
            selectedWeapon = 0;
        if (Input.GetKeyDown(KeyCode.Alpha2) && weapons.Length > 1 && weaponAvailable[1])
            selectedWeapon = 1;
        if (Input.GetKeyDown(KeyCode.Alpha3) && weapons.Length > 2 && weaponAvailable[2])
            selectedWeapon = 2;
    }

    private void HandleScrollWheelInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            int nextWeapon = selectedWeapon;
            do
            {
                nextWeapon = (nextWeapon + 1) % weapons.Length;
                if (weaponAvailable[nextWeapon])
                {
                    selectedWeapon = nextWeapon;
                    break;
                }
            } while (nextWeapon != selectedWeapon);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            int nextWeapon = selectedWeapon;
            do
            {
                nextWeapon--;
                if (nextWeapon < 0)
                    nextWeapon = weapons.Length - 1;
                if (weaponAvailable[nextWeapon])
                {
                    selectedWeapon = nextWeapon;
                    break;
                }
            } while (nextWeapon != selectedWeapon);
        }
    }

    private void SelectWeapon()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
            {
                weapons[i].SetActive(i == selectedWeapon && weaponAvailable[i]);
            }
        }
    }

    public void UnlockWeapon(int weaponIndex)
    {
        if (weaponIndex >= 0 && weaponIndex < weapons.Length)
        {
            weaponAvailable[weaponIndex] = true;
        }
    }
}
