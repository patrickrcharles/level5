//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class inventory : MonoBehaviour {


//    [SerializeField] decimal totalMoney;
//    public static inventory instance;
//    uiInventory uiInventory;

//    // Use this for initialization
//    void Start () {

//        uiInventory = GameObject.FindGameObjectWithTag("Player").GetComponent<uiInventory>();
//        //Debug.Log("inventory - gameobject : " + gameObject.name);
//        instance = this;
//	}
	
//	// Update is called once per frame
//	void Update () {
		
//	}

//    public void addToTotalMoney(float amount)
//    {
//        totalMoney += (decimal)amount;
//        //Debug.Log("total money.tostring()  : " + totalMoney.ToString());
//        uiInventory.setCurrentMoney(totalMoney.ToString());
//    }

//    private int _magnumAmmo;
//    public int magnumAmmo
//    {
//        get { return  _magnumAmmo; }
//        set { _magnumAmmo = value; }
//    }
//    private int _ak47Ammo;
//    public int ak47Ammo
//    {
//        get { return _ak47Ammo; }
//        set { _ak47Ammo = value; }
//    }
//    private int _shotgunAmmo;
//    public int shotgunAmmo
//    {
//        get { return _shotgunAmmo; }
//        set { _shotgunAmmo = value; }
//    }
//    private int _deathrayAmmo;
//    public int deathrayAmmo
//    {
//        get { return _deathrayAmmo; }
//        set { _deathrayAmmo = value; }
//    }
//    private int _flamethrowerFuelAmmo;
//    public int flamethrowerFuelAmmo
//    {
//        get { return _flamethrowerFuelAmmo; }
//        set { _flamethrowerFuelAmmo = value; }
//    }
//    private int _RocketLauncherAmmo;
//    public int RocketLauncherAmmo
//    {
//        get { return _RocketLauncherAmmo; }
//        set { _RocketLauncherAmmo = value; }
//    }
//    private int _batHealth;
//    public int batHealth
//    {
//        get { return _batHealth; }
//        set { _batHealth = value; }
//    }
//    private int _spikedBatHealth;
//    public int spikedBatHealth
//    {
//        get { return _spikedBatHealth; }
//        set { _spikedBatHealth = value; }
//    }
//    private int _whipHealth;
//    public int whipHealth
//    {
//        get { return _whipHealth; }
//        set { _whipHealth = value; }
//    }
//    private int _cigarettes;
//    public int cigarettes
//    {
//        get { return _cigarettes; }
//        set { _cigarettes = value; }
//    }
//    private int _smokebombs;
//    public int smokebombs
//    {
//        get { return _smokebombs; }
//        set { _smokebombs = value; }
//    }
//    private int _molotovs;
//    public int molotovs
//    {
//        get { return _molotovs; }
//        set { _molotovs = value; }
//    }
//}
