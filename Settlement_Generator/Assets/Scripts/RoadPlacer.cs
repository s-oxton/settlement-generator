using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadPlacer : MonoBehaviour
{

    [Header("Road Components")]
    [SerializeField]
    private GameObject turtle;
    [SerializeField]
    private GameObject roadPrefab;
    [SerializeField]
    private GameObject junctionPrefab;
    [SerializeField]
    private LSystem lSystem;

    [Header("Road Variables")]
    [SerializeField]
    [Range(70f, 110f)]
    private float baseAngle = 90;
    [SerializeField]
    [Range(0f, 20f)]
    private float angleVariance;
    [SerializeField]
    [Range(0f, 1f)]
    private float roadScaling;

    private Stack<TurtleTransform> turtleTransforms = new Stack<TurtleTransform>();
    private List<TurtleTransform> roadJuctionTransforms = new List<TurtleTransform>();

    private int depth = -1;
    private string sentence;

    private void Start()
    {
        CreateRoadSystem();
    }

    public void CreateRoadSystem()
    {
        sentence = lSystem.GenerateSentence();
        Debug.Log(sentence);
        PlaceRoads(sentence);

        foreach (TurtleTransform junction in roadJuctionTransforms)
        {
            Instantiate(junctionPrefab, junction.GetPosition(), junction.GetRotation(), this.transform);
        }

    }

    private void PlaceRoads(string sentence)
    {

        foreach (char c in sentence)
        {

            GameObject road;
            float randomRotation = 0f;
            TurtleTransform tempTurtle;

            //create the encoding for the character
            Encoding encoding = (Encoding)c;

            //switch statement for determining what to do with each character
            switch (encoding)
            {
                case Encoding.save:
                    //push current turtle position and rotation to stack
                    tempTurtle = new TurtleTransform(turtle.transform.position, turtle.transform.rotation);
                    turtleTransforms.Push(tempTurtle);
                    if (depth < 5)
                    {
                        roadJuctionTransforms.Add(tempTurtle);
                    }
                    depth++;
                    break;
                case Encoding.load:
                    //pop from stack, update turtle position and rotation
                    tempTurtle = turtleTransforms.Pop();
                    turtle.transform.position = tempTurtle.GetPosition();
                    turtle.transform.rotation = tempTurtle.GetRotation();
                    depth--;
                    break;
                case Encoding.road:
                case Encoding.endRoad:
                    //place road at current position and rotation of turtle
                    road = Instantiate(roadPrefab, turtle.transform.position, turtle.transform.rotation, this.transform);
                    road.transform.localScale = new Vector3(1, 1, 1 * Mathf.Pow(roadScaling, depth));
                    //update position of turtle to end of road. (note, rotation does not need to be updated)
                    turtle.transform.position = road.transform.Find("Connector").transform.position;
                    break;
                case Encoding.right:
                    //rotate turtle to the right. can implement random angles later by using angleVariance
                    randomRotation = Random.Range(-angleVariance, angleVariance);
                    turtle.transform.Rotate(0f, baseAngle + randomRotation, 0f);
                    break;
                case Encoding.left:
                    //rotate turtle to the left. can implement random angles later by using angleVariance
                    randomRotation = Random.Range(-angleVariance, angleVariance);
                    turtle.transform.Rotate(0f, -baseAngle + randomRotation, 0f);
                    break;
                case Encoding.straight:
                    //rotate turtle to the left. can implement random angles later by using angleVariance
                    randomRotation = Random.Range(-angleVariance, angleVariance);
                    turtle.transform.Rotate(0f, randomRotation, 0f);
                    break;
                default:
                    break;
            }

        }

    }

    //encoding to make switch statement clearer
    private enum Encoding
    {
        save = '[',
        load = ']',
        road = 'G',
        endRoad = 'F',
        right = '+',
        left = '-',
        straight = '='

    }

}
