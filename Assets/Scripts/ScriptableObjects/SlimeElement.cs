using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime Element", menuName = "GameData/SlimeElement", order = 1)]
public class SlimeElement : ScriptableObject
{
    [Header("Animation")]
    public RuntimeAnimatorController animationSet;
    [Header("Sprites")]
    public Sprite fullHeartImage;
    public Sprite halfHeartImage;
    public Sprite emptyHeartImage;
    [Header("Combat")]
    public PlayerMovement.SpecialAttack specialAttack;
    [Header("Visuals")]
    public GameObject groundImpactEffect;
    public GameObject jumpEffect;
    public Color trailStartColor;
    public Color trailEndColor;

}
