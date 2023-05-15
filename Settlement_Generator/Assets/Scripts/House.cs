using UnityEngine;

public class House
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
        return house.transform.position;
    }

    public Quaternion GetRotation()
    {
        return house.transform.rotation;
    }

    public void SetPosition(Vector3 newPosition)
    {
        house.transform.position = newPosition;
    }

    public void SetRotation(Quaternion rotation)
    {
        house.transform.rotation = rotation;
    }

    public Collider GetCollider()
    {
        return house.GetComponent<Collider>();
    }

}
