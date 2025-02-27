using System;
using System.Collections;
using System.Collections.Generic;
using Commons;
using Game.Block;
using Game.Config;
using NaughtyAttributes;
using Patterns;
using UnityEngine;


namespace Game.Map
{
    public class SpawnRandomBlock : MonoBehaviour
    {
        public Transform container;
        private string[] _blockTags = new string [8]
        {
            Constants.BLOCK1,
            Constants.BLOCK2_1,
            Constants.BLOCK2_2,
            Constants.BLOCK3_1,
            Constants.BLOCK3_2,
            Constants.BLOCK3_3,
            Constants.BLOCK3_4,
            Constants.BLOCK4,
        };

        [SerializeField] private BlockController _currentBlock;

        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubRegister(EventID.OnStartGameplay, OnStartGameplay);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubUnregister(EventID.OnStartGameplay, OnStartGameplay);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
        }

        private void OnStartGameplay(object obj)
        {
            SpawnRandom();
        }

        private void OnSetBlockToSlot(object obj)
        {
            var data = ((BlockController, Slot))obj;
            BlockController block = data.Item1;
            if (block != _currentBlock) return;
            
            _currentBlock = null;
            SpawnRandom();
        }

        [Button]
        public void SpawnRandom()
        {
            if (_currentBlock is not null)
            {
                _currentBlock = null;
            }
            var randomBlockTag = _blockTags.GetRandomItem();
            var randomBlock = ObjectPooling.Instance.GetPool(randomBlockTag).Get(
                parent: container,
                rotation: Quaternion.identity.eulerAngles);
            randomBlock.transform.localPosition = Vector3.zero;
            _currentBlock = randomBlock.GetComponent<BlockController>();
        }

        private void OnCleanupLevel(object obj)
        {
            if (_currentBlock != null)
            {
                _currentBlock = null;
            }
            foreach (var tag in _blockTags)
            {
                ObjectPooling.Instance.GetPool(tag).DestroyAll();
            }
            this.PubSubBroadcast(EventID.OnFinishCleanupBlockSpawner);
        }
    }
}