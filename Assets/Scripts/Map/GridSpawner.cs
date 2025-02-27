using System;
using System.Collections.Generic;
using Commons;
using Game.Block;
using Game.Config;
using Game.Level;
using Patterns;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace Game.Map
{
    public class GridSpawner : MonoBehaviour
    {
        
        [SerializeField] private List<SpawnRandomBlock> _blockSpawners;
        [SerializeField] private SpawnRandomBlock _blockSpawnerPrefab;
        [SerializeField] private SpawnerHorizontalLayout _spawnerHorizontalLayout;

       
        internal void SpawnBlockSpawners(LevelConfig levelConfig)
        {
            if (_blockSpawners.Count < levelConfig.spawnerCount)
            {
                for (int i = _blockSpawners.Count + 1; i <= levelConfig.spawnerCount; ++i)
                {
                    var newSpawner = Instantiate(_blockSpawnerPrefab.gameObject, parent: _spawnerHorizontalLayout.transform);
                    newSpawner.SetActive(true);
                    _blockSpawners.Add(newSpawner.GetComponent<SpawnRandomBlock>());

                }
            }
            _spawnerHorizontalLayout.Arrange();
            foreach (var s in _blockSpawners)
            {
                s.SpawnRandom();
            }
        }


        internal Dictionary<(int,int),Slot> SpawnGrid(LevelConfig levelConfig)
        {
            Dictionary<(int, int), Slot> res = new();
            bool[,] grid = levelConfig.Grid;

            for (int r = 0; r < levelConfig.rows; r++)
            {
                for (int c = 0; c < levelConfig.columns; c++)
                {
                    if (!grid[r, c]) continue;
                    
                    Vector3 spawnPos = new Vector3((c - levelConfig.columns / 2) * Constants.CELL_SIZE.x, 0, -r * Constants.CELL_SIZE.y);
                    GameObject newSlot = ObjectPooling.Instance.GetPool(Constants.SLOT_TAG)
                        .Get(position: spawnPos, rotation: Quaternion.identity.eulerAngles);
                    newSlot.name = $"Slot_{c}_{r}";
                    newSlot.transform.SetParent(transform);

                    res.Add((c, r), newSlot.GetComponent<Slot>());
                }
            }

            return res;

        }

        internal void CleanupGrid()
        {
            ObjectPooling.Instance.GetPool(Constants.SLOT_TAG).RecycleAll();
        }

    }
}