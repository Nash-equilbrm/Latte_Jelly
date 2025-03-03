using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.Config
{
    public static class Constants
    {
        public static int MAX_ROWS = 2;
        public static int MAX_COLUMNS = 2;
        public static Vector2 CELL_SIZE = new (2,2);
        public static string PLANE_TAG = "plane";


        public static string SLOT_TAG = "slot";
        public static string BLOCK1 = "block1";
        public static string BLOCK2_1 = "block2_1";
        public static string BLOCK2_2 = "block2_2";
        public static string BLOCK3_1 = "block3_1";
        public static string BLOCK3_2 = "block3_2";
        public static string BLOCK3_3 = "block3_3";
        public static string BLOCK3_4 = "block3_4";
        public static string BLOCK4 = "block4";


        public static float INTERVAL_BETWEEN_LEVELS = 2f;


        #region VFX
        public static string VFX_JELLY_POP = "JellyPopVFX";
        #endregion

        #region AUDIO
        public static string SFX_POP = "Pop";
        public static string SFX_JELLY_POP = "JellyPop";
        public static string SFX_BTN_CLICKED = "BtnClicked";

        #endregion
    }

}
