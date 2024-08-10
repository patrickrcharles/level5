using System;
using System.Collections.Generic;

public static class CpuBaseStats
{
    public const  int ACCURACY_3PT = 75;
    public const  int ACCURACY_4PT = 75;
    public const  int ACCURACY_7PT = 75;
    public const  int RELEASE = 75;
    public const  int RANGE = 25;
    public const  int LUCK = 0;
    public const  int CLUTCH = 0;
    public const  int CLUTCH_DIVIDER = 2;
    public const  int LUCK_DIVIDER = 3;
    public const float JUMP_FLOOR = 3.5f;
    public const float JUMP_CEILING = 6;
    public const float SPEED_FLOOR = 2.5f;
    public const float SPEED_CEILING = 6.5f;
    public const float MAX_SHOOT_DELAY = 2f;

    public enum ShooterType
    {
        Three,
        Four,
        Seven,
    }

    public enum DefensiveType
    {
        easy,
        normal,
        hardcore
    }
}
