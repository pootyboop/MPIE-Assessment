using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    //public Vector2[] characterPositions;

    public static Map instance;

    public GameObject map;
    private Character[] characters;



    void Start()
    {
        instance = this;
        characters = GameObject.FindObjectsOfType<Character>();
    }


    public void CharacterGivenBook(Character character)
    {

    }
}
