using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameProperties
{
    public static int _life = 3;
    public static int _score = 0;
    public static int _credits = 1;
    public static int _currentLevel = 0;
    public static int[][] _extraLevelRequirements = new int[][]
    {
        new int[] { 0 },
        new int[] { 0, 1 },
        new int[] { 0, 1 },
        new int[] { 0, 3 }
    };
    public static int _shiplevel = 0;
}
