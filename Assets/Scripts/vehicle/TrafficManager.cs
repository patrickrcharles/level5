using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{

    [SerializeField]
    GameObject[] _vehicleTargetPositions;

    [SerializeField]
    GameObject _vehicleSpawnLeftPosition;
    [SerializeField]
    GameObject _vehicleSpawnRightPosition;

    [SerializeField]
    List<VehicleController> _vehiclesList;

    [SerializeField]
    List<GameObject> _vehiclesPrefabsList;

    const string vehicleTargetsTag = "vehicle_position_marker";
    const string spawnLeftTag = "vehicle_spawn_left";
    const string spawnRightTag = "vehicle_spawn_right";

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
        // get spawn points
        _vehicleSpawnLeftPosition = GameObject.FindGameObjectWithTag(spawnLeftTag);
        _vehicleSpawnRightPosition = GameObject.FindGameObjectWithTag(spawnRightTag);
        loadVehiclePrefabs();
    }

    private void loadVehiclePrefabs()
    {
        // where are the prefabs, load them
        string path = "Prefabs/vehicle";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        // get all the objects in folder, create list of the vehicleControllers
        foreach (GameObject obj in objects)
        {
            VehicleController temp = obj.GetComponent<VehicleController>();
            VehiclesList.Add(temp);
            VehiclesPrefabsList.Add(obj);

            // this where to set direction and target
        }
        //spawn vehicles
        spawnVehiclePrefabs();
    }

    public void spawnVehicle(int vehicleId, string direction, float waitTimeToRespawn)
    {
        // find object in list by prefab
        VehicleController vehiclePrefab = VehiclesList.Where(x => x.VehicleId == vehicleId).SingleOrDefault();

        // call coroutine
        StartCoroutine(spawnVehicleCoRoutine(vehiclePrefab, direction, waitTimeToRespawn));
    }

    private IEnumerator spawnVehicleCoRoutine(VehicleController vehicle, string direction, float waitTimeToRespawn)
    {
        // change to opposite direction
        if(direction == "left")
        {
            vehicle.Direction = "right";
            vehicle.CurrentTarget = GameObject.Find(eastBoundRightText).transform.position;
            yield return new WaitForSeconds(waitTimeToRespawn);
            Instantiate(vehicle, _vehicleSpawnLeftPosition.transform.position, Quaternion.identity);
        }
        if (direction == "right")
        {
            vehicle.Direction = "left";
            vehicle.CurrentTarget = GameObject.Find(westBoundLeftText).transform.position;
            ////flip sprite
            //vehicle.Flip();
            yield return new WaitForSeconds(waitTimeToRespawn);
            Instantiate(vehicle, _vehicleSpawnRightPosition.transform.position, Quaternion.identity);
        }
    }

    private void spawnVehiclePrefabs()
    {
        // sort list by  mode id
        VehiclesList.Sort(sortByVehicleId);

        //instantiate vehicle at first postion
        int vehicleIndex = 0;

        // to prevent vehicles spawning on top of each other
        Vector3 VectorToAddToSpawn = new Vector3();

        // ************* need to be spawning from prefabs list. this is saving and changing prefabs value
        foreach (VehicleController v in VehiclesList)
        {
            if (v.VehicleId % 2 == 0)
            {
                //direction to move vehicle towards
                v.Direction = "right";
                v.FacingRight = true;
                // set target to correct vector3
                v.CurrentTarget = GameObject.Find(eastBoundRightText).transform.position;
                VectorToAddToSpawn += new Vector3((-5 * vehicleIndex), 0, 0);
                Instantiate(v, _vehicleSpawnLeftPosition.transform.position + VectorToAddToSpawn, Quaternion.identity);
            }
            else
            {
                //direction to move vehicle towards
                v.Direction = "left";
                v.FacingRight = false;
                // set target to vector3
                v.CurrentTarget = GameObject.Find(westBoundLeftText).transform.position;
                VectorToAddToSpawn += new Vector3((5 * vehicleIndex), 0, 0);
                Instantiate(v, _vehicleSpawnRightPosition.transform.position + VectorToAddToSpawn, Quaternion.identity);
            }
            vehicleIndex++;
        }
    }

    static int sortByVehicleId(VehicleController m1, VehicleController m2)
    {
        return m1.VehicleId.CompareTo(m2.VehicleId);
    }

    public List<VehicleController> VehiclesList { get => _vehiclesList; }

    public List<GameObject> VehiclesPrefabsList { get => _vehiclesPrefabsList; }
}
