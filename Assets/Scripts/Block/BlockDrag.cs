using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Game.Block;
using Patterns;

namespace Game.Map
{
    public class DraggableObject : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        private Vector3 _screenPoint;
        private Vector3 _offset;
        private bool _isDragging = false;
        private int _originalLayer;
        public string DragLayerName = "Ignore Raycast";
        public float DragSpeed = 10f;

        public float DragDistance = 0f;

        private Vector3 _targetPosition;

        private Slot _currentSlot;

        public void OnPointerDown(PointerEventData eventData)
        {
            BeginDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EndDrag(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;
            
            Vector3 curScreenPoint = new Vector3(eventData.position.x, eventData.position.y, _screenPoint.z);
            Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + _offset;

            _targetPosition = curPosition;

            DetectSlotHover(eventData);
        }

        private void BeginDrag(PointerEventData eventData)
        {
            _isDragging = true;

            _screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            _offset = gameObject.transform.position -
                      Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y,
                          _screenPoint.z));

            if (DragDistance <= 0f)
            {
                DragDistance = Vector3.Distance(transform.position, Camera.main.transform.position);
            }

            _originalLayer = gameObject.layer;
            gameObject.layer = LayerMask.NameToLayer(DragLayerName);
        }

        private void EndDrag(PointerEventData eventData)
        {
            _isDragging = false;
            gameObject.layer = _originalLayer;

            if (_currentSlot != null)
            {
                transform.SetParent(_currentSlot.container);
                transform.localPosition = Vector3.zero;
                this.PubSubBroadcast(EventID.OnDropCurrentBlock, gameObject.GetComponent<BlockController>());
            }
            else
            {
                transform.localPosition = Vector3.zero;
            }
            

            if (_currentSlot != null)
            {
                this.PubSubBroadcast(EventID.OnEndHoverOnSlot, _currentSlot);
                _currentSlot = null;
            }

        }


        private void Start()
        {
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem =
                    new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }

            if (string.IsNullOrEmpty(DragLayerName))
            {
                Debug.LogError("DragLayerName is not set! Please set it in the Inspector.");
            }

            if (LayerMask.NameToLayer(DragLayerName) == -1)
            {
                Debug.LogError(
                    $"Layer '{DragLayerName}' does not exist.  Please create it in Edit > Project Settings > Tags and Layers.");
            }
        }

        private void Update()
        {
            if (_isDragging)
            {
                transform.position = Vector3.Lerp(transform.position, _targetPosition, DragSpeed * Time.deltaTime);
            }
        }

        private void DetectSlotHover(PointerEventData eventData)
        {
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventData, results);

            Slot newSlot = null;

            foreach (RaycastResult result in results)
            {
                Slot slot = result.gameObject.GetComponent<Slot>();
                if (slot != null)
                {
                    newSlot = slot;
                    break;
                }
            }

            if (newSlot != _currentSlot)
            {
                if (_currentSlot != null)
                {
                    this.PubSubBroadcast(EventID.OnEndHoverOnSlot, _currentSlot);
                }

                if (newSlot != null)
                {
                    this.PubSubBroadcast(EventID.OnStartHoverOnSlot, newSlot);
                }

                _currentSlot = newSlot;
            }

        }
    }
}