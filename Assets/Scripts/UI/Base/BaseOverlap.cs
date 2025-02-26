using UnityEngine;

namespace UI
{
    public class BaseOverlap : BaseUIElement
    {
        public override void Hide()
        {
            base.Hide();
        }

        public override void Init()
        {
            base.Init();
            uiType = UIType.Overlap;

            RectTransform rectTransform = GetComponent<RectTransform>();
            rectTransform.offsetMin = Vector3.zero;
            rectTransform.offsetMax = Vector3.zero;
        }

        public override void Show(object data)
        {
            base.Show(data);
        }
    }
}