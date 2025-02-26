using System;
using Commons;
using Game.Config;
using Game.Level;
using Patterns;
using UnityEngine;

namespace Game.Map
{
    public class GridSpawner : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.name = nameof(GridSpawner);
        }

        private void OnEnable()
        {
            LogUtility.Info("GridSpawner.PubSubRegister", "EventID.OnInitLevel");
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
        }

        private void OnInitLevel(object obj)
        {
            if (obj is not LevelConfig config) return;
            SpawnGrid(config);
        }


        private void SpawnGrid(LevelConfig levelConfig)
        {
            bool[,] grid = levelConfig.Grid;

            for (int r = 0; r < levelConfig.rows; r++)
            {
                for (int c = 0; c < levelConfig.columns; c++)
                {
                    if (!grid[r, c]) continue;
                    
                    Vector3 spawnPos = new Vector3((c - levelConfig.columns / 2) * Constants.CELL_SIZE.x, 0, r * Constants.CELL_SIZE.y);
                    GameObject newSlot = ObjectPooling.Instance.GetPool(Constants.SLOT_TAG)
                        .Get(position: spawnPos, rotation: Quaternion.identity.eulerAngles);
                    newSlot.name = $"Slot_{r}_{c}";
                    newSlot.transform.SetParent(transform); // Keep hierarchy clean
                }
            }
        }
    }
}