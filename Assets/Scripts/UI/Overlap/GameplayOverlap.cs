using Commons;
using DG.Tweening;
using Game.Level;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace Game.UI
{
    public class GameplayOverlap : BaseOverlap
    {
        public GameObject jellyScoreTemplate;
        public HorizontalLayoutGroup jellyScoreLayout;
        private List<GameObject> _jellyScoreUI = new();
        private Dictionary<JellyColor, TMP_Text> _uiDict = new();

        public override void Hide()
        {
            Sequence seq = DOTween.Sequence().Pause();
            foreach (var jellyUI in _jellyScoreUI)
            {
                seq.Join(jellyUI.transform.DOScale(Vector3.zero, 1.3f).SetEase(Ease.InOutExpo));
            }
            seq.OnComplete(() =>
            {
                base.Hide();
                UnregisterEvents();
                foreach (var item in _jellyScoreUI)
                {
                    Destroy(item);
                }
                _jellyScoreUI.Clear();
                _uiDict.Clear();
                jellyScoreLayout.gameObject.SetActive(false);
            });
           
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Show(object data)
        {
            base.Show(data);
            if (data is not LevelConfig config) return;

            RegisterEvents();


            foreach(var requirement in config.levelRequirements)
            {
                JellyColor color = requirement.jellyColor;
                var jellyUI = Instantiate(jellyScoreTemplate, jellyScoreTemplate.transform.parent);
                jellyUI.GetComponent<UnityEngine.UI.Image>().color = Common.GetColorFromJelly(color);
                _uiDict.Add(color, jellyUI.GetComponentInChildren<TMP_Text>());
                _uiDict[color].text = requirement.amount.ToString();
                _jellyScoreUI.Add(jellyUI);
                jellyUI.transform.localScale = Vector3.zero;
                jellyUI.SetActive(true);
                jellyUI.transform.DOScale(Vector3.one, 1.3f).SetEase(Ease.InOutExpo);
            }
            jellyScoreLayout.gameObject.SetActive(true);
        }

        private void RegisterEvents()
        {
            this.PubSubRegister(EventID.OnUIUpdateScore, OnUIUpdateScore);
            this.PubSubRegister(EventID.OnFinishLevel, OnFinishLevel);
            this.PubSubRegister(EventID.OnCleanupLevel, OnCleanupLevel);
        }


        private void UnregisterEvents()
        {
            this.PubSubUnregister(EventID.OnUIUpdateScore, OnUIUpdateScore);
            this.PubSubUnregister(EventID.OnFinishLevel, OnFinishLevel);
            this.PubSubUnregister(EventID.OnCleanupLevel, OnCleanupLevel);
        }

        private void OnCleanupLevel(object obj)
        {
            Hide();
        }

        private void OnFinishLevel(object obj)
        {
            
        }

        private void OnUIUpdateScore(object obj)
        {
            if (obj is not LevelRequirement score) return;
            var ui = _uiDict.FirstOrDefault(x => x.Key == score.jellyColor);
            ui.Value.text = score.amount.ToString();
            ui.Value.rectTransform.DOShakeScale(.3f).SetEase(Ease.InOutExpo);
        }
    }
}

