using UnityEngine;

[CreateAssetMenu(menuName = "SettlementGenerator/Rule")]
public class Rule : ScriptableObject
{
    [SerializeField]
    private string input;
    [SerializeField]
    private string output;

    public string GetInput()
    {
        return input;
    }

    public string GetOutput()
    {
        return output;
    }

}
