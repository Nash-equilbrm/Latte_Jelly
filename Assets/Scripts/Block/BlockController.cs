using DG.Tweening;
using Game.Config;
using Game.Level;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Block
{
    public class BlockController : MonoBehaviour
    {
        [SerializeField] private GameObject subBlockPrefab;
        [SerializeField] private List<SubBlock> _subBlocks = new List<SubBlock>();
        [SerializeField] private JellyColor[,] _colors;

        [field: SerializeField] public int Height { get; private set; }
        [field: SerializeField] public int Width { get; private set; }
        public JellyColor[,] Colors { get => _colors; set => _colors = value; }

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

        #region Testing
        [Header("Testing")]
        public SubBlock toRemove;
        [Button]
        public void TestRemoveBlock()
        {
            RemoveSubBlock(toRemove);
        }
        [Button]
        public void GenerateRandomSubBlocks()
        {
            if (_subBlocks == null)
                _subBlocks = new List<SubBlock>();

            foreach (var sub in _subBlocks)
            {
                Destroy(sub.gameObject);
            }
            _subBlocks.Clear();
            ResetColors();

            int maxSubBlocks = Random.Range(3, 6);
            int attempts = 0, maxAttempts = 100;

            List<JellyColor> availableColors = new List<JellyColor> { JellyColor.Cyan, JellyColor.Purple, JellyColor.Green, JellyColor.Yellow };
            availableColors = availableColors.OrderBy(_ => Random.value).ToList();

            for (int i = 0; i < maxSubBlocks; i++)
            {
                if (i >= availableColors.Count) break;

                int x = 0, y = 0, width = 1, height = 1;
                bool validPosition = false;

                while (!validPosition && attempts < maxAttempts)
                {
                    attempts++;
                    x = Random.Range(0, Width - 1);
                    y = Random.Range(0, Height - 1);
                    width = Random.Range(1, 3);
                    height = Random.Range(1, 3);

                    if (x + width > Width || y + height > Height) continue;

                    validPosition = true;
                    for (int r = y; r < y + height; r++)
                    {
                        for (int c = x; c < x + width; c++)
                        {
                            if (_colors[r, c] != JellyColor.None)
                            {
                                validPosition = false;
                                break;
                            }
                        }
                        if (!validPosition) break;
                    }
                }

                if (!validPosition) break;

                JellyColor color = availableColors[i];

                GameObject subBlockObj = Instantiate(subBlockPrefab, transform);
                SubBlock subBlock = subBlockObj.GetComponent<SubBlock>();
                subBlock.Controller = this;
                subBlock.X = x;
                subBlock.Y = y;
                subBlock.Size = new Vector2(width, height);
                subBlock.JellyColor = color;
                subBlock.gameObject.name = color.ToString();
                _subBlocks.Add(subBlock);

                for (int r = y; r < y + height; r++)
                {
                    for (int c = x; c < x + width; c++)
                    {
                        _colors[r, c] = color;
                    }
                }
            }
        }
        #endregion

        private void RemoveSubBlock(SubBlock block)
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
                Destroy(block.gameObject);

                ExpandAllBlocks();
            }
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
            List<SubBlock> horizontalFirst = _subBlocks
                .OrderByDescending(sub => sub.CanExpandHorizontal(-1) || sub.CanExpandHorizontal(1)) // ✅ Prioritize horizontal expansion
                .ToList();

            foreach (var sub in horizontalFirst)
            {
                sub.Expand();
            }
        }


        [Button]
        public void AssignUniqueColorsToSubBlocks()
        {
            if (_subBlocks.Count == 0) return;

            List<JellyColor> availableColors = new List<JellyColor>
            {
                JellyColor.Cyan, JellyColor.Purple, JellyColor.Green, JellyColor.Yellow
            };

            availableColors = availableColors.OrderBy(_ => Random.value).ToList();

            for (int i = 0; i < _subBlocks.Count; i++)
            {
                if (i >= availableColors.Count)
                {
                    availableColors = availableColors.OrderBy(_ => Random.value).ToList();
                }

                _subBlocks[i].JellyColor = availableColors[i % availableColors.Count];
                _subBlocks[i].UpdateColor();

                _subBlocks[i].Controller = this;
                for (int k = _subBlocks[i].Y; k < _subBlocks[i].Y + (int)_subBlocks[i].Size.y; k++)
                {
                    for (int j = _subBlocks[i].X; j < _subBlocks[i].X + (int)_subBlocks[i].Size.x; j++)
                    {
                        _colors[k, j] = _subBlocks[i].JellyColor;
                    }
                }
            }
        }

    }
}
