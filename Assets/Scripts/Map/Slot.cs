using Commons;
using DG.Tweening;
using Game.Block;
using Patterns;
using System;
using UnityEngine;


namespace Game.Map
{
    public class Slot : MonoBehaviour
    {
        public Transform container;
        public SpriteRenderer outline;
        public Color outlineColor;
        public Color outlineSelectColor;
        private Vector2 _onScreenPosition;
        private bool _onHover;
        [SerializeField] private BlockController _currentBlock;
        public BlockController CurrentBlock {
            get
            {
                return _currentBlock;
            }
            set
            {
                _currentBlock = value;
                if (_currentBlock is null) return;
                _currentBlock.Slot = this;
            }
        }

        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubRegister(EventID.OnBlockHovering, OnBlockHovering);

            var slotScreenPosVec3 = Camera.main.WorldToScreenPoint(transform.position);
            _onScreenPosition = (Vector2)slotScreenPosVec3;
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnSetBlockToSlot, OnSetBlockToSlot);
            this.PubSubUnregister(EventID.OnBlockHovering, OnBlockHovering);
        }

        private void OnBlockHovering(object obj)
        {
            if (obj is null || obj is not Collider collider)
            {
                if (outline != null) outline.color = outlineColor;
                _onHover = false;
                return;
            }
            else if (collider.gameObject != gameObject)
            {
                if(outline != null) outline.color = outlineColor;
                _onHover = false;
            }
            else
            {
                if (outline != null) outline.color = outlineSelectColor;
                _onHover = true;
            }

        }

        private void OnSetBlockToSlot(object obj)
        {
            if (obj is not BlockController block) return;
            CurrentBlock = block;
        }


        internal void OnDragEnd(Vector2 screenPos)
        {
            outline.color = outlineColor;
            if (_onHover)
            {
                this.PubSubBroadcast(EventID.OnDropToSlot, this);
            }


            _onHover = false;
        }

        internal void CleanupSlot()
        {
            if (outline != null) outline.color = outlineColor;
            _onHover = false;
            var block = CurrentBlock;
            if(block != null)
            {
                Destroy(block.gameObject);
                CurrentBlock = null;
            }
        }
    }
}

