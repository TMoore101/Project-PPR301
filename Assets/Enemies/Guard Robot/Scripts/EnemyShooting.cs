using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EnemyShooting : MonoBehaviour {
    //Weapon Manager Variable
    private WeaponManager weaponManager;

    //Audio
    private AudioSource audioSource;
    public AudioClip[] Gunshots;

    //Weapon data
    private WeaponClasses weaponList;
    private GameObject currentWeapon;
    public Weapons currentWeaponData;
    public Transform BulletSpawn;
    private Transform bulletSpawn;
    private bool hasWeaponEquipped;

    public bool Shoot;
    [SerializeField] private Transform eyeSight;

    //Weapon index variables
    //private int weaponIndex;
    //private int currentWeaponType;

    //Individual variables
    [SerializeField] private Material enemyBulletMat;
    [SerializeField] private Transform arms;

    private void Start() {
        weaponManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<WeaponManager>();
        weaponList = new WeaponClasses(weaponManager.weapons);

        currentWeaponData = weaponList.primaryWeapons[1];
        bulletSpawn = BulletSpawn;
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        //Cooldowns
        weaponManager.Cooldowns(weaponList);

        if (!currentWeaponData.isCooling && !currentWeaponData.hasShot)
        {
            if (Shoot) {
                audioSource.clip = Gunshots[UnityEngine.Random.Range(0, Gunshots.Length-1)];
                audioSource.Play();
                weaponManager.Fire(bulletSpawn, arms, currentWeaponData, eyeSight, enemyBulletMat, false, null);
            }
        }
    }
}
