using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementGenerator : MonoBehaviour
{

    [SerializeField]
    private RoadPlacer roadPlacer;
    [SerializeField]
    private HousePlacer housePlacer;

    [Header("Program Variables")]
    [SerializeField]
    [Range(0, 6)]
    private int lSystemIterations = 1;

    [SerializeField]
    [Range(0, 200)]
    private int numberOfHouses = 10;

    [SerializeField]
    private bool placeHouses;

    [SerializeField]
    [Range(0f, 1f)]
    private float housePlacementRatio = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        List<RoadDetails> roadList = new List<RoadDetails>();
        Vector3[] roadBounds;

        //place roads
        roadList = roadPlacer.CreateRoadSystem(lSystemIterations);

        //find max and min x and z values of road
        if (roadList.Count > 0)
        {
            roadBounds = GetRoadBounds(roadList);
        }
        else
        {
            roadBounds = new Vector3[] { Vector3.zero, Vector3.zero };
        }

        if (placeHouses)
        {
            //place taverns on road
            housePlacer.PlaceTaverns(roadList);

            housePlacer.InitialiseHouseList(numberOfHouses);

            int housesOnRoad = Mathf.RoundToInt(Mathf.Lerp(0, numberOfHouses, housePlacementRatio));
            int failedHouses = 0;
            Debug.Log(housesOnRoad);

            if (housesOnRoad > 0)
            {
                //find places where the houses can go on the road
                List<TurtleTransform> houseLocations = housePlacer.FindHouseLocations(roadList);

                if (housesOnRoad > houseLocations.Count)
                {
                    housesOnRoad = houseLocations.Count;
                }

                //place a certain number of houses on the road
                failedHouses = housePlacer.PlaceHousesOnRoad(housesOnRoad, houseLocations);
            }

            //place houses randomly
            housePlacer.PlaceHousesRandomly(numberOfHouses - (housesOnRoad - failedHouses), roadBounds);

            //spread out houses
            housePlacer.ActivateSpacingMethod();

        }



    }

    private Vector3[] GetRoadBounds(List<RoadDetails> roadList)
    {

        //first item in array is min values
        //second item in array is max values
        Vector3[] roadBounds = new Vector3[2];

        foreach (RoadDetails road in roadList)
        {
            //if road x position is smaller than the current stored min x position
            if (road.GetCentrepoint().x < roadBounds[0].x)
            {
                roadBounds[0].x = road.GetCentrepoint().x;
            }
            //if road x position is bigger than the current stored max x position
            else if (road.GetCentrepoint().x > roadBounds[1].x)
            {
                roadBounds[1].x = road.GetCentrepoint().x;
            }

            //if road z position is smaller than the current stored min z position
            if (road.GetCentrepoint().z < roadBounds[0].z)
            {
                roadBounds[0].z = road.GetCentrepoint().z;
            }
            //if road z position is bigger than the current stored max z position
            else if (road.GetCentrepoint().z > roadBounds[1].z)
            {
                roadBounds[1].z = road.GetCentrepoint().z;
            }

        }


        return roadBounds;
    }


}
