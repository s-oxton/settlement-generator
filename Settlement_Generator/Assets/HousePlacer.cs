using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousePlacer : MonoBehaviour
{

    [SerializeField]
    private GameObject house;

    [SerializeField]
    [Range(5, 20)]
    private int noOfHouses = 10;

    [SerializeField]
    private float areaRange = 5;

    [SerializeField]
    private float yOffset = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < noOfHouses; i++)
        {
            Debug.Log("House " + i + " placed.");
            Instantiate(house, CreateRandomLocation(), Quaternion.Euler(new Vector3(0,Random.Range(0,360),0)), gameObject.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {

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
