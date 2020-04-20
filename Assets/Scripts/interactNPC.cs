using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class interactNPC : MonoBehaviour
{
    Text text;
    public List<string> phrases = new List<string>();
    Random random;
    int ranNum;
    public GameObject textObject;
    //GameObject rootGameObject;
    Animator anim;


        // Use this for initialization
        void Start()
        {

        anim = GetComponentInChildren<Animator>();
        //text = textObject.GetComponent<Text>();
        //rootGameObject = transform.root.gameObject;

        }

        // Update is called once per frame
        void Update()
        {

        }

        void Awake()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (gameObject.tag == "flasher" && other.tag == "playerHitbox")
            {
                anim.Play("flash");
            }

        }

        private void OnTriggerExit(Collider other)
        {

        //text.text = null;

    }
    }
