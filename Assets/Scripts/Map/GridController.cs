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
            this.PubSubRegister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubRegister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
        }



        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnInitLevel, OnInitLevel);
            this.PubSubUnregister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);


            foreach (var slot in _slots.Values)
            {
                // InputReader.Instance.OnDragging -= slot.OnDragging;
                InputReader.Instance.OnDragEnd -= slot.OnDragEnd;
            }
        }


        private void OnInitLevel(object obj)
        {
            if (obj is not LevelConfig config) return;
            _slots = _spawner.SpawnGrid(config);
            _spawner.SpawnBlockSpawners(config);
            foreach (var slot in _slots.Values)
            {
                // InputReader.Instance.OnDragging += slot.OnDragging;
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
            //StartCoroutine(IEOnSetBlockToSlot(slot, block));
            block.CheckRaycast();
        }





        private IEnumerator IEOnSetBlockToSlot(Slot slot, BlockController block)
        {
            if(slot.CurrentBlock != block)
            {
                slot.CurrentBlock = block;
            }


            List<Slot> adjacentSlots = GetAdjacentSlots(slot);


            Dictionary<Jelly, List<Jelly>> sameColorSubblock = new();
            foreach (Jelly subBlock in block.SubBlocks) 
            {
                foreach (Slot adjSlot in adjacentSlots)
                {
                    if (adjSlot.CurrentBlock is null
                        || adjSlot.CurrentBlock.SubBlocks is null
                        || adjSlot.CurrentBlock.SubBlocks.Count == 0) continue;
                    foreach (Jelly adjsBlock in adjSlot.CurrentBlock.SubBlocks)
                    {
                        if (subBlock.JellyColor != adjsBlock.JellyColor) continue;

                        if (ValidateAdjacentSubblock(subBlock, slot, adjsBlock, adjSlot))
                        {
                            if (sameColorSubblock.ContainsKey(subBlock))
                            {
                                sameColorSubblock[subBlock].Add(adjsBlock);
                            }
                            else
                            {
                                sameColorSubblock.Add(subBlock, new List<Jelly>() { adjsBlock });
                            }
                        }
                    }
                }
            }
            if (sameColorSubblock.Count == 0) yield break;
            foreach(var subblock in sameColorSubblock.Keys)
            {
                foreach(var adjsBlock in sameColorSubblock[subblock])
                {
                    adjsBlock.RemoveSelf();
                }
                subblock.RemoveSelf();
            }

            yield return new WaitForSeconds(0.5f);
            yield return IEOnSetBlockToSlot(slot, block);
        }



        

        


        private bool ValidateAdjacentSubblock(Jelly sbA, Slot A, Jelly sbB, Slot B)
        {
            var bAPositionInMap = _slots.GetKeyFromValue(A);
            var sbApositionInMap = (
                    bAPositionInMap.Item1 * Constants.MAX_COLUMNS + sbA.X,
                    bAPositionInMap.Item2 * Constants.MAX_ROWS + sbA.Y
                );

            var bBPositionInMap = _slots.GetKeyFromValue(B);
            var sbBpositionInMap = (
                    bBPositionInMap.Item1 * Constants.MAX_COLUMNS + sbB.X,
                    bBPositionInMap.Item2 * Constants.MAX_ROWS + sbB.Y
                );

            return AreRectanglesTouching(
                new Rectangle() { position = new(sbApositionInMap.Item1, sbApositionInMap.Item2), size = new((int)sbA.Size.x, (int)sbA.Size.y) },
                new Rectangle() { position = new(sbBpositionInMap.Item1, sbBpositionInMap.Item2), size = new((int)sbB.Size.x, (int)sbB.Size.y) }
                );
        }
        public struct Rectangle
        {
            public Vector2Int position;
            public Vector2Int size;
        }

        public bool AreRectanglesTouching(Rectangle a, Rectangle b)
        {
            int aLeft = a.position.x;
            int aRight = a.position.x + a.size.x - 1;
            int aTop = a.position.y;
            int aBottom = a.position.y + a.size.y - 1;

            int bLeft = b.position.x;
            int bRight = b.position.x + b.size.x - 1;
            int bTop = b.position.y;
            int bBottom = b.position.y + b.size.y - 1;

            bool touchingVertically = (aRight == bLeft - 1 || aLeft == bRight + 1) &&
                                      (aTop <= bBottom && aBottom >= bTop);

            bool touchingHorizontally = (aTop == bBottom + 1 || aBottom == bTop - 1) &&
                                        (aRight >= bLeft && aLeft <= bRight);

            return touchingVertically || touchingHorizontally;
        }


        private List<Slot> GetAdjacentSlots(Slot slot)
        {
            (int, int) position = _slots.GetKeyFromValue(slot);
            (int, int)[] adjacentPositions = new (int, int)[]
            {
                (position.Item1 - 1, position.Item2),
                (position.Item1 + 1, position.Item2),
                (position.Item1, position.Item2 - 1),
                (position.Item1, position.Item2 + 1)
            };

            List<Slot> adjacentSlots = new();
            foreach (var p in adjacentPositions)
            {
                if (_slots.ContainsKey(p))
                    adjacentSlots.Add(_slots[p]);
            }

            return adjacentSlots;
        }


    }

}
