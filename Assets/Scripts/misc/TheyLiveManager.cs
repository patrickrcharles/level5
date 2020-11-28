using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TheyLiveManager : MonoBehaviour
{
    Sprite currentSprite;
    //SpriteRenderer spriteRenderer;
    [SerializeField]
    bool theyLiveEnabled;
    bool on = false;

    GameObject[] billboardList;

    public List<Sprite> theyLiveSpriteList;

    string path = "billboards";

    public static TheyLiveManager instance;

    void Awake()
    {
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //currentSprite = spriteRenderer.sprite;
        instance = this;

        billboardList = GameObject.FindGameObjectsWithTag("billboard");

        Sprite[] temp = Resources.LoadAll<Sprite>(path) as Sprite[];
        foreach (Sprite sprite in temp)
        {
            if (sprite.name.ToLower().Contains("theylive"))
            {
                theyLiveSpriteList.Add(sprite);
            }
        }
    }

    private void Update()
    {
        if (theyLiveEnabled && !on)
        {
            on = true;
            LoadBillboards();
        }
        if (!theyLiveEnabled)
        {
            on = false;
        }
    }

    public void LoadBillboards()
    {

        foreach (GameObject billboard in billboardList)
        {
            int randomIndex = Random.Range(0, (theyLiveSpriteList.Count - 1));

            SpriteRenderer spriteRenderer = billboard.GetComponentInChildren<SpriteRenderer>();
            Sprite currentSprite = spriteRenderer.sprite;

            if (currentSprite.name.ToLower().Contains("fbipi"))
            {
                spriteRenderer.sprite = theyLiveSpriteList.Where(x => x.name.ToLower().Contains("watchtv") == true).Single();
            }
            else
            {
                spriteRenderer.sprite = theyLiveSpriteList[randomIndex];
            }
        }
    }

    public bool TheyLiveEnabled { get => theyLiveEnabled; set => theyLiveEnabled = value; }
    public bool On { get => on; set => on = value; }

}
