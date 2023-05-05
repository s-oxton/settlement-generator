using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class LSystem : MonoBehaviour
{
    [Header("LSystem Components")]
    [SerializeField]
    private string axiom;
    [SerializeField]
    [Range(0, 8)]
    private int iterations;
    [SerializeField]
    private Rule[] rules;

    [Header("LSystem Variables")]
    [SerializeField]
    [Range(0f, 20f)]
    private float angleRange;

    private void Start()
    {
        string sentence = GenerateSentence();

    }

    private string GenerateSentence()
    {
        string sentence = axiom;
        Debug.Log(sentence);
        for (int i = 0; i < iterations; i++)
        {
            sentence = ApplyRules(sentence);
            Debug.Log(sentence);
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
            newSentence.Append(c.ToString());
            //if one of more rules is viable, pick one at random to apply
            if (viableRules.Count > 0)
            {
                newSentence.Append(viableRules[Random.Range(0, viableRules.Count)].GetOutput());
            }
        }


        return newSentence.ToString();

    }

}
