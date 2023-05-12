using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    [Header("LSystem Components")]
    [SerializeField]
    private string[] axioms;
    [SerializeField]
    private Rule[] rules;

    public string GenerateSentence(int iterations)
    {

        string sentence = axioms[Random.Range(0, axioms.Length)];
        for (int i = 0; i < iterations; i++)
        {
            sentence = ApplyRules(sentence);
        }

        return sentence;

    }

    private string ApplyRules(string sentence)
    {
        List<Rule> viableRules = new List<Rule>();
        StringBuilder newSentence = new StringBuilder();

        //loop through every character in the sentence
        foreach (char c in sentence)
        {
            viableRules.Clear();
            //check which rules the character matches
            foreach (Rule rule in rules)
            {
                //if character equals character input for the rule, add the rule to a list of potential rules
                //more than one rule can be valid given a certain character, so list of them needs to be created.
                if (c.ToString().Equals(rule.GetInput()))
                {
                    viableRules.Add(rule);
                }
            }

            //if one of more rules is viable, pick one at random to apply
            if (viableRules.Count > 0)
            {
                newSentence.Append(viableRules[Random.Range(0, viableRules.Count)].GetOutput());
            }
            else
            {
                newSentence.Append(c.ToString());
            }
        }


        return newSentence.ToString();

    }

}
