using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField]
    GameObject attackBox;

    private void Start()
    {
        attackBox = transform.parent.Find("attackBox").gameObject;
        disableAttackBox();
    }

    public void enableAttackBox()
    {
        Debug.Log("enableAttackBox");
        attackBox.SetActive(true);
    }
    public void disableAttackBox()
    {
        Debug.Log("disableAttackBox");
        attackBox.SetActive(false);
    }


}
