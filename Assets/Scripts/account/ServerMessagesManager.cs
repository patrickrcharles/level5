using Assets.Scripts.Models;
using Assets.Scripts.restapi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerMessagesManager : MonoBehaviour
{
    [SerializeField]
    List<Text> serverMessagesText;
    [SerializeField]
    List<ServerMessageModel> serverMessagesModels;
    // Start is called before the first frame update
    void Start()
    {
        serverMessagesModels = APIHelper.GetServerMessages();
        setUIMessages();
    }

    private void setUIMessages()
    {
        for (int i = 0; i < serverMessagesModels.Count; i++)
        {
            serverMessagesText[i].text = serverMessagesModels[i].Date +"\n" + serverMessagesModels[i].Message;
        }
    }
}
