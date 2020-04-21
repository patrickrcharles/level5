using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class droppedObject : MonoBehaviour {

    public float dollarAmount;
    public int enemyId;
    public float lifetime = 10;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "droppedObject" && other.gameObject.tag == "playerHitbox")
        {
           //Debug.Log("money picked up");
            //inventory.instance.addToTotalMoney(dollarAmount);
            
            Destroy(gameObject);
        }
    }
}
