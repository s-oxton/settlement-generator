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

        //place roads
        roadList = roadPlacer.CreateRoadSystem();

        //place big houses on road
        housePlacer.PlaceTaverns(roadList);

        //place small houses/trees randomly


        //spread out small houses/trees
        //housePlacer.SpaceHouses();

        //make any house close to the road face the road

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
