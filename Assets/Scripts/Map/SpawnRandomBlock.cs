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

        private BlockController _currentBlock;
        
        [Button]
        public void SpawnRandom()
        {
            if (_currentBlock is not null)
            {
                ObjectPooling.Remove(_currentBlock.gameObject);
                _currentBlock = null;
            }
            var position = transform.position;
            var randomBlockTag = _blockTags.GetRandomItem();
            var randomBlock = ObjectPooling.Instance.GetPool(randomBlockTag).Get(
                parent: container,
                rotation: Quaternion.identity.eulerAngles);
            randomBlock.transform.localPosition = Vector3.zero;
            _currentBlock = randomBlock.GetComponent<BlockController>();
        }
    }
}