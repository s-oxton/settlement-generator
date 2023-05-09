using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettlementGenerator : MonoBehaviour
{

    [SerializeField]
    private RoadPlacer roadPlacer;
    [SerializeField]
    private HousePlacer housePlacer;

    // Start is called before the first frame update
    void Start()
    {
        List<RoadDetails> roadList = new List<RoadDetails>();
        Vector3[] roadBounds;

        //place roads
        roadList = roadPlacer.CreateRoadSystem();

        //find max and min x and z values of road
        roadBounds = GetRoadBounds(roadList);

        //place big houses on road
        housePlacer.PlaceTaverns(roadList);

        //place small houses/trees randomly
        housePlacer.PlaceHouses(roadBounds);

        //spread out small houses/trees
        housePlacer.SpaceHouses();

        //make any house close to the road face the road

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
