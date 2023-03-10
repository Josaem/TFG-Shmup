using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameProperties
{
    public static int _life = 3;
    public static int _score = 0;
    public static int _credits = 1;
    public static int _currentLevel = 1;
    public static int[][] _extraLevelRequirements = new int[][]
    {
        new int[] { 0 },
        new int[] { 0, 1 },
        new int[] { 0, 1 },
        new int[] { 0, 3 }
    };
    public static int _shiplevel = 0;
}

/*
    TODO TAGS:

    TODOVISUALS
    TODOUI
    TODO
 */

/*

    Gun: never stops firing, can swap bullets depending on timing
    -base rotation
    -delay until shooting
    -duration (0 means infinite)
    -gunBehavior[]:
        -rotative
        -pointatplayer
        -fire rate
        -bulletspeed
        -duration
        -bulletObject
        -movementType script to add (component to be added to the bullet on spawn)


    BulletObject
    -bullets (prefab)
    -spawn order/rate
    -time to spawn if multiple bullets
         */
