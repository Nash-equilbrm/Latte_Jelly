using Game.Level;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Commons
{
    public static class Common
    {
        public static Color GetColorFromJelly(JellyColor color)
        {
            return color switch
            {
                JellyColor.Cyan => Color.cyan,
                JellyColor.Purple => new Color(0.5f, 0f, 0.5f),
                JellyColor.Green => Color.green,
                JellyColor.Yellow => Color.yellow,
                _ => Color.white,
            };
        }
    }

}
