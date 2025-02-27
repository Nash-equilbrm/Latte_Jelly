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
        internal List<LevelConfig> LevelConfigs { get => _levelConfigs; set => _levelConfigs = value; }


        [SerializeField] private int _currentConfigIndex = 0;
        public int CurrentConfigIndex
        {
            get => _currentConfigIndex;
            internal set => _currentConfigIndex = Mathf.Clamp(value, 0, _levelConfigs.Count - 1);
        }

        public LevelConfig CurrentConfig => LevelConfigs[CurrentConfigIndex];
        
        #region States
        private StateMachine<GameManager> _stateMachine = new();
        private InitState _initState;
        private InitLevelState _initLevelState;
        private GameplayState _gameplayState;
        private CleanupLevelState _cleanupLevelState;
        #endregion


        public List<GameObject> initAfterPubSub;

        public string StateName = "";
        private void Update()
        {
            StateName = _stateMachine.CurrentState.name;
        }


        private void Start()
        {
            foreach (var obj in initAfterPubSub) { obj.SetActive(false); }
            _initState = new(this, "Init State");
            _initLevelState = new(this, "Init Level State");
            _gameplayState = new(this, "Gameplay State");
            _cleanupLevelState = new(this, "Clean up Level State");

            _stateMachine.Initialize(_initState);
        }

        #region State Change
        internal void ChangeToInitLevelState()
        {
            _stateMachine.ChangeState(_initLevelState);
        }

        internal void ChangeToGameplayState()
        {
            _stateMachine.ChangeState(_gameplayState);
        }

        internal void ChangeToCleanupLevelState()
        {
            _stateMachine.ChangeState(_cleanupLevelState);
        }
        #endregion

    }

}
