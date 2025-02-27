using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Commons;
using Game.Map;
using Patterns;
using UnityEngine;


namespace  Game.States
{
    /// <summary>
    /// TODO: Init new level
    /// </summary>
    public class InitLevelState : State<GameManager>
    {
        private bool _objectInitialized = false;
        private GridSpawner _gridSpawner;
        public InitLevelState(GameManager context) : base(context)
        {
        }

        public InitLevelState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            SetUpGameObjects();
            LogUtility.Info("InitLevelState", "PubSubBroadcast(EventID.OnInitLevel)");
            _context.PubSubBroadcast(EventID.OnInitLevel, _context.CurrentConfig);
            _context.ChangeToGameplayState();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void SetUpGameObjects()
        {
            if (_objectInitialized) return;

            _objectInitialized = true;
            
            foreach (var go in _context.initAfterPubSub)
            {
                go.SetActive(true);
            }

        }
    }
}

