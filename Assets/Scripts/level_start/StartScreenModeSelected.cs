using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenModeSelected : MonoBehaviour
{

    [SerializeField] private int modeId;
    [SerializeField] private string modelDisplayName;
    [SerializeField] private string modeObjectName;
    [SerializeField] private string modeDescription;

    public int ModeId
    {
        get => modeId;
        set => modeId = value;
    }

    public string ModelDisplayName
    {
        get => modelDisplayName;
        set => modelDisplayName = value;
    }

    public string ModeObjectName
    {
        get => modeObjectName;
        set => modeObjectName = value;
    }

    public string ModeDescription
    {
        get => modeDescription;
        set => modeDescription = value;
    }
}
