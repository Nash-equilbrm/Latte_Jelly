using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

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
    public struct LevelConfig
    {
        public int id;
        public LevelRequirement[] levelRequirements;
        public JellyColor[] colors;
        public int spawnerCount;
        public int rows; // Grid height
        public int columns; // Grid width
        [SerializeField] private bool[] serializedGrid; // Stores 2D data as 1D array
        
        public bool[,] Grid
        {
            get
            {
                bool[,] grid = new bool[rows, columns];
                if (serializedGrid == null || serializedGrid.Length != rows * columns)
                    serializedGrid = new bool[rows * columns];

                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        grid[r, c] = serializedGrid[r * columns + c];
                    }
                }
                return grid;
            }
            set
            {
                serializedGrid = new bool[rows * columns];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        serializedGrid[r * columns + c] = value[r, c];
                    }
                }
            }
        }

        public void SaveToSerialized()
        {
            serializedGrid = new bool[rows * columns];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    serializedGrid[r * columns + c] = Grid[r, c];
                }
            }
        }

        public override string ToString()
        {
            var res = $"Id = {id}_Requirements = ";
            res = levelRequirements.Aggregate(res, (current, r) => current + (r + "_"));
            res += "_Colors = ";
            res = colors.Aggregate(res, (current, c) => current + (c + "_"));
            res += $"_GridSize: {rows}x{columns}";
            return res;
        }
    }

    [Serializable]
    public struct LevelRequirement
    {
        public JellyColor jellyColor;
        public int amount;

        public override string ToString()
        {
            return $"(Color: {jellyColor}, Amount: {amount})";
        }
    }
}
