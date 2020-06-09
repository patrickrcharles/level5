using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{

    //[SerializeField]
    //GameObject[] _vehicleTargetPositions;

    //[SerializeField]
    //GameObject[] _vehicleSpawnPositions;

    [SerializeField]
    List<VehicleController> _vehiclesList;

    [SerializeField]
    List<GameObject> _vehiclesPrefabsList;

    const string vehicleTargetsTag = "vehicle_position_marker";
    const string spawnPointsTag = "vehicle_spawn_target";

    const string leftSpawnText1 = "leftSpawn1";
    const string leftSpawnText2 = "leftSpawn2";
    const string rightSpawnText1 = "rightSpawn1";
    const string rightpawnText2 = "rightSpawn1";

    const string eastBoundLeftText = "eastBoundLeft";
    const string eastBoundRightText = "eastBoundRight";
    const string westBoundLeftText = "westBoundLeft";
    const string westBoundRightText = "westBoundRight";

    [SerializeField]
    float spawnTime;

    public static TrafficManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        //_vehicleTargetPositions = GameObject.FindGameObjectsWithTag(vehicleTargetsTag);
        //_vehicleSpawnPositions = GameObject.FindGameObjectsWithTag(spawnPointsTag);

        string path = "Prefabs/vehicle";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            VehicleController temp = obj.GetComponent<VehicleController>();
            VehiclesList.Add(temp);
            VehiclesPrefabsList.Add(obj);

            // this where to set direction and target
        }
        // sort list by  mode id
        VehiclesList.Sort(sortByVehicleId);
        // set vehicles directions and targets
        setVehicleDirection(VehiclesList);

        //instantiate vehicle at first postion
        //int i = 0;
        //foreach (GameObject obj in objects)
        //{
        //    if()
        //    Instantiate(obj, _vehicleSpawnPositions[i].transform.position, Quaternion.identity);
        //    i++;
        //}
    }

    void setVehicleDirection(List<VehicleController> vehicles)
    {
        int i = 0;
        foreach (VehicleController v in vehicles)
        {
            // if index is even
            if (i % 2 == 0)
            {
                //direction to move vehicle towards
                v.Direction = "right";
                // set target to correct vector3
                v.CurrentTarget = GameObject.Find(eastBoundRightText).transform.position;
            }
            else
            {
                //direction to move vehicle towards
                v.Direction = "left";
                // set target to vector3
                v.CurrentTarget = GameObject.Find(westBoundLeftText).transform.position;
            }
            i++;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    static int sortByVehicleId(VehicleController m1, VehicleController m2)
    {
        return m2.VehicleId.CompareTo(m2.VehicleId);
    }

    public List<VehicleController> VehiclesList { get => _vehiclesList; }
    //public GameObject[] VehicleTargetPositions { get => _vehicleTargetPositions; }
    public List<GameObject> VehiclesPrefabsList { get => _vehiclesPrefabsList; }
}
