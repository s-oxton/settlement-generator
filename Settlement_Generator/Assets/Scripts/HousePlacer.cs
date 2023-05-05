using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePlacer : MonoBehaviour
{

    class House
    {
        //the house object
        private GameObject house;

        //Initialiser
        public House(GameObject thisHouse)
        {
            house = thisHouse;
        }

        public GameObject GetHouse()
        {
            return house;
        }

        public Vector3 GetPosition()
        {
            Vector3 location;

            location = house.transform.position;

            location = new Vector3(location.x, 0, location.z);

            return location;
        }

        public void SetPosition(Vector3 newPosition)
        {
            house.transform.position = newPosition;
        }

        public Collider GetCollider()
        {
            return house.GetComponent<Collider>();
        }

        public void MoveHouse(Vector3 movement)
        {
            house.transform.position = house.transform.position + movement;
        }

    }

    [Header("GameObjects")]
    [SerializeField]
    private GameObject housePrefab;

    [SerializeField]
    private GameObject Road;

    [Header("Village Variables")]
    [SerializeField]
    [Range(2, 600)]
    private int noOfHouses = 10;

    [SerializeField]
    [Range(1, 100)]
    private int percentHousesOnRoad = 15;

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

    private void SpaceHouses()
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
                    //if the house is too close to the road, move it to a set distance away.
                    if (comparisonHouse.GetPosition().z < Road.transform.position.z + 1 && comparisonHouse.GetPosition().z > Road.transform.position.z - 1)
                    {

                        //calculate how far the house needs to move to be the correct distance away from the road
                        float zOffset = (1 + houseSeparation) - Mathf.Abs((Road.transform.position.z - comparisonHouse.GetPosition().z));

                        //flip the offset if necessary
                        if (comparisonHouse.GetPosition().z - Road.transform.position.z < 0)
                        {
                            zOffset *= -1;
                        }


                        //move the house to the new position
                        comparisonHouse.MoveHouse(Vector3.forward * zOffset);
                    }
                }

            }
        }
    }

    private void CreateHouse(bool houseType, int i, Vector3 position)
    {
        GameObject house;
        house = Instantiate(housePrefab, position, Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), gameObject.transform);
        //if housetype is true, it is a regular house
        house.transform.localScale = CreateRandomScale(houseType);
        //bump house position up to account for random scale (so it doesnt go through the floor)
        house.transform.position += CalculateOffset(house);
        houseArray[i] = new House(house);
    }

    private Vector3 CreateRoadLocation()
    {
        //set the house to a random position along the middle of the road
        Vector3 position = new Vector3(Random.Range(-areaRange, areaRange), 0, Road.transform.position.z);

        //push the house to a random side of the road.
        if (Random.Range(0, 2) == 0)
        {
            position += Vector3.forward * (1 + houseSeparation);
        }
        else
        {
            position += Vector3.back * (1 + houseSeparation);
        }

        return position;
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
