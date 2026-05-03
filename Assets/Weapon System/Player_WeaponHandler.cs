using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player_WeaponHandler : MonoBehaviour
{
    //Weapon manager variable
    private WeaponManager weaponManager;

    //Weapon data
    private WeaponClasses weaponList;
    [SerializeField] public GameObject currentWeapon;
    [SerializeField] public Weapons currentWeaponData;
    [HideInInspector] public Transform bulletSpawn;
    private bool hasWeaponEquipped;
    [SerializeField] private Transform eyeSight;
    [SerializeField] private Transform weaponPosition;
    [SerializeField] private Transform ADSPosition;

    //Weapon index variables
    [HideInInspector] public int weaponIndex;
    private int weaponCategory;
    [HideInInspector] public int currentWeaponType;

    //Individual variables
    [SerializeField] private Material playerBulletMat;

    //Weapon heat slider variable
    public Slider Heat;
    public Slider HeatCrosshair;

    //Recoil variables
    public Player_CameraController cameraController;

    //Audio
    [SerializeField]
    [Tooltip("The audio to play when the player has taken an ar shot")]
    private AudioClip[] audioClipsAR;
    [SerializeField]
    [Tooltip("The audio to play when the player has taken a shotgun shot")]
    private AudioClip[] audioClipsShotgun;
    [SerializeField]
    private AudioSource audioSource;

    // Input variables
    private Player_InputActions input;

    private void Start()
    {
        // Get input actions
        input = InputHandler.instance.input;

        weaponManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<WeaponManager>();
        weaponList = new WeaponClasses(weaponManager.weapons);

        //Equip the first available weapon
        if (!hasWeaponEquipped)
        {
            //If there is a primary weapon available, spawn with that
            foreach (Weapons weapon in weaponList.primaryWeapons)
            {
                //If the weapon in the primary weapons list is available and unlocked, start with that weapon
                if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                {
                    //Create weapon
                    currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                    currentWeapon.transform.Rotate(Vector3.up, -90);
                    currentWeapon.transform.position = weaponPosition.position;
                    //Set weapon data
                    currentWeaponData = weapon;
                    weaponIndex = Array.IndexOf(weaponList.primaryWeapons, weapon);
                    bulletSpawn = currentWeapon.transform.GetChild(0);
                    currentWeapon.layer = 11;
                    foreach (Transform child in currentWeapon.transform)
                        child.gameObject.layer = 11;

                    hasWeaponEquipped = true;
                    currentWeaponType = 1;
                    return;
                }
            }
            //If there is a secondary weapon available, spawn with that
            foreach (Weapons weapon in weaponList.secondaryWeapons)
            {
                //If the weapon in the secondary weapons list is available and unlocked, start with that weapon
                if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                {
                    //Create weapon
                    currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                    currentWeapon.transform.Rotate(Vector3.up, 90.2f);
                    currentWeapon.transform.position = weaponPosition.position;
                    //Set weapon data
                    currentWeaponData = weapon;
                    weaponIndex = Array.IndexOf(weaponList.secondaryWeapons, weapon);
                    bulletSpawn = currentWeapon.transform.GetChild(0);
                    currentWeapon.layer = 11;
                    foreach (Transform child in currentWeapon.transform)
                        child.gameObject.layer = 11;

                    hasWeaponEquipped = true;
                    currentWeaponType = 2;
                    return;
                }
            }        
        }
    }

    //== On Update
    private void Update()
    {
        // Cooldowns
        weaponManager.Cooldowns(weaponList);

        // Rotate camera root down if recoiled
        if (cameraController.transform.localRotation.eulerAngles.x > 0)
            cameraController.transform.Rotate(new Vector3(1f * Time.deltaTime, 0, 0));

        if (currentWeapon)
        {
            // Fire weapon
            if (currentWeaponData.fireMode == 1 && !currentWeaponData.isCooling && !currentWeaponData.hasShot && Time.timeScale != 0)
            {
                if (input.Player.Shoot.IsPressed()) {
                    weaponManager.Fire(bulletSpawn, eyeSight, currentWeaponData, eyeSight, playerBulletMat, true, cameraController);
                    //audioSource.pitch = UnityEngine.Random.Range(2, 3.5f);
                    audioSource.clip = currentWeaponData.fireSFX[UnityEngine.Random.Range(0, currentWeaponData.fireSFX.Length)];
                    audioSource.Play();
                }
            }
            else if (currentWeaponData.fireMode == 2 && !currentWeaponData.isCooling && !currentWeaponData.hasShot && Time.timeScale != 0)
            {
                if (input.Player.Shoot.WasPressedThisFrame()) {
                    weaponManager.Fire(bulletSpawn, eyeSight, currentWeaponData, eyeSight, playerBulletMat, true, cameraController);
                    //audioSource.pitch = UnityEngine.Random.Range(2, 3.5f);
                    audioSource.clip = currentWeaponData.fireSFX[UnityEngine.Random.Range(0, currentWeaponData.fireSFX.Length)];
                    audioSource.Play();
                }
            }

            // Aim weapon
            if (input.Player.Aim.IsPressed())
                weaponManager.Aim(currentWeapon, weaponPosition, ADSPosition, currentWeaponData, true, true);
            else
                weaponManager.Aim(currentWeapon, weaponPosition, ADSPosition, currentWeaponData, false, true);
        }
       

        if (input.Player.Weapon1.WasPressedThisFrame())
        {
            weaponManager.SwapWeapon(currentWeapon, 1, weaponList, weaponIndex, weaponPosition, currentWeaponData, bulletSpawn, gameObject.GetComponent<Player_WeaponHandler>(), currentWeaponType, eyeSight);
            currentWeapon.layer = 11;
            foreach (Transform child in currentWeapon.transform)
                child.gameObject.layer = 11;

            weaponCategory = 1;
        }
        if (input.Player.Weapon2.WasPressedThisFrame())
        {
            weaponManager.SwapWeapon(currentWeapon, 2, weaponList, weaponIndex, weaponPosition, currentWeaponData, bulletSpawn, gameObject.GetComponent<Player_WeaponHandler>(), currentWeaponType, eyeSight);
            currentWeapon.layer = 11;
            foreach (Transform child in currentWeapon.transform)
                child.gameObject.layer = 11;

            weaponCategory = 2;
        }
        if (Mouse.current.scroll.ReadValue().y > 0)
        {
            if (weaponCategory == 1)
            {
                weaponCategory = 2;
            }
            else
            {
                weaponCategory = 1;
            }

            weaponManager.SwapWeapon(currentWeapon, weaponCategory, weaponList, weaponIndex, weaponPosition, currentWeaponData, bulletSpawn, gameObject.GetComponent<Player_WeaponHandler>(), currentWeaponType, eyeSight);
            currentWeapon.layer = 11;
            foreach (Transform child in currentWeapon.transform)
                child.gameObject.layer = 11;
        }
        else if (Mouse.current.scroll.ReadValue().y < 0)
        {
            
            if (weaponCategory == 1)
            {
                weaponCategory = 2;
            }
            else
            {
                weaponCategory = 1;
            }

            weaponManager.SwapWeapon(currentWeapon, weaponCategory, weaponList, weaponIndex, weaponPosition, currentWeaponData, bulletSpawn, gameObject.GetComponent<Player_WeaponHandler>(), currentWeaponType, eyeSight);
            currentWeapon.layer = 11;
            foreach (Transform child in currentWeapon.transform)
                child.gameObject.layer = 11;
        }

        ////Adjust overheat slider to the current weapon's heat level
        //Heat.value = currentWeaponData.coolingCDTimer / currentWeaponData.coolingCooldown;

        HeatCrosshair.value = (currentWeaponData.coolingCDTimer / currentWeaponData.coolingCooldown);
    }
}