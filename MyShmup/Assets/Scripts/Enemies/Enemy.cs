using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    Enemies (clase base):
    -invincible
    -si son prioritarios
    -level requirements index -> update when dead
    -transform de entrada
    -behavior
    -path
    -waypoints
    -transform salida
    -offset hasta primer ataque
    -attack[]:
        -weapon[]
        -duration
        -timeUntilNextAttack
    
    Move
    GameManage.UpdatePriorityEnemies -> when dead
    Update levelRequirements
    Die
    ShootAttack
    */
}
