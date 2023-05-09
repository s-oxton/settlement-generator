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
    [Range(0.1f, 5)]
    private float houseSeparation = 0.3f;

    [SerializeField]
    [Range(0.1f, 10f)]
    private float roadSpacing = 1f;

    [Header("Algorithm Variables")]
    [SerializeField]
    [Range(0.1f, 2f)]
    private float houseMoveDistance = 0.1f;

    [SerializeField]
    [Range(0, 500)]
    private int spacingIterations = 25;

    private House[] houseArray;

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
        Collider[] tempColliders = Physics.OverlapBox(position, new Vector3(0.9f, 0.4f, 0.4f), rotation);

        if (tempColliders.Length == 0)
        {
            spaceEmpty = true;
            GameObject bounds = Instantiate(boundsChecker, position, rotation, this.transform);
        }

        //Destroy(bounds);
        return spaceEmpty;
    }

    public void PlaceHouses(Vector3[] roadBounds)
    {
        houseArray = new House[noOfHouses];

        for (int i = 0; i < noOfHouses; i++)
        {
            //place a house
            Vector3 position = CreateRandomLocation(roadBounds);
            int prefabNumber = Random.Range(0, housePrefabs.Length);
            GameObject house = Instantiate(housePrefabs[prefabNumber], position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), gameObject.transform);
            houseArray[i] = new House(house);
        }
    }

    public void SpaceHouses()
    {

        //spread out position of houses
        //counter used to stop loop if it is running too many times
        for (int i = 0; i < spacingIterations; i++)
        {
            int overlapCount = 0;
            foreach (House house in houseArray)
            {

                //if house too far away from road, move towards centre
                Collider[] colliders = Physics.OverlapSphere(house.GetPosition(), roadSpacing);

                bool moveToRoad = true;
                foreach (Collider collider in colliders)
                {
                    //if tag is road, movetoroad = false
                    if (collider.gameObject.tag.Equals("Road"))
                    {
                        moveToRoad = false;
                        break;
                    }
                }

                if (moveToRoad)
                {
                    //movetowards 0,0,0
                    Vector3 direction = (Vector3.zero - house.GetPosition()).normalized;
                    //house.SetPosition(house.GetPosition() + direction * (houseMoveDistance / 2));
                }

                //get all the nearby objects 
                colliders = Physics.OverlapBox(house.GetPosition(), (Vector3.one * houseSeparation) + (house.GetCollider().bounds.size), house.GetRotation());

                foreach (Collider collider in colliders)
                {
                    //if the collider isn't the collider of the current house
                    if (collider != house.GetCollider())
                    {
                        //find the closest points between the house and the nearby object
                        Vector3 closestPoint = collider.ClosestPoint(house.GetPosition());
                        Vector3 currentHouseClosestPoint = house.GetCollider().ClosestPoint(closestPoint);

                        float distance = Vector3.Distance(closestPoint, currentHouseClosestPoint);

                        //if these points are the same the house is overlapping
                        if (closestPoint == currentHouseClosestPoint)
                        {
                            overlapCount++;
                        }
                        //if distance too small, object needs to be moved
                        if (distance < houseSeparation || closestPoint == currentHouseClosestPoint)
                        {

                            Vector3 otherPosition = new Vector3();

                            //if object is a road, need to get the Centrepoint from the prefab
                            if (collider.gameObject.tag.Equals("Road"))
                            {
                                otherPosition = collider.gameObject.transform.parent.gameObject.transform.Find("CentrePoint").transform.position;
                            }
                            else
                            {
                                otherPosition = collider.gameObject.transform.position;
                            }

                            //get direction from house to other position
                            Vector3 direction = (otherPosition - house.GetPosition()).normalized;

                            //if other object is immovable (i.e. a road), move current house AWAY from object
                            if (collider.gameObject.tag.Equals("Immovable") || collider.gameObject.tag.Equals("Road"))
                            {
                                house.SetPosition(house.GetPosition() + -direction * houseMoveDistance);
                            }
                            // else move other object away from house
                            else
                            {
                                collider.gameObject.transform.position = otherPosition + direction * houseMoveDistance;
                            }
                        }
                    }
                }
            }

            Debug.Log("Iteration "+i+". Overlap count " + overlapCount);

        }
    }

    private float GetMaxElement(Vector3 bounds)
    {
        float maxValue = 0f;

        maxValue = Mathf.Max(bounds.x, bounds.y, bounds.z);

        return maxValue;
    }

    private Vector3 CreateRandomLocation(Vector3[] roadBounds)
    {
        //initialise location
        float x = Random.Range(roadBounds[0].x, roadBounds[1].x);
        float z = Random.Range(roadBounds[0].z, roadBounds[1].z);

        return new Vector3(x, 0, z);
    }

}
