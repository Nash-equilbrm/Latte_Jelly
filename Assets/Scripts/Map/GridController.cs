using Commons;
using Game.Block;
using Game.Config;
using Game.Level;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UserInput;



namespace Game.Map
{
    public class GridController : MonoBehaviour
    {
        [SerializeField] private GridSpawner _spawner;
        [SerializeField] private Dictionary<(int, int), Slot> _slots = new();


        private void Awake()
        {
            if(_spawner == null) _spawner = GetComponent<GridSpawner>();
        }


        private void OnEnable()
        {
            LogUtility.Info("GridController", "PubSubRegister(EventID.OnInitLevel)");
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
        }



        private void OnDisable()
        {
            LogUtility.Info("GridController", "PubSubUnregister(EventID.OnInitLevel)");
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);


            foreach (var slot in _slots.Values)
            {
                InputReader.Instance.OnDragEnd -= slot.OnDragEnd;
            }
        }

        private void OnCleanupLevel(object obj)
        {
            foreach (var slot in _slots.Values) 
            {
                slot.CleanupSlot();
            }
            _spawner.CleanupGrid();
            this.PubSubBroadcast(EventID.OnFinishCleanupGridSpawner);
        }

        private void OnInitLevel(object obj)
        {
            if (obj is not LevelConfig config) return;
            LogUtility.Info("GridController", "OnInitLevel");

            _slots = _spawner.SpawnGrid(config);
            _spawner.SpawnBlockSpawners(config);
            foreach (var slot in _slots.Values)
            {
                InputReader.Instance.OnDragEnd += slot.OnDragEnd;
            }
        }
        private void OnSetBlockToSlot(object obj)
        {
            var data = ((BlockController, Slot))obj;
            Slot slot = data.Item2;
            BlockController block = data.Item1;
            if (slot is null) return;
            if (!_slots.ContainsValue(slot)) return;
            block.CheckRaycast();
        }


    }

}
