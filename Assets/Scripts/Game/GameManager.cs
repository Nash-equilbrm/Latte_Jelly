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


        [SerializeField] private int _currentConfigIndex;
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
        #endregion
        
        
        private void Start()
        {
            _initState = new(this);
            _initLevelState = new(this);

            _stateMachine.Initialize(_initState);
        }

        #region State Change
        internal void ChangeToInitLevelState()
        {
            _stateMachine.ChangeState(_initLevelState);
        }
        

        #endregion
       
    }

}
