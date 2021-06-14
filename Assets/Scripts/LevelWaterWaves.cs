using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = System.Random;

public class LevelWaterWaves : MonoBehaviour
{
    [SerializeField]
    float maxHeight;
    [SerializeField]
    float minHeight;

    Direction direction;

    internal enum Direction
    {
        UP = 0,
        DOWN = 1
    }

    private void Start()
    {
        maxHeight = transform.position.y + 0.05f;
        minHeight = maxHeight - 0.4f;
        direction = Direction.DOWN;
        InvokeRepeating("Waves", 0, 0.5f);
    }

    private void Waves()
    {
        float height = gameObject.transform.position.y;

        if(direction == Direction.DOWN)
        {
            transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - 0.01f, gameObject.transform.position.z);
            if(height < minHeight)
            {
                direction = Direction.UP;
            }
        }
        if (direction == Direction.UP)
        {
            transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 0.01f, gameObject.transform.position.z);
            if (height > maxHeight)
            {
                direction = Direction.DOWN;
            }
        }
    }
}
