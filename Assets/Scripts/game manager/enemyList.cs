using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyList : MonoBehaviour {

    public List<GameObject> enemyName;

    public static enemyList instance;


	
	// Update is called once per frame
	private void Awake () {
        instance = this;
	}
}
