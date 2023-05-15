using UnityEngine;

public class RoadDetails
{

    GameObject road;

    public RoadDetails(GameObject tempRoad)
    {
        road = tempRoad;
    }

    public GameObject GetRoad()
    {
        return road;
    }

    public Vector3 GetDirection()
    {
        return road.transform.rotation * Vector3.forward;
    }

    public Vector3 GetPosition()
    {
        return road.transform.position;
    }

    public Vector3 GetCentrepoint()
    {
        return road.transform.Find("CentrePoint").transform.position;
    }

    public float GetRoadWidth()
    {
        return road.transform.GetChild(0).transform.localScale.x;
    }

    public float GetRoadLength()
    {
        return road.transform.localScale.z * road.transform.Find("RoadMesh").localScale.z;
    }

}
