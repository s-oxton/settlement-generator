using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePlacer : MonoBehaviour
{

    class House
    {
        //the house object
        private GameObject house;
        //how close this house can be to other houses
        private float houseSeparation;
        //list of nearby houses
        private List<House> nearbyHouses = new List<House>();

        //Initialiser
        public House(GameObject thisHouse, float separation)
        {
            house = thisHouse;
            houseSeparation = separation;
        }

        public Vector3 GetPosition()
        {
            Vector3 location;

            location = house.transform.position;

            return location;
        }

        public float GetSeparation()
        {
            return houseSeparation;
        }

        public void MoveHouse(Vector3 movement)
        {
            Debug.DrawRay(house.transform.position, movement.normalized, Color.green, Time.deltaTime);
            house.transform.position = house.transform.position + movement;
        }

        public void ClearNearbyHouses()
        {
            nearbyHouses.Clear();
        }

        public void AddNearbyHouse(House nearbyHouse)
        {
            nearbyHouses.Add(nearbyHouse);
        }

        public void DebugHouseList()
        {
            Debug.Log("Number of nearby houses: " + nearbyHouses.Count);
        }

    }

    [SerializeField]
    private GameObject housePrefab;

    [SerializeField]
    [Range(2, 60)]
    private int noOfHouses = 10;

    [SerializeField]
    [Range(0.1f, 10)]
    private float houseSeparation = 1f;

    [SerializeField]
    private float areaRange = 5;

    [SerializeField]
    private float yOffset = 0.25f;

    [SerializeField]
    private float houseMoveDistance = 0.1f;

    private House[] houseArray;

    // Start is called before the first frame update
    void Start()
    {

        houseArray = new House[noOfHouses];

        //place houses
        for (int i = 0; i < noOfHouses; i++)
        {
            GameObject house;
            house = Instantiate(housePrefab, CreateRandomLocation(), Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0)), gameObject.transform);
            //add created house to array of all houses
            houseArray[i] = new House(house, houseSeparation);
        }

    }

    private void Update()
    {



        //detect nearby houses
        //for every house in the array
        foreach (House house in houseArray)
        {
            //clear the list of nearby houses
            //house.ClearNearbyHouses();

            //loop through all the other houses
            foreach (House comparisonHouse in houseArray)
            {
                if (house != comparisonHouse)
                {
                    //find the distance between the houses
                    float distance = Vector3.Distance(comparisonHouse.GetPosition(), house.GetPosition());
                    //if the houses are too close, add comparison house to first house's list of houses that are too close
                    if (distance < house.GetSeparation())
                    {
                        //house.AddNearbyHouse(comparisonHouse);
                        //get direction between the two houses
                        Vector3 direction = -(house.GetPosition() - comparisonHouse.GetPosition()).normalized;
                        //Debug.DrawRay(comparisonHouse.GetPosition(), direction, Color.green, Time.deltaTime);
                        //move the other house 
                        comparisonHouse.MoveHouse(direction * houseMoveDistance);
                    }
                }
            }

            //house.DebugHouseList();

        }

    }

    private Vector3 CreateRandomLocation()
    {
        //initialise location
        float x = Random.Range(-areaRange, areaRange);
        float z = Random.Range(-areaRange, areaRange);

        Vector3 location = new Vector3(x, yOffset, z);
        return location;
    }



}
