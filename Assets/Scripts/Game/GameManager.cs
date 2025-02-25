using Game.Level;
using Game.States;
using Patterns;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game
{
    public class GameManager : Singleton<GameManager>
    {
        [SerializeField] internal TextAsset[] configFiles;
        [SerializeField] private List<LevelConfig> _levelConfigs = new();
        internal List<LevelConfig> LevelConfigs { get => levelConfigs; private set => levelConfigs = value; }

        #region States
        private StateMachine<GameManager> _stateMachine = new();
        private InitState _initState;
        private List<LevelConfig> levelConfigs = new();


        #endregion


        private void Start()
        {
            _initState = new(this);

            _stateMachine.Initialize(_initState);
        }



    }

}
