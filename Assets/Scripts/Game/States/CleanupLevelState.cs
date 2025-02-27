using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.States
{
    public class CleanupLevelState : State<GameManager>
    {
        private bool _finishCleanupGridSpawner = false;
        private bool _finishCleanupBlockSpawner = false;
        public CleanupLevelState(GameManager context) : base(context)
        {
        }

        public override void Enter()
        {
            base.Enter();
            RegisterEvents();
            _context.PubSubBroadcast(EventID.OnCleanupLevel);
        }

        public override void Exit()
        {
            base.Exit();
            UnregisterEvents();

        }

        private void RegisterEvents()
        {
            _context.PubSubRegister(EventID.OnFinishCleanupGridSpawner, OnFinishCleanupGridSpawner);
            _context.PubSubRegister(EventID.OnFinishCleanupBlockSpawner, OnFinishCleanupBlockSpawner);
        }

        private void UnregisterEvents()
        {
            _context.PubSubUnregister(EventID.OnFinishCleanupGridSpawner, OnFinishCleanupGridSpawner);
            _context.PubSubUnregister(EventID.OnFinishCleanupBlockSpawner, OnFinishCleanupBlockSpawner);
        }

        private void OnFinishCleanupBlockSpawner(object obj)
        {
            _finishCleanupBlockSpawner = true;
            ValidateCleanup();
        }

        private void OnFinishCleanupGridSpawner(object obj)
        {
            _finishCleanupGridSpawner = true;
            ValidateCleanup();
        }

        private void ValidateCleanup()
        {
            if(_finishCleanupBlockSpawner 
                && _finishCleanupGridSpawner)
            {
                _context.ChangeToInitLevelState();
            }
        }
    }
}

