using DG.Tweening;
using Game.Audio;
using Game.Config;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UI;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;


namespace Game.UI
{
    public class ReplayPopup : BasePopup
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private CanvasGroup _panelCanvasGroup;
        [SerializeField] private Button _replayBtn;
        public override void Hide()
        {
            DoHideAnim(() =>
            {
                base.Hide();
            });
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Show(object data)
        {
            base.Show(data);
            DoShowAnim();
        }


        private void DoShowAnim(Action callback = null)
        {
            DOTween.Sequence()
                .Join(_panel.DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutExpo))
                .Join((_replayBtn.transform as RectTransform).DOAnchorPos(Vector2.zero, 1f).SetEase(Ease.InOutExpo))
                .Join(_panelCanvasGroup.DOFade(1, 1f).SetEase(Ease.InOutExpo))
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }


        private void DoHideAnim(Action callback = null)
        {
            DOTween.Sequence()
                .Join(_panel.DOAnchorPos(new Vector2(0f, -200f), 1f).SetEase(Ease.InOutExpo))
                .Join((_replayBtn.transform as RectTransform).DOAnchorPos(new Vector2(0f, -60f), 1f).SetEase(Ease.InOutExpo))
                .Join(_panelCanvasGroup.DOFade(0, 1f).SetEase(Ease.InOutExpo))
                .OnComplete(() =>
                {
                    callback?.Invoke();
                });
        }


        public void OnReplayBtnClicked()
        {
            this.PubSubBroadcast(EventID.OnReplayBtnClicked);
            AudioManager.Instance.PlaySFX(Constants.SFX_BTN_CLICKED);
        }
    }

}
