using DG.Tweening;
using Game.Block;
using Game.Level;
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
        private LevelRequirement[] _score;
        public GameplayState(GameManager context) : base(context)
        {
        }

        public override void Enter()
        {
            base.Enter();

            _score = _context.CurrentConfig.levelRequirements;

            _context.PubSubRegister(EventID.OnBlockSelected, OnBlockSelected);
            _context.PubSubRegister(EventID.OnDropToSlot, OnDropToSlot);
            _context.PubSubRegister(EventID.OnDestroyJelly, OnDestroyJelly);

            UIManager.Instance.ShowOverlap<GameplayOverlap>(_context.CurrentConfig, forceShowData: true);
            _context.PubSubBroadcast(EventID.OnStartGameplay);
        }

        public override void Exit()
        {
            base.Exit();
            _context.PubSubUnregister(EventID.OnBlockSelected, OnBlockSelected);
            _context.PubSubUnregister(EventID.OnDropToSlot, OnDropToSlot);
            _context.PubSubUnregister(EventID.OnDestroyJelly, OnDestroyJelly);

        }

        private void OnDestroyJelly(object obj)
        {
            if (obj is not JellyColor jelly) return;
            for (int i = 0; i < _score.Length; i++) 
            {
                if (_score[i].jellyColor == jelly && _score[i].amount > 0)
                {
                    _score[i].amount--;
                    if (_score[i].amount < 0) _score[i].amount = 0;
                    _context.PubSubBroadcast(EventID.OnUIUpdateScore, _score[i]);
                }
            }

            ValidateScore();
        }

        private void ValidateScore()
        {
            foreach (var score in _score) {
                if (score.amount > 0) return;
            }

            _context.PubSubBroadcast(EventID.OnFinishLevel);
            CheckForNextLevel();
        }

        private void CheckForNextLevel()
        {
            var totalLevel = _context.LevelConfigs.Count;
            if (_context.CurrentConfigIndex < totalLevel - 1) 
            {
                GoToNextLevel();
                return;
            }
            FinishGame();
        }

        private void FinishGame()
        {
            throw new NotImplementedException();
        }

        private void GoToNextLevel()
        {
            _context.CurrentConfigIndex++;
            _context.ChangeToCleanupLevelState();
        }

        private void OnDropToSlot(object obj)
        {
            if(_selectedBlock is null) return;
            if (obj is not Slot slot) return;
            if (slot.CurrentBlock is not null) return;
            slot.CurrentBlock = _selectedBlock;
            _selectedBlock.transform.SetParent(slot.container);
            //_selectedBlock.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            _selectedBlock.transform.DOLocalMove(Vector3.zero, .3f).SetEase(Ease.InOutExpo)
                .OnComplete(() =>
                {
                    _selectedBlock.transform.localRotation = Quaternion.identity;
                    _selectedBlock.DroppedToSlot = true;
                    (BlockController, Slot) data = (_selectedBlock, slot);
                    _context.PubSubBroadcast(EventID.OnSetBlockToSlot, data);
                    _selectedBlock = null;
                }); 
        }

        private void OnBlockSelected(object obj)
        {
            if (obj is not BlockController block) return;
            _selectedBlock = block;
        }
    }

}
