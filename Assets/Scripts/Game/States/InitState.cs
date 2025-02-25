using DG.Tweening;
using Game.Level;
using Patterns;
using UnityEngine;


namespace Game.States
{
    public class InitState : State<GameManager>
    {
        private bool _initialized = false;
        public InitState(GameManager context) : base(context)
        {
        }

        public override void Enter()
        {
            base.Enter();
            //if (_initialized) return;
            //_initialized = true;
            DOTween.Init();
            Application.targetFrameRate = 60;
            LoadLevelConfigs();
        }

        public override void Exit()
        {
            base.Exit();
        }

        private void LoadLevelConfigs()
        {
            foreach (var file in _context.configFiles)
            {
                var levelConfig = JsonUtility.FromJson<LevelConfig>(file.text);
                _context.LevelConfigs.Add(levelConfig);
            }
        }
    }

}
