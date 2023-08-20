using UnityEngine;

public class PlayerAttackPosition : MonoBehaviour
{
    public bool engaged;
    public GameObject enemyEngaged;
    public int attackPositionId;
    public Vector3 position;
    PlayerIdentifier playerIdentifier;

    private void Start()
    {
        playerIdentifier = GetComponentInParent<PlayerIdentifier>();
        InvokeRepeating("updateAttackPositionTransform", 0, 1);
    }

    void updateAttackPositionTransform()
    {
        if (enemyEngaged == null)
        {
            engaged = false;
        }
        Vector3 playerTransform = playerIdentifier.isCpu ? playerIdentifier.autoPlayer.transform.position : playerIdentifier.player.transform.position;
        if (attackPositionId == 1)
        {
            transform.position = new Vector3(playerTransform.x - 0.6f, playerTransform.y, playerTransform.z - 0.25f);
        }
        if (attackPositionId == 2)
        {
            transform.position = new Vector3(playerTransform.x - 0.6f, playerTransform.y, playerTransform.z + 0.25f);
        }
        if (attackPositionId == 3)
        {
            transform.position = new Vector3(playerTransform.x + 0.6f, playerTransform.y, playerTransform.z - 0.25f);
        }
        if (attackPositionId == 4)
        {
            transform.position = new Vector3(playerTransform.x + 0.6f, playerTransform.y, playerTransform.z + 0.25f);
        }
        position = transform.position;
    }
}
