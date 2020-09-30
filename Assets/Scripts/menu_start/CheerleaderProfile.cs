
using UnityEngine;

public class CheerleaderProfile : MonoBehaviour
{

    [SerializeField] private string cheerleaderDisplayName;
    [SerializeField] private string cheerleaderObjectName;
    [SerializeField] private Sprite cheerleaderPortrait;
    [SerializeField] private GameObject cheerleaderProfileObject;
    [SerializeField] private bool isLocked;
    [SerializeField] private int cheerleaderId;
    [SerializeField] private string unlockCharacterText;

    public int CheerleaderId { get => cheerleaderId; set => cheerleaderId = value; }
    public string CheerleaderDisplayName { get => cheerleaderDisplayName; set => cheerleaderDisplayName = value; }
    public string CheerleaderObjectName { get => cheerleaderObjectName; set => cheerleaderObjectName = value; }
    public bool IsLocked { get => isLocked; set => isLocked = value; }
    public string UnlockCharacterText { get => unlockCharacterText; set => unlockCharacterText = value; }
    public Sprite CheerleaderPortrait { get => cheerleaderPortrait; set => cheerleaderPortrait = value; }
}
