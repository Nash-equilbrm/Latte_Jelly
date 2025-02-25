using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Level
{
    public enum JellyColor
    {
        None = 0,
        Cyan = 1,
        Purple = 2,
        Green = 3,
        Yellow = 4,
        Pink = 5,
    }

    [Serializable]
    public class LevelConfig
    {
        public int id;
        public LevelRequireMent[] levelRequireMents;
        public JellyColor[] colors;
    }


    [Serializable]
    public class LevelRequireMent
    {
        public JellyColor jellyColor;
        public int amount;
    }

}
