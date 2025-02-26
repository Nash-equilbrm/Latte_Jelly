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
        private GridSpawner _gridSpawner;
        public InitLevelState(GameManager context) : base(context)
        {
        }

        public override void Enter()
        {
            base.Enter();
            SetUpGameObjects();
            LogUtility.Info("InitLevelState.PubSubBroadcast", "EventID.OnInitLevel");
            _context.PubSubBroadcast(EventID.OnInitLevel, _context.CurrentConfig);
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void SetUpGameObjects()
        {
            if (!_gridSpawner)
            {
                _gridSpawner = Resources.FindObjectsOfTypeAll<GridSpawner>().FirstOrDefault();
            }
            _gridSpawner?.gameObject.SetActive(true);
        }
    }
}

