using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace VikingCrewTools.Sidescroller {
    public class UIWeaponDisplay : MonoBehaviour {
        public Text txtWeapon;
        public Text txtAmmo;

        CharacterController2D character;
        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {

        }

        public void NewCharacterCallback(CharacterController2D newCharacter) {
            DeSelectCharacter(character);
            SelectCharacter(newCharacter);
        }

        void SelectCharacter(CharacterController2D newCharacter) {
            if (newCharacter != null) {
                newCharacter.GetComponent<InventoryBehaviour>().OnSwitchedWeapon += SwitchedWeaponCallback;
                SwitchedWeaponCallback(newCharacter.gun);
            }
            character = newCharacter;
        }

        void DeSelectCharacter(CharacterController2D oldCharacter) {
            if (oldCharacter != null)
                oldCharacter.GetComponent<InventoryBehaviour>().OnSwitchedWeapon -= SwitchedWeaponCallback;
            character = null;
            UpdateWeapon(null);
        }

        public void SwitchedWeaponCallback(FirearmBehaviour gun) {
            UpdateWeapon(gun);
            if (gun == null)
                return;
            gun.OnWeaponAmmoChange -= HandleWeaponAmmoChange;
            gun.OnWeaponAmmoChange += HandleWeaponAmmoChange;
        }

        void HandleWeaponAmmoChange(FirearmBehaviour gun) {
            UpdateAmmo(gun);
        }

        void UpdateAmmo(FirearmBehaviour gun) {
            if (gun != null)
                txtAmmo.text = "Ammo: " + gun.currentAmmo.ToString() + " / " + gun.data.maxAmmo.ToString();
            else
                txtAmmo.text = "Ammo: <Unarmed>";
        }

        void UpdateWeapon(FirearmBehaviour gun) {
            if (gun != null)
                txtWeapon.text = "Weapon: " + gun.name.ToString();
            else
                txtWeapon.text = "Weapon: <Unarmed>";
            UpdateAmmo(gun);
        }
    }
}