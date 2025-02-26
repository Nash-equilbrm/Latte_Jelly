using Commons;
using Game.Level;
using System.Collections;
using System.Collections.Generic;
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
            base.Hide();
            foreach (var item in _jellyScoreUI)
            {
                Destroy(item);
            }
            _jellyScoreUI.Clear();
            _uiDict.Clear();
            jellyScoreLayout.gameObject.SetActive(false);
        }

        public override void Init()
        {
            base.Init();
        }

        public override void Show(object data)
        {
            base.Show(data);
            if (data is not LevelConfig config) return;

            foreach(var requirement in config.levelRequirements)
            {
                JellyColor color = requirement.jellyColor;
                var jellyUI = Instantiate(jellyScoreTemplate, jellyScoreTemplate.transform.parent);
                jellyUI.GetComponent<UnityEngine.UI.Image>().color = Common.GetColorFromJelly(color);
                _uiDict.Add(color, jellyUI.GetComponentInChildren<TMP_Text>());
                _uiDict[color].text = requirement.amount.ToString();
                jellyUI.SetActive(true);
            }
            jellyScoreLayout.gameObject.SetActive(true);
        }
    }
}

