using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePlacer : MonoBehaviour
{

    [Header("GameObjects")]
    [SerializeField]
    private GameObject[] housePrefabs;

    [SerializeField]
    private GameObject tavern;

    [SerializeField]
    private GameObject turtle;

    [SerializeField]
    private GameObject boundsChecker;

    [Header("Village Variables")]

    [SerializeField]
    [Range(0f, 5)]
    private float houseSeparation = 0.3f;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float roadSpacing = 1f;

    [Header("Algorithm Variables")]
    [SerializeField]
    [Range(0.1f, 2f)]
    private float houseMoveDistance = 0.1f;

    [SerializeField]
    [Range(0, 5000)]
    private int spacingIterations = 25;

    [SerializeField]
    [Range(0f, 1f)]
    private float locationScaling = 1f;

    private List<House> houseList = new List<House>();

    public void PlaceTaverns(List<RoadDetails> roadList)
    {

        List<TurtleTransform> validTransforms = new List<TurtleTransform>();

        //generate number of taverns
        int tavernCount = NumberOfTaverns(roadList.Count);

        //calculate the distance the tavern should be placed from the road.
        float distanceFromRoad = houseSeparation + (tavern.transform.localScale.z * (tavern.GetComponent<BoxCollider>().size.z / 2));

        //for each road in roadlist, need to check if there is a space on either side of road
        foreach (RoadDetails road in roadList)
        {

            //get location where cube needs to be placed.
            Vector3 perpVector = Vector3.Cross(road.GetDirection(), Vector3.up).normalized;

            for (int i = 0; i < 2; i++)
            {
                //flip perp Vector to get both sides
                perpVector = -perpVector;

                //set transform to side of road
                turtle.transform.position = road.GetCentrepoint() + ((road.GetRoadWidth() / 2) + distanceFromRoad) * perpVector;
                //set rotation to be perpendicular to road
                turtle.transform.LookAt(road.GetCentrepoint());

                //if there is space, add position to list, and add rotation so tavern faces towards road. use turtletransform
                if (CheckIfLocationEmpty(boundsChecker.transform.localScale, turtle.transform.position, turtle.transform.rotation))
                {
                    validTransforms.Add(new TurtleTransform(turtle.transform.position, turtle.transform.rotation));
                }

            }

        }

        foreach (Transform child in this.transform)
        {
            if (child.CompareTag("BoundsChecker"))
            {
                Destroy(child.gameObject);
            }
        }

        //randomly pick tavern spots from list based on number of taverns
        //to ensure that all the taverns can be placed.
        if (tavernCount > validTransforms.Count)
        {
            tavernCount = validTransforms.Count;
        }

        //for each tavern
        for (int i = 0; i < tavernCount; i++)
        {
            //pick random object from valid position list
            int random = Random.Range(0, validTransforms.Count);
            //place tavern at that position
            TurtleTransform tempTransform = validTransforms[random];
            Instantiate(tavern, tempTransform.GetPosition(), tempTransform.GetRotation(), this.transform);
            //remove position from list
            validTransforms.RemoveAt(random);

        }

    }

    private int NumberOfTaverns(int roadCount)
    {
        //generate number of taverns
        int tavernCount = 0;
        if (roadCount > 0 && roadCount <= 20)
        {
            tavernCount = 1;
        }
        else if (roadCount > 20 && roadCount <= 80)
        {
            tavernCount = 2;
        }
        else if (roadCount > 80)
        {
            tavernCount = 3;
        }
        return tavernCount;
    }

    //returns true if the location is empty
    private bool CheckIfLocationEmpty(Vector3 scale, Vector3 position, Quaternion rotation)
    {
        bool spaceEmpty = false;

        //this doesn't really fully work but it does the job new Vector3(0.9f, 0.4f, 0.4f)
        Collider[] tempColliders = Physics.OverlapBox(position, (scale + Vector3.one * houseSeparation) / 2, rotation);

        if (tempColliders.Length == 0)
        {
            spaceEmpty = true;
            GameObject bounds = Instantiate(boundsChecker, position, rotation, this.transform);
        }

        //Destroy(bounds);
        return spaceEmpty;
    }

    //starts by placing all the houses under the map
    public void InitialiseHouseList(int noOfHouses)
    {
        for (int i = 0; i < noOfHouses; i++)
        {
            int prefabNumber = Random.Range(0, housePrefabs.Length);
            GameObject house = Instantiate(housePrefabs[prefabNumber], this.transform.position + (Vector3.down*5), this.transform.rotation, gameObject.transform);
            houseList.Add(new House(house));
        }

    }

    public void PlaceHousesRandomly(Vector3[] roadBounds)
    {
        foreach (House house in houseList) { 
             
            house.SetPosition(CreateRandomLocation(roadBounds));

        }
    }

    private bool spaceHouses = false;
    private int iterationsNeeded = 0;
    private bool houseMoved = false;

    public void ActivateSpacingMethod()
    {
        spaceHouses = true;
    }

    private void SpaceHouses()
    {
        houseMoved = false;
        foreach (House house in houseList)
        {

            List<Vector3> closeRoads = new List<Vector3>();

            //get all the nearby objects 
            Collider[] colliders = Physics.OverlapBox(house.GetPosition(), ((Vector3.one * houseSeparation) + house.GetCollider().bounds.size) / 2, house.GetRotation());

            foreach (Collider collider in colliders)
            {

                //if the collider isn't the collider of the current house
                if (collider != house.GetCollider())
                {
                    houseMoved = true;
                    //if the collider doesn't belong to a road, just move the things apart
                    if (!collider.gameObject.tag.Equals("Road"))
                    {
                        //get position of other object
                        Vector3 otherPosition = collider.gameObject.transform.position;
                        //find the direction between the two things
                        Vector3 direction = (house.GetPosition() - otherPosition).normalized;
                        //move house
                        house.SetPosition(house.GetPosition() + direction * houseMoveDistance * Time.fixedDeltaTime);
                        //if other object not immovable, move it as well. This stops houses getting stuck in between multiple houses.
                        if (!collider.gameObject.tag.Equals("Immovable"))
                        {
                            collider.gameObject.transform.position = collider.gameObject.transform.position + -direction * houseMoveDistance * Time.fixedDeltaTime;

                        }
                    }
                    //if the collider belongs to a road, move house away from road.
                    else
                    {
                        //direction between road and house
                        Vector3 directionOfHouse = (house.GetPosition() - collider.gameObject.transform.parent.position).normalized;
                        //direction is -1 for left side, +1 when right side
                        float sideOfRoad = CalculateDirection(collider.gameObject.transform.parent.forward, directionOfHouse, collider.gameObject.transform.parent.up);
                        //update house position. if on right side, side of road = 1 so transform.right is not flipped, otherwise it is flipped to the left side.
                        house.SetPosition(house.GetPosition() + (directionOfHouse + (collider.gameObject.transform.parent.right * sideOfRoad)).normalized * houseMoveDistance * Time.fixedDeltaTime);
                        //add the road to the close roads list.
                        closeRoads.Add(collider.ClosestPoint(house.GetPosition()));
                    }

                }

            }
            //if there is a nearby road, make house face towards road.
            if (closeRoads.Count > 0)
            {
                FixHouseRotation(house, closeRoads);
            }

        }
        if (houseMoved)
        {
            iterationsNeeded++;
            Debug.Log("Iterations: " + iterationsNeeded);
            //break out of loop if houses arent getting fixed
            if (iterationsNeeded > spacingIterations)
            {
                spaceHouses = false;
            }

        }

    }

    private void FixedUpdate()
    {
        if (spaceHouses)
        {
            SpaceHouses();
        }
    }

    //calculate if the house is on the left or right hand side of the road using dot product.
    private float CalculateDirection(Vector3 roadForward, Vector3 directionOfHouse, Vector3 roadUp)
    {
        Vector3 perpendicular = Vector3.Cross(roadForward, directionOfHouse);
        float direction = Vector3.Dot(perpendicular, roadUp);

        if (direction > 0f)
        {
            return 1;
        }
        else if (direction < 0f)
        {
            return -1;
        }
        else
        {
            return 0;
        }

    }

    //make house look at nearest road.
    private void FixHouseRotation(House house, List<Vector3> closeRoads)
    {

        //set the first road to be the closest one
        float smallestDistance = Vector3.Distance(closeRoads[0], house.GetPosition());
        Vector3 closestRoad = closeRoads[0];
        //check to see if any of the roads are closer
        foreach (Vector3 point in closeRoads)
        {
            if (smallestDistance > Vector3.Distance(point, house.GetPosition()))
            {
                closestRoad = point;
            }
        }
        //if the road is close enough, make house look at road
        if (smallestDistance < roadSpacing)
        {
            house.GetHouse().transform.LookAt(closestRoad);
        }

    }

    private Vector3 CreateRandomLocation(Vector3[] roadBounds)
    {
        //initialise location
        float x = Random.Range(roadBounds[0].x, roadBounds[1].x) * locationScaling;
        float z = Random.Range(roadBounds[0].z, roadBounds[1].z) * locationScaling;

        return new Vector3(x, 0, z);
    }

}
