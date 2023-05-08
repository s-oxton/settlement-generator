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
    [Range(2, 200)]
    private int noOfHouses = 10;

    [SerializeField]
    [Range(0.1f, 1)]
    private float houseSeparation = 0.3f;

    [SerializeField]
    private float areaRange = 5;

    [Header("Algorithm Variables")]
    [SerializeField]
    private float houseMoveDistance = 0.1f;

    [SerializeField]
    [Range(0, 100)]
    private int spacingIterations = 25;

    private House[] houseArray;

    // Start is called before the first frame update
    /*void Start()
    {

        houseArray = new House[noOfHouses];
        int noOfTaverns = Random.Range(0, 2);
        if (noOfHouses >= 100)
        {
            noOfTaverns = noOfTaverns + Mathf.FloorToInt(noOfHouses / 200);
        }

        //make a percentage of the houses stick to the road
        int noOfRoadHouses = Mathf.FloorToInt(noOfHouses * (percentHousesOnRoad/100f));

        Vector3 housePosition;

        //randomly place houses
        for (int i = 0; i < noOfHouses; i++)
        {
            //place the first amount of houses on the road
            if (i < noOfRoadHouses)
            {
                housePosition = CreateRoadLocation();
            }
            else
            {
                housePosition = CreateRandomLocation();
            }

        }

        SpaceHouses();

    }*/


    public void PlaceTaverns(List<RoadDetails> roadList)
    {

        List<TurtleTransform> validTransforms = new List<TurtleTransform>();

        //generate number of taverns
        int roadCount = roadList.Count;
        int tavernCount = 0;
        if (roadCount > 0 && roadCount <= 20)
        {
            tavernCount = 1;
        }
        else if (roadCount > 20 && roadCount <= 80)
        {
            tavernCount = 2;
        }
        else if (roadCount > 80 && roadCount <= 100)
        {
            tavernCount = 3;
        }
        else if (roadCount > 100)
        {
            tavernCount = 4;
        }


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
                if (CheckIfLocationEmpty(turtle.transform.position, turtle.transform.rotation))
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

        Debug.Log("Valid Tavern Positions:" + validTransforms.Count);

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

    //returns true if the location is empty
    private bool CheckIfLocationEmpty(Vector3 position, Quaternion rotation)
    {
        bool spaceEmpty = false;

        //this doesn't really fully work but it does the job
        Collider[] tempColliders = Physics.OverlapBox(position, new Vector3 (0.9f, 0.4f, 0.4f), rotation);

        if (tempColliders.Length == 0)
        {
            spaceEmpty = true;
            GameObject bounds = Instantiate(boundsChecker, position, rotation, this.transform);
            bounds.name = "Valid Position";
        }
        else
        {
            GameObject bounds = Instantiate(boundsChecker, position, rotation, this.transform);
            bounds.name = "Invalid Position";

            foreach (Collider collider in tempColliders)
            {
                Debug.Log(collider.gameObject.name);
            }

        }

        //Destroy(bounds);
        return spaceEmpty;
    }

    public void SpaceHouses()
    {
        //spread out position of houses
        //counter used to stop loop if it is running too many times
        for (int i = 0; i < spacingIterations; i++)
        {

            foreach (House house in houseArray)
            {
                //loop through all the other houses
                foreach (House comparisonHouse in houseArray)
                {
                    if (house != comparisonHouse)
                    {
                        //only do intensive collider checking if houses are close enough for it to matter.
                        if (Vector3.Distance(house.GetPosition(), comparisonHouse.GetPosition()) < 2f)
                        {
                            //find the distance between the houses
                            //get the closest points on each house's collider
                            Vector3 comparisonHouseClosestPoint = comparisonHouse.GetCollider().ClosestPoint(house.GetPosition());
                            Vector3 currentHouseClosestPoint = house.GetCollider().ClosestPoint(comparisonHouseClosestPoint);

                            float distance = Vector3.Distance(comparisonHouseClosestPoint, currentHouseClosestPoint);

                            //if the houses are too close, move the comparison house
                            if (distance < houseSeparation)
                            {
                                //get direction between the two houses
                                Vector3 direction = -(house.GetPosition() - comparisonHouse.GetPosition()).normalized;
                                //Debug.DrawRay(comparisonHouse.GetPosition(), direction, Color.green, Time.deltaTime);
                                //move the other house 
                                direction = new Vector3(direction.x, 0f, direction.z);
                                comparisonHouse.MoveHouse(direction * houseMoveDistance);
                            }
                        }
                    }

                }

            }
        }
    }

    private void CreateHouse(bool houseType, int i, Vector3 position)
    {
        GameObject house;
        house = Instantiate(housePrefabs[0], position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), gameObject.transform);
        //if housetype is true, it is a regular house
        house.transform.localScale = CreateRandomScale(houseType);
        //bump house position up to account for random scale (so it doesnt go through the floor)
        house.transform.position += CalculateOffset(house);
        houseArray[i] = new House(house);
    }



    private Vector3 CreateRandomLocation()
    {
        float areaScaling = 1;
        //change the range a house can be placed in
        float scaledArea = areaRange / areaScaling;
        //initialise location
        float x = Random.Range(-scaledArea, scaledArea);
        float z = Random.Range(-scaledArea, scaledArea);

        Vector3 location = new Vector3(x, 0, z);
        return location;
    }

    private Vector3 CreateRandomScale(bool houseType)
    {
        float xScale = 0.5f, yScale = 0.5f, zScale = 0.5f;
        //if scaling a house.
        if (houseType)
        {
            xScale = Random.Range(0.4f, 0.75f);
            zScale = Random.Range(0.5f, 0.75f);
        }
        //if scaling a tavern.
        else
        {
            xScale = Random.Range(1f, 3f);
            //adds 1 or 2 floors to the tavern.
            yScale *= Random.Range(2, 4);
            zScale = Random.Range(1f, 3f);
        }

        Vector3 scale = new Vector3(xScale, yScale, zScale);
        return scale;
    }

    private Vector3 CalculateOffset(GameObject house)
    {
        //half house starts in the floor, so needs to be moved up by half of the scale.
        float scale = house.transform.localScale.y;
        float offset = scale / 2;
        return Vector3.up * offset;
    }

}
