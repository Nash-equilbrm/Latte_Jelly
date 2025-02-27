using System.Collections;
using Commons;
using DG.Tweening;
using Game.Config;
using Game.Level;
using Patterns;
using UnityEngine;


namespace Game.States
{
    /// <summary>
    /// TODO: Init game specs when loaded
    /// </summary>
    public class InitState : State<GameManager>
    {
        private bool _initialized = false;
        public InitState(GameManager context) : base(context)
        {
        }

        public InitState(GameManager context, string name) : base(context, name)
        {
        }

        public override void Enter()
        {
            base.Enter();
            if (!_initialized)
            {
                _initialized = true;
                DOTween.Init();
                Application.targetFrameRate = 60;
                LoadLevelConfigs();
                _context.StartCoroutine(IEWaitForSingletons());
                PreparePools();
                _context.CurrentConfigIndex = 0;
            }
            _context.ChangeToInitLevelState();
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
                LogUtility.Info("InitState", levelConfig.ToString());

                _context.LevelConfigs.Add(levelConfig);
            }
        }

        private void PreparePools()
        {
            //ObjectPooling.Instance.GetPool(Constants.SLOT_TAG).Prepare(20);
        }

        private IEnumerator IEWaitForSingletons()
        {
            yield return new WaitUntil(() => PubSub.HasInstance && ObjectPooling.HasInstance);
        }
    }

}
