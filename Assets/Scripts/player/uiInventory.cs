//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;

//public class uiInventory : MonoBehaviour {

//    public GameObject imageWeapon, imageMoney, imageMelee, imageThrow;
//    public GameObject textWeaponAmmo, textMoneyAmount, textMeleeAmount, textThrowAmount;


//    public static GameObject uiWeapon;

//    //weapons
//    public Sprite magnum, ak47, deathray, uzi, shotgun, flamethrower, rocketLauncher, peacemaker, freezeRay;
//    //melee
//    public Sprite punch, bat, spikedBat, whip;
//    //throw
//    public Sprite smokebombs, molotov;

//    public Image currentWeapon, money, currentMelee, currentThrow;
//    public Text currentWeaponAmmo, currentMoneyAmount, currentMeleePercent, currentThrowAmount;

//    [SerializeField]
//    playercontrollerscript player;

//    decimal i = 0;

//	// Use this for initialization
//	void Start () {

//        //Debug.Log("uiIventory ::::: void Awake()");
//        currentWeapon = imageWeapon.GetComponent<Image>();
//        currentMelee = imageMelee.GetComponent<Image>();
//        currentThrow = imageThrow.GetComponent<Image>();
//        money = imageMoney.GetComponent<Image>();
//        currentWeaponAmmo = textWeaponAmmo.GetComponent<Text>();
//        currentMoneyAmount = textMoneyAmount.GetComponent<Text>();
//        currentMeleePercent = textMeleeAmount.GetComponent<Text>();
//        currentThrowAmount = textThrowAmount.GetComponent<Text>();
//        uiWeapon = imageWeapon;
//        player = gameManager.instance.playerState;

//    }
	
//	// Update is called once per frame
//	void Update () {

//        //setCurrentWeapon(player.currentWeapon);
//    }


//    public void setCurrentWeapon( string currentWpn)
//    {
//        //Debug.Log("current weapon : " + currentWpn);
//        //Debug.Log("current weapon : " + player.weapons.gunAmmoCapacity);

//        if (currentWpn == "magnum")
//        {
//            currentWeapon.overrideSprite = magnum;
//            currentWeaponAmmo.text = "x "+player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "ak47")
//        {
//            currentWeapon.overrideSprite = ak47;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "deathRay")
//        {
//            currentWeapon.overrideSprite = deathray;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "uzi")
//        {
//            currentWeapon.overrideSprite = uzi;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "shotgun")
//        {
//            currentWeapon.overrideSprite = shotgun;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "flamethrower")
//        {
//            currentWeapon.overrideSprite = flamethrower;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "rocketLauncher")
//        {
//            currentWeapon.overrideSprite = rocketLauncher;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "molotov")
//        {
//            currentWeapon.overrideSprite = molotov;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "peacemaker")
//        {
//            currentWeapon.overrideSprite = peacemaker;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//        if (currentWpn == "freezeRay")
//        {
//            currentWeapon.overrideSprite = freezeRay;
//            currentWeaponAmmo.text = "x " + player.gunAmmoCapacity.ToString();
//        }
//    }

//    public void setMelee(string currentWpn)
//    {
//        //Debug.Log("public void setMelee(string currentWpn)");
//        if (currentWpn == "rightPunch")
//        {
//            //Debug.Log("current weapon : " + currentWpn);
//            currentMelee.overrideSprite = punch;
//            currentMeleePercent.text = "x 100%"; 
//        }
//        if (currentWpn == "bat")
//        {
//            currentMelee.overrideSprite = bat;
//            currentMeleePercent.text = "x 100%";
//        }
//        if (currentWpn == "spikedBat")
//        {
//            currentMelee.overrideSprite = spikedBat;
//            currentMeleePercent.text = "x 100%";
//        }
//        if (currentWpn == "whip")
//        {
//            currentMelee.overrideSprite = whip;
//            currentMeleePercent.text = "x 100%";
//        }
//    }

//    public void setThrow(string currentWpn)
//    {
//        //Debug.Log("current throw : " + currentWpn);

//        if (currentWpn == "molotov")
//        {
//            //Debug.Log("current weapon : " + currentWpn);
//            currentThrow.overrideSprite = molotov;
//            currentThrowAmount.text = "x infinite";
//        }
//        if (currentWpn == "smokebomb")
//        {
//            currentThrow.overrideSprite = smokebombs;
//            currentThrowAmount.text = "x infinite";
//        }
//    }

//    public void setCurrentMoney(string currentMoney)
//    {
//        if(currentMoney == null)
//        {
//            currentMoneyAmount.text = "$" + 0;
//        }
//        currentMoneyAmount.text = "$"+currentMoney;
//    }

//    public static void setUIweaponDisplay(bool value)
//    {
//        uiWeapon.SetActive(value);
//    }
//}
