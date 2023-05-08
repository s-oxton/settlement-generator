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
