using Game.Block;
using Game.Map;
using Game.UI;
using Patterns;
using System;
using UI;
using UnityEngine;


namespace Game.States
{
    public class GameplayState : State<GameManager>
    {
        private BlockController _selectedBlock;

        public GameplayState(GameManager context) : base(context)
        {
        }

        public override void Enter()
        {
            base.Enter();
            _context.PubSubRegister(EventID.OnBlockSelected, OnBlockSelected);
            _context.PubSubRegister(EventID.OnDropToSlot, OnDropToSlot);

            UIManager.Instance.ShowOverlap<GameplayOverlap>(_context.CurrentConfig, forceShowData: true);
        }

        public override void Exit()
        {
            base.Exit();
            _context.PubSubUnregister(EventID.OnBlockSelected, OnBlockSelected);
            _context.PubSubUnregister(EventID.OnDropToSlot, OnDropToSlot);
        }

        private void OnDropToSlot(object obj)
        {
            if(_selectedBlock is null) return;
            if (obj is not Slot slot) return;
            if (slot.CurrentBlock is not null) return;
            _selectedBlock.transform.SetParent(slot.container);
            _selectedBlock.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _selectedBlock.DroppedToSlot = true;
            (BlockController, Slot) data = (_selectedBlock, slot);
            _context.PubSubBroadcast(EventID.OnSetBlockToSlot, data);
            _selectedBlock = null;

        }

        private void OnBlockSelected(object obj)
        {
            if (obj is not BlockController block) return;
            _selectedBlock = block;
        }
    }

}
