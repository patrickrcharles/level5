using System;
using UnityEngine;
using Random = System.Random;

public class PickupObject : MonoBehaviour
{
    [SerializeField] private string Objectname;
    [SerializeField] float moneyValue;
    [SerializeField] private float moneyDestroyTime;

    //[SerializeField]
    //int moneyType;
    //[SerializeField] private int shotType;

    void Awake()
    {
        // destroy game object
        Destroy(gameObject, moneyDestroyTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("money"))
        {
            // update player money value
            PlayerStats.instance.Money += moneyValue;
            Destroy(gameObject);
        }
    }

    public void updateMoneyValue(float value)
    {
        moneyValue = value;// + generateRandomCents();
        //Debug.Log("money value : " + moneyValue);
    }

    float generateRandomCents()
    {
        var random = new Random();
        decimal cents = random.Next(1, 100);

        //Debug.Log("rand change: "+ (float)(Math.Round(cents, 2)) / 100);
        return (float)(Math.Round(cents, 2)) / 100;
    }

    public string Name
    {
        get => Objectname;
        set => Objectname = value;
    }

    //public int MoneyType
    //{
    //    get => moneyType;
    //    set => moneyType = value;
    //}

    public float MoneyValue
    {
        get => moneyValue;
        set => moneyValue = value;
    }

}
