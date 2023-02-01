using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class moveRandomly : MonoBehaviour
{
    private FighterController fighterController;
    private FighterPath fighterPath;

    // The maximum distance that the game object can move towards a random waypoint
    public float maxDistance = 5000f;
    public float minDistance = 1000f;

    // The minimum time interval between adding new random waypoints
    public float minTimeInterval = 8f;

    // The maximum time interval between adding new random waypoints
    public float maxTimeInterval = 9f;

    private List<Vector3> armyCamps;

    private bool addArmyCampPositions = false;

    Vector3 previousTarget = Vector3.zero;

    void Start()
    {
        armyCamps = new List<Vector3>();
        // Get references to the FighterController and FighterPath components
        fighterController = GetComponent<FighterController>();
        fighterPath = GetComponent<FighterPath>();

        // Start adding random waypoints at random intervals
        StartCoroutine(AddRandomWaypoints());

    }

    IEnumerator AddRandomWaypoints()
    {
        while (true)
        {
            // Wait for a random amount of time before adding a new waypoint
            yield return new WaitForSeconds(Random.Range(minTimeInterval, maxTimeInterval));



            if (!addArmyCampPositions)
            {
                GameObject[] structures = GameObject.FindGameObjectsWithTag("Structure");
                foreach (GameObject structure in structures)
                {
                    StructureSelector _sSelector = structure.GetComponent<StructureSelector>();
                    if (_sSelector.structureType.Equals("Army Camp") || _sSelector.structureType.Equals("Mega Army Camp"))
                    {
                        Vector3 armyCampPosition = structure.transform.position;
                      
                        armyCamps.Add(armyCampPosition);
                    }
                }
                addArmyCampPositions = true;
            }


            // Choose a random direction and distance from the game object's current position
            Vector3 direction = Random.insideUnitCircle.normalized;
            float distance = Random.Range(minDistance, maxDistance);

            // Calculate the position of the new waypoint
            Vector3 newWaypoint = transform.position + direction * distance;
            armyCamps.Add(newWaypoint);

            Vector3 selectedTarget = SelectTarget(armyCamps, previousTarget);
            previousTarget = selectedTarget;
            ArrayList waypoints = FindNextFree(selectedTarget);
            armyCamps.RemoveAt(armyCamps.Count - 1);
            // Add the new waypoint to the path
            fighterPath.waypoints.Clear();

            for (int i=0; i<waypoints.Count; i++)
            {
                Node wapPnode = (Node)waypoints[i];
                fighterPath.waypoints.Add(wapPnode.position);
            }
            
           
            // Update the path to reflect the new waypoint
            fighterController.lb_UpdatePathForAutoMovement();

        }
    }

    private Vector3 SelectTarget(List<Vector3> armyCamps, Vector3 previousTarget)
    {
        // Choose a random index in the list
        int index = Random.Range(0, armyCamps.Count);

        // Get the Vector3 at the chosen index
        Vector3 target = armyCamps[index];

        // If the target is the same as the previous target, choose a new target
        while (target == previousTarget)
        {
            index = Random.Range(0, armyCamps.Count);
            target = armyCamps[index];
        }

        return target;
    }

    private ArrayList FindNextFree(Vector3 targetPoint)//since the corners are free, but might be inaccessible 
    {

        Vector3 endPos = targetPoint;// of aiTargets
                                            //Assign StartNode and Goal Node
        Node startNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(this.gameObject.transform.position)));
        Node goalNode = new Node(GridManager.instance.GetGridCellCenter(GridManager.instance.GetGridIndex(endPos)));

        ArrayList pathArray = AStar.FindPath(startNode, goalNode);

        return pathArray;

    }
}