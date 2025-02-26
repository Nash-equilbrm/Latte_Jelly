using System;
using Game.Level;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;


namespace Commons
{
    public static class Common
    {
        private static readonly Random _random = new Random();

        public static T GetRandomItem<T>(this IEnumerable<T> collection)
        {
            if (collection == null || !collection.Any())
                throw new InvalidOperationException("Collection is empty or null.");

            return collection.ElementAt(_random.Next(collection.Count()));
        }


        
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
