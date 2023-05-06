using UnityEngine;

public class TurtleTransform
{

    private Vector3 position;
    private Quaternion rotation;

    //initialise the turtle
    public TurtleTransform(Vector3 turtlePosition, Quaternion turtleRotation)
    {
        position = turtlePosition;
        rotation = turtleRotation;
    }

    public Vector3 GetPosition()
    {
        return position;
    }

    public Quaternion GetRotation()
    {
        return rotation;
    }

}
