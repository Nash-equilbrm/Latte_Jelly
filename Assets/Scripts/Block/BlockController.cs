using Commons;
using DG.Tweening;
using Game.Config;
using Game.Level;
using Game.Map;
using NaughtyAttributes;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UserInput;

namespace Game.Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private List<Jelly> _subBlocks = new List<Jelly>();
        [SerializeField] private JellyColor[,] _colors;
        [SerializeField] private float _speed = 20f;
        [SerializeField] private string _blockTypeTag;
        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public int Width { get; private set; }
        public LayerMask jellyLayerMask;
        public JellyColor[,] Colors { get => _colors; set => _colors = value; }

        private bool _followPlayerInput = false;
        [field: SerializeField] public bool DroppedToSlot { get; set; } = false;
        public Slot Slot { get; set; }
        public List<Jelly> SubBlocks { get => _subBlocks; }

        private void Start()
        {
            _colors = new JellyColor[Constants.MAX_ROWS, Constants.MAX_COLUMNS];
            (Height, Width) = (Constants.MAX_ROWS, Constants.MAX_COLUMNS);

            if (_subBlocks.Count > 0)
            {
                AssignUniqueColorsToSubBlocks();
            }
            else
            {
                ResetColors();
            }
        }


        private void OnEnable()
        {
            InputReader.Instance.OnDragStart += OnDragStart;
        }


        private void OnDisable()
        {
            UnregisterEvents();
        }

        private void OnDestroy()
        {
            UnregisterEvents();
        }

        private void UnregisterEvents()
        {
            InputReader.Instance.OnDragStart -= OnDragStart;
            InputReader.Instance.OnDragging -= OnDragging;
            InputReader.Instance.OnDragEnd -= OnDragEnd;
        }


        private void OnDragStart(Vector2 screenPos)
        {
            if (DroppedToSlot) return;
            var pivot = transform.position;
            pivot.x += 1;
            pivot.z -= 1;
            var slotScreenPosVec3 = Camera.main.WorldToScreenPoint(pivot);
            var onScreenPosition = (Vector2)slotScreenPosVec3;

            if(Vector3.SqrMagnitude(screenPos - onScreenPosition) < 90 * 90)
            {
                _followPlayerInput = true;
                InputReader.Instance.OnDragging += OnDragging;
                InputReader.Instance.OnDragEnd += OnDragEnd;
                this.PubSubBroadcast(EventID.OnBlockSelected, this);
            }

        }

        private void OnDragging(Vector2 screenPos)
        {
            if (DroppedToSlot || !_followPlayerInput) return;
            var screenPosition = new Vector3(screenPos.x, screenPos.y, 5f);
            screenPosition.x += -100;
            screenPosition.y += 300;
            var targetPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            if (Vector3.SqrMagnitude(targetPosition - transform.position) <= 0.1f)
            {
                transform.position = targetPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, _speed * Time.deltaTime);
            }

            if (Physics.Raycast(transform.position + new Vector3(1f, 0f, -1f), Camera.main.transform.forward, out RaycastHit hit, 30f, ~jellyLayerMask))
            {
                if (!hit.collider) return;
                LogUtility.Info("BlockController.Raycast", hit.collider.tag);
                if (hit.collider.CompareTag(Constants.SLOT_TAG))
                {
                    this.PubSubBroadcast(EventID.OnBlockHovering, hit.collider);
                }
                else if (hit.collider.CompareTag(Constants.PLANE_TAG))
                {
                    this.PubSubBroadcast(EventID.OnBlockHovering, null);
                }

            }
        }

        private void OnDragEnd(Vector2 screenPos)
        {
            if (DroppedToSlot || !_followPlayerInput) return;
            //transform.localPosition = Vector3.zero;
            transform.DOLocalMove(Vector3.zero, .3f).SetEase(Ease.InOutExpo);
          
            _followPlayerInput = false;
            InputReader.Instance.OnDragging -= OnDragging;
            InputReader.Instance.OnDragEnd -= OnDragEnd;
        }




        public void RemoveSubBlock(Jelly block)
        {
            if (!_subBlocks.Contains(block)) return;
            if (block != null)
            {
                block.DestroyCube();

                for (int i = block.Y; i < block.Y + (int)block.Size.y; i++)
                {
                    for (int j = block.X; j < block.X + (int)block.Size.x; j++)
                    {
                        _colors[i, j] = JellyColor.None;
                    }
                }

                _subBlocks.Remove(block);
                ExpandAllBlocks();
            }

            if(_subBlocks.Count == 0)
            {
                StartCoroutine(IEWaitForJellyDestroyed(block, () =>
                {
                    if (Slot != null) Slot.CurrentBlock = null;
                    ObjectPooling.Instance.GetPool(_blockTypeTag).DestroyObject(gameObject);
                    LogUtility.Info("BlockController","Destroy block");
                }));
            }
        }


        private IEnumerator IEWaitForJellyDestroyed(Jelly jelly, Action callback)
        {
            yield return new WaitUntil(() => jelly == null);
            callback.Invoke();
        }

      
        private void ResetColors()
        {
            for (int i = 0; i < Height; ++i)
            {
                for (int j = 0; j < Width; ++j)
                {
                    _colors[i, j] = JellyColor.None;
                }
            }
        }

        private void ExpandAllBlocks()
        {
            List<Jelly> horizontalFirst = _subBlocks
                .OrderByDescending(sub => sub.CanExpandHorizontal(-1) || sub.CanExpandHorizontal(1))
                .ToList();

            foreach (var sub in horizontalFirst)
            {
                sub.Expand();
            }
            StartCoroutine(IEWaitForFinishExpanding());
        }


        [Button]
        public void AssignUniqueColorsToSubBlocks()
        {
            if (_subBlocks.Count == 0) return;

            List<JellyColor> availableColors = GameManager.Instance.CurrentConfig.colors.ToList();

            availableColors = availableColors.OrderBy(_ => UnityEngine.Random.value).ToList();
           
            for (int i = 0; i < _subBlocks.Count; i++)
            {
                _subBlocks[i].JellyColor = availableColors[i % availableColors.Count];
            }

            foreach (var subBlock in _subBlocks)
            {
                subBlock.UpdateColor();

                for (int k = subBlock.Y; k < subBlock.Y + (int)subBlock.Size.y; k++)
                {
                    for (int j = subBlock.X; j < subBlock.X + (int)subBlock.Size.x; j++)
                    {
                        _colors[k, j] = subBlock.JellyColor;
                    }
                }
            }
        }

        internal void CheckRaycast()
        {
            if(SubBlocks.Count == 0) return;
            foreach (var sb in _subBlocks)
            {
                if (sb.CheckRaycast())
                {
                    return;
                }
            }
        }

        private IEnumerator IEWaitForFinishExpanding()
        {
            yield return new WaitUntil(() =>
            {
                foreach (var jelly in _subBlocks)
                {
                    if(jelly.Expanding) return false;
                }
                return true;
            }
            );
            CheckRaycast();
        }


    }
}
