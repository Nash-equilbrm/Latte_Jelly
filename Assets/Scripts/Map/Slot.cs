using DG.Tweening;
using Patterns;
using UnityEngine;


namespace Game.Map
{
    public class Slot : MonoBehaviour
    {
        public Transform container;
        public SpriteRenderer outline;

        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnStartHoverOnSlot, OnStartHoverOnSlot);
            this.PubSubRegister(EventID.OnEndHoverOnSlot, OnEndHoverOnSlot);
        }


        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnStartHoverOnSlot, OnStartHoverOnSlot);
            this.PubSubUnregister(EventID.OnEndHoverOnSlot, OnEndHoverOnSlot);
        }

        private void OnEndHoverOnSlot(object obj)
        {
            if (obj is not Slot slot || slot != this) return;
            OnSlotHoverEnd();
        }

        private void OnStartHoverOnSlot(object obj)
        {
            if (obj is not Slot slot || slot != this) return;
            OnSlotHover();
        }

        public void OnSlotHover()
        {
            outline.DOFade(1f, 0.3f).SetEase(Ease.OutCirc);
        }

        public void OnSlotHoverEnd()
        {
            Debug.Log("Exited hover on slot: " + gameObject.name);
            outline.DOFade(0.3f, 0.3f).SetEase(Ease.OutCirc);
        }
    }
}

