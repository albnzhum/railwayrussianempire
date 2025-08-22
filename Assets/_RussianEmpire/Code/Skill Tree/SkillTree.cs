using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTree : MonoBehaviour
{
    public static SkillTree skillTree;
    
    private void Awake() => skillTree = this;

    public int[] SkillLevels;
    public int[] SkillCaps;
    public string[] SkillNames;
    public string[] SkillDescriptions;
    
    public List<Skill> Skills = new List<Skill>();
    public GameObject SkillHolder;

    public int SkillPoint;

    private void Start()
    {
        SkillPoint = 20;
        
        SkillLevels = new int[6];
        SkillCaps = new int[] {1, 5, 5, 2, 10, 10};
        
        SkillNames = new[] { "Upgrade 1", "Upgrade 2", "Upgrade 3", "Upgrade 4", "Upgrade 5", "Upgrade 6" };
        SkillDescriptions = new[]
        {
            "Does a thing",
            "Does a cool thing",
            "Does a warm up",
            "Does a really cool thing",
            "Does an awesome thing",
            "Does a really cool thing"
        };


        foreach (var skill in SkillHolder.GetComponentsInChildren<Skill>())
        {
            
        }
    }
    
    
}
