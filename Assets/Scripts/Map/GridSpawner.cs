using System.Collections.Generic;
using Commons;
using Game.Config;
using Game.Level;
using Patterns;
using UnityEngine;

namespace Game.Map
{
    public class GridSpawner : MonoBehaviour
    {
        
        [SerializeField] private List<SpawnRandomBlock> _blockSpawners;
        [SerializeField] private SpawnRandomBlock _blockSpawnerTemplate;
        [SerializeField] private SpawnerHorizontalLayout _spawnerHorizontalLayout;
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
            if (_blockSpawners.Count < config.spawnerCount)
            {
                for (int i = _blockSpawners.Count + 1; i <= config.spawnerCount; ++i)
                {
                    var newSpawner = Instantiate(_blockSpawnerTemplate.gameObject, parent: _spawnerHorizontalLayout.transform);
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