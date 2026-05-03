using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponManager : MonoBehaviour
{
    //Weapon variables
    public WeaponClasses weapons;

    //Raycast ignore layer
    public LayerMask ignoreLayer;

    //Fire weapon function
    public void Fire(Transform bulletSpawn, Transform recoilController, Weapons currentWeaponData, Transform eyeSight, Material bulletMat, bool isPlayer, Player_CameraController cameraController)
    {
        //Fire a bullet for the current weapon's bullet count amount of times
        for (int i = 0; i < currentWeaponData.bulletCount; i++)
        {
            //Instantiate the bullet prefab and set it's variables set the bullet's damage
            GameObject bullet = Instantiate(currentWeaponData.bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
            bullet.GetComponent<MeshRenderer>().material = bulletMat;
            bullet.GetComponent<Bullet>().damage = currentWeaponData.damage;
            if (isPlayer)
                bullet.GetComponent<Bullet>().isEnemyBullet = false;
            else
                bullet.GetComponent<Bullet>().isEnemyBullet = true;

            //Raycast where the camera is looking
            RaycastHit hit;
            var ray = new Ray(eyeSight.position, eyeSight.forward);

            //Make the bullet look towards the raycast point if it exists, else make the bullet look forward towards the camera
            if (Physics.Raycast(ray, out hit, 1000f, ~ignoreLayer, QueryTriggerInteraction.Ignore))
                bullet.transform.LookAt(hit.point);
            else
                bullet.transform.rotation = eyeSight.rotation;


            //Get the direction the bullet is facing + a random Vector3 made from the spread radius
            Vector3 dir = bullet.transform.forward + new Vector3(UnityEngine.Random.Range(-currentWeaponData.spreadRadius, currentWeaponData.spreadRadius), UnityEngine.Random.Range(-currentWeaponData.spreadRadius, currentWeaponData.spreadRadius), UnityEngine.Random.Range(-currentWeaponData.spreadRadius, currentWeaponData.spreadRadius));

            //Add force to the bullet by the dir variable * the bullet speed, using ForceMode.Impulse for immediate fixed speed
            bullet.GetComponent<Rigidbody>().AddForce(dir * currentWeaponData.bulletSpeed, ForceMode.Impulse);

            if (isPlayer)
                Recoil(recoilController, currentWeaponData, cameraController);

            //Set the weapon to hasShot
            currentWeaponData.hasShot = true;

            currentWeaponData.coolingCDTimer += currentWeaponData.coolingCDIncrease;
        }
    }

    //Recoil function
    public void Recoil(Transform recoilController, Weapons currentWeaponData, Player_CameraController cameraController)
    {
        // Rotate up by recoil amount
        cameraController.addedPitch = -currentWeaponData.recoilAmount;
    }

    //Aiming function
    public void Aim(GameObject currentWeapon, Transform weaponPosition, Transform adsPosition, Weapons currentWeaponData, bool isAiming, bool isPlayer)
    {
        //If the entity is aiming, lerp the weapon position to the adsPosition, else lerp it back to the weaponPosition
        if (isAiming)
        {
            if (currentWeaponData.adsTimer < 1)
                currentWeaponData.adsTimer += currentWeaponData.adsSpeed * 0.8f * Time.deltaTime;
            else
                currentWeaponData.adsTimer = 1;

            currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, adsPosition.position, currentWeaponData.adsSpeed * 1.2f * Time.deltaTime);
            if (isPlayer)
                Camera.main.fieldOfView = Mathf.Lerp(60, 60/currentWeaponData.adsScope, currentWeaponData.adsTimer);
        }
        else
        {
            if (currentWeaponData.adsTimer > 0)
                currentWeaponData.adsTimer -= currentWeaponData.adsSpeed * 0.8f * Time.deltaTime;
            else
                currentWeaponData.adsTimer = 0;

            currentWeapon.transform.position = Vector3.Lerp(currentWeapon.transform.position, weaponPosition.transform.position, currentWeaponData.adsSpeed * 1.2f * Time.deltaTime);
            if (isPlayer)
                Camera.main.fieldOfView = Mathf.Lerp(60, 60/currentWeaponData.adsScope, currentWeaponData.adsTimer);
        }
    }

    //Swap weapons function
    public void SwapWeapon(GameObject currentWeapon, int weaponType, WeaponClasses weaponList, int weaponIndex, Transform weaponPosition, Weapons currentWeaponData, Transform bulletSpawn, Player_WeaponHandler weaponHandler, int currentWeaponType, Transform eyeSight)
    {
        //Found weapon variable
        bool foundWeapon = false;

        //If the weapon type is primary, swap to an available primary weapon
        if (weaponType == 1)
        {
            if (currentWeaponType != 1)
            {
                //Search for weapon
                foreach (Weapons weapon in weaponList.primaryWeapons)
                {
                    //If the weapon found is set up correctly, equip that weapon
                    if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                    {
                        //Destroy the current weapon
                        Destroy(currentWeapon);
                        //Create weapon
                        weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                        weaponHandler.currentWeapon.transform.Rotate(Vector3.up, -90);
                        weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                        //Set new weapon data
                        weaponHandler.currentWeaponData = weapon;
                        weaponHandler.weaponIndex = Array.IndexOf(weaponList.primaryWeapons, weapon);
                        weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                        weaponHandler.currentWeaponType = 1;
                        return;
                    }
                }
            } 
            else
            {
                //If the current weapon is the last in the primary weapons list, search for the first available weapon
                if (weaponIndex >= weaponList.primaryWeapons.Length - 1)
                {
                    //Search for weapon
                    foreach (Weapons weapon in weaponList.primaryWeapons)
                    {
                        //If the weapon found is not the same as the current weapon and it is set up correctly, equip that weapon
                        if (Array.IndexOf(weaponList.primaryWeapons, weapon) != weaponIndex)
                        {
                            if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                            {
                                //Destroy the current weapon
                                Destroy(currentWeapon);
                                //Create weapon
                                weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                weaponHandler.currentWeapon.transform.Rotate(Vector3.up, -90);
                                weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                //Set new weapon data
                                weaponHandler.currentWeaponData = weapon;
                                weaponHandler.weaponIndex = Array.IndexOf(weaponList.primaryWeapons, weapon);
                                weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                weaponHandler.currentWeaponType = 1;
                                return;
                            }
                        }
                    }
                }
                //If the current weapon is not the last in the primary weapons list, search for the next available weapon
                else
                {
                    //Search for weapon
                    foreach (Weapons weapon in weaponList.primaryWeapons)
                    {
                        //If the weapon found is not before the current weapon in the list and it is set up correctly, equip that weapon
                        if (Array.IndexOf(weaponList.primaryWeapons, weapon) > weaponIndex)
                        {
                            if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                            {
                                //Destroy the current weapon
                                Destroy(currentWeapon);
                                //Create weapon
                                weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                weaponHandler.currentWeapon.transform.Rotate(Vector3.up, -90);
                                weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                //Set new weapon data
                                weaponHandler.currentWeaponData = weapon;
                                weaponHandler.weaponIndex = Array.IndexOf(weaponList.primaryWeapons, weapon);
                                weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                //Set found weapon to true
                                foundWeapon = true;

                                weaponHandler.currentWeaponType = 1;
                                return;
                            }
                        }
                    }
                    //If the next available weapon was not found, search for the first available weapon
                    if (!foundWeapon)
                    {
                        //Search for weapon
                        foreach (Weapons weapon in weaponList.primaryWeapons)
                        {
                            //If the weapon found is not the same as the current weapon and it is set up correctly, equip that weapon
                            if (Array.IndexOf(weaponList.primaryWeapons, weapon) != weaponIndex)
                            {
                                if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                                {
                                    //Destroy the current weapon
                                    Destroy(currentWeapon);
                                    //Create weapon
                                    weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                    weaponHandler.currentWeapon.transform.Rotate(Vector3.up, -90);
                                    weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                    //Set new weapon data
                                    weaponHandler.currentWeaponData = weapon;
                                    weaponHandler.weaponIndex = Array.IndexOf(weaponList.primaryWeapons, weapon);
                                    weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                    weaponHandler.currentWeaponType = 1;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
        //If the weapon type is secondary, swap to an available secondary weapon
        else if (weaponType == 2)
        {
            if (currentWeaponType != 2)
            {
                //Search for weapon
                foreach (Weapons weapon in weaponList.secondaryWeapons)
                {
                    //If the weapon found is set up correctly, equip that weapon
                    if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                    {
                        //Destroy the current weapon
                        Destroy(currentWeapon);
                        //Create weapon
                        weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                        weaponHandler.currentWeapon.transform.Rotate(Vector3.up, 90.2f);
                        weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                        //Set new weapon data
                        weaponHandler.currentWeaponData = weapon;
                        weaponHandler.weaponIndex = Array.IndexOf(weaponList.secondaryWeapons, weapon);
                        weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                        weaponHandler.currentWeaponType = 2;
                        return;
                    }
                }
            }
            else
            {
                //If the current weapon is the last in the secondary weapons list, search for the first available weapon
                if (weaponIndex >= weaponList.secondaryWeapons.Length - 1)
                {
                    //Search for weapon
                    foreach (Weapons weapon in weaponList.secondaryWeapons)
                    {
                        //If the weapon found is not the same as the current weapon and it is set up correctly, equip that weapon
                        if (Array.IndexOf(weaponList.secondaryWeapons, weapon) != weaponIndex)
                        {
                            if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                            {
                                //Destroy the current weapon
                                Destroy(currentWeapon);
                                //Create weapon
                                weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                weaponHandler.currentWeapon.transform.Rotate(Vector3.up, 90.2f);
                                weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                //Set new weapon data
                                weaponHandler.currentWeaponData = weapon;
                                weaponHandler.weaponIndex = Array.IndexOf(weaponList.secondaryWeapons, weapon);
                                weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                weaponHandler.currentWeaponType = 2;
                                return;
                            }
                        }
                    }
                }
                //If the current weapon is not the last in the secondary weapons list, search for the next available weapon
                else
                {
                    //Search for weapon
                    foreach (Weapons weapon in weaponList.secondaryWeapons)
                    {
                        //If the weapon found is not before the current weapon in the list and it is set up correctly, equip that weapon
                        if (Array.IndexOf(weaponList.secondaryWeapons, weapon) > weaponIndex)
                        {
                            if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                            {
                                //Destroy the current weapon
                                Destroy(currentWeapon);
                                //Create weapon
                                weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                weaponHandler.currentWeapon.transform.Rotate(Vector3.up, 90.2f);
                                weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                //Set new weapon data
                                weaponHandler.currentWeaponData = weapon;
                                weaponHandler.weaponIndex = Array.IndexOf(weaponList.secondaryWeapons, weapon);
                                weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                //Set found weapon to true
                                foundWeapon = true;

                                weaponHandler.currentWeaponType = 2;
                                return;
                            }
                        }
                    }
                    //If the next available weapon was not found, search for the first available weapon
                    if (!foundWeapon)
                    {
                        //Search for weapon
                        foreach (Weapons weapon in weaponList.secondaryWeapons)
                        {
                            //If the weapon found is not the same as the current weapon and it is set up correctly, equip that weapon
                            if (Array.IndexOf(weaponList.secondaryWeapons, weapon) != weaponIndex)
                            {
                                if (weapon.weaponPrefab && weapon.bulletPrefab && weapon.unlocked)
                                {
                                    //Destroy the current weapon
                                    Destroy(currentWeapon);
                                    //Create weapon
                                    weaponHandler.currentWeapon = Instantiate(weapon.weaponPrefab, eyeSight, false);
                                    weaponHandler.currentWeapon.transform.Rotate(Vector3.up, 90.2f);
                                    weaponHandler.currentWeapon.transform.position = weaponPosition.position;
                                    //Set new weapon data
                                    weaponHandler.currentWeaponData = weapon;
                                    weaponHandler.weaponIndex = Array.IndexOf(weaponList.secondaryWeapons, weapon);
                                    weaponHandler.bulletSpawn = weaponHandler.currentWeapon.transform.GetChild(0);

                                    weaponHandler.currentWeaponType = 2;
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //Cooldown functions
    public void Cooldowns(WeaponClasses weaponList)
    {
        if (weaponList.primaryWeapons != null)
            foreach (Weapons weapon in weaponList.primaryWeapons)
            {
                //If the weapon is cooling and is not recharging, decrease the cooldown timer until it is less than or equal to 0
                if (weapon.isCooling && !weapon.recharging)
                {
                    if (weapon.coolingCDTimer <= 0)
                    {
                        weapon.coolingCDTimer = 0;
                        weapon.isCooling = false;
                    }
                    else
                        weapon.coolingCDTimer -= Time.deltaTime;
                }
                //If the weapon is recharging, drecrease the cooldown timer faster until it is less than or equal to 0
                else if (weapon.isCooling && weapon.recharging)
                {
                    if (weapon.coolingCDTimer <= 0)
                    {
                        weapon.coolingCDTimer = 0;
                        weapon.isCooling = false;
                    }
                    else
                        weapon.coolingCDTimer -= Time.deltaTime * weapon.rechargingSpeed;
                }
                //If the weapon is not cooling but its cooldownCDTimer is greater than 0, decrease the cooldown timer by Time.deltaTime
                else if (weapon.coolingCDTimer > 0 && !weapon.hasShot)
                    weapon.coolingCDTimer -= Time.deltaTime;
                //If the weapon's coolingCDTimer is greater than or equal to the coolingCooldown, set the weapon cooling to true
                if (weapon.coolingCDTimer >= weapon.coolingCooldown)
                    weapon.isCooling = true;

                //If the weapon has shot, increase the rate of fire cooldown until it is greater tahn or equal to the cooldown length
                if (weapon.hasShot)
                {
                    if (weapon.rateOfFireCDTimer >= weapon.rateOfFire)
                    {
                        weapon.rateOfFireCDTimer = 0;
                        weapon.hasShot = false;
                    }
                    else
                        weapon.rateOfFireCDTimer += Time.deltaTime;
                }
            }
        if (weaponList.secondaryWeapons != null)
            foreach (Weapons weapon in weaponList.secondaryWeapons)
            {
                //If the weapon is cooling and is not recharging, decrease the cooldown timer until it is less than or equal to 0
                if (weapon.isCooling && !weapon.recharging)
                {
                    if (weapon.coolingCDTimer <= 0)
                    {
                        weapon.coolingCDTimer = 0;
                        weapon.isCooling = false;
                    }
                    else
                        weapon.coolingCDTimer -= Time.deltaTime;
                }
                //If the weapon is recharging, drecrease the cooldown timer faster until it is less than or equal to 0
                else if (weapon.isCooling && weapon.recharging)
                {
                    if (weapon.coolingCDTimer <= 0)
                    {
                        weapon.coolingCDTimer = 0;
                        weapon.isCooling = false;
                    }
                    else
                        weapon.coolingCDTimer -= Time.deltaTime * weapon.rechargingSpeed;
                }
                //If the weapon is not cooling but its cooldownCDTimer is greater than 0, decrease the cooldown timer by Time.deltaTime
                else if (weapon.coolingCDTimer > 0)
                    weapon.coolingCDTimer -= Time.deltaTime;
                //If the weapon's coolingCDTimer is greater than or equal to the coolingCooldown, set the weapon cooling to true
                if (weapon.coolingCDTimer >= weapon.coolingCooldown)
                    weapon.isCooling = true;

                //If the weapon has shot, increase the rate of fire cooldown until it is greater tahn or equal to the cooldown length
                if (weapon.hasShot)
                {
                    if (weapon.rateOfFireCDTimer >= weapon.rateOfFire)
                    {
                        weapon.rateOfFireCDTimer = 0;
                        weapon.hasShot = false;
                    }
                    else
                        weapon.rateOfFireCDTimer += Time.deltaTime;
                }
            }
    }
}

//Weapon classes
[Serializable]
public class WeaponClasses
{
    public Weapons[] primaryWeapons;
    public Weapons[] secondaryWeapons;

    public WeaponClasses(WeaponClasses other)
    {
        primaryWeapons = new Weapons[other.primaryWeapons.Length];
        secondaryWeapons = new Weapons[other.secondaryWeapons.Length];
        for (int i = 0; i < other.primaryWeapons.Length; i++)
        {
            this.primaryWeapons[i] = new Weapons(other.primaryWeapons[i]);
        }
        for (int i = 0; i < other.secondaryWeapons.Length; i++)
        {
            this.secondaryWeapons[i] = new Weapons(other.secondaryWeapons[i]);
        }
    }
}
//Weapon data
[Serializable]
public class Weapons
{
    //Weapon information
    [Header("Weapon Information")]
    public string weaponName;
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public bool unlocked;

    //Weapon specs
    [Header("Weapon Specs")]
    [Range(1,2)]
    [Tooltip("1 Auto, 2 Semi-Auto")]
    public int fireMode = 1;
    public float damage;
    [Range(0,0.5f)]
    public float spreadRadius;
    public int bulletCount;
    public float bulletSpeed;
    public float rateOfFire;
    public float recoilAmount;
    public float adsSpeed;
    public float adsScope;
    [HideInInspector] public float adsTimer;

    //Rate of fire variables
    [HideInInspector] public bool hasShot;
    [HideInInspector] public float rateOfFireCDTimer;

    //Cooling variables
    [HideInInspector] public bool isCooling;
    [HideInInspector] public bool recharging;
    [Header("Cooling Variables")]
    public float coolingCooldown;
    [HideInInspector] public float coolingCDTimer;
    public float coolingCDIncrease;
    public float rechargingSpeed = 1;

    // Audio variables
    public AudioClip[] fireSFX;

    public Weapons(Weapons other)
    {
        this.weaponName = other.weaponName;
        this.weaponPrefab = other.weaponPrefab;
        this.bulletPrefab = other.bulletPrefab;
        this.unlocked = other.unlocked;

        this.fireMode = other.fireMode;
        this.damage = other.damage;
        this.spreadRadius = other.spreadRadius;
        this.bulletCount = other.bulletCount;
        this.bulletSpeed = other.bulletSpeed;
        this.rateOfFire = other.rateOfFire;
        this.recoilAmount = other.recoilAmount;
        this.adsSpeed = other.adsSpeed;
        this.adsScope = other.adsScope;
        this.adsTimer = other.adsTimer;

        this.hasShot = other.hasShot;
        this.rateOfFireCDTimer = other.rateOfFireCDTimer;

        this.isCooling = other.isCooling;
        this.recharging = other.recharging;
        this.coolingCooldown = other.coolingCooldown;
        this.coolingCDTimer = other.coolingCDTimer;
        this.coolingCDIncrease = other.coolingCDIncrease;
        this.rechargingSpeed = other.rechargingSpeed;

        this.fireSFX = other.fireSFX;
    }
}