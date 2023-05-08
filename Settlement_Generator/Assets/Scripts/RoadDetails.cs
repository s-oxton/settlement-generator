using UnityEngine;

public class RoadDetails
{

    private Vector3 roadDirection;

    private Vector3 roadCentrePoint;

    private float roadWidth;

    public RoadDetails( Quaternion rotation, Vector3 centrePoint, float width)
    {
        roadDirection = rotation * Vector3.forward;
        roadCentrePoint = centrePoint;
        roadWidth = width;
    }

    public Vector3 GetDirection()
    {
        return roadDirection;
    }

    public Vector3 GetCentrepoint()
    {
        return roadCentrePoint;
    }

    public float GetRoadWidth()
    {
        return roadWidth;
    }

}
