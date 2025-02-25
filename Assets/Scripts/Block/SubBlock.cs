using UnityEngine;
using System.Linq;
using DG.Tweening;
using NaughtyAttributes;
using Game.Level;
using Commons;

namespace Game.Block
{
    public class SubBlock : MonoBehaviour
    {
        [field: SerializeField] public BlockController Controller { get; set; }
        [field: SerializeField] public JellyColor JellyColor { get; set; }

        [field: SerializeField] public Vector2 Size { get; set; }
        [field: SerializeField] public int X { get; set; }
        [field: SerializeField] public int Y { get; set; }

        [SerializeField] private GameObject _cube;
        [SerializeField] private Renderer _cubeRenderer;

        private void Start()
        {
            CreateCube();
        }

        private void CreateCube()
        {
            UpdateCubeTransform();
            _cube.gameObject.SetActive(true);
            //_cubeRenderer.material.color = GetColorFromJelly(JellyColor);
            UpdateColor();
            _cube.transform.SetParent(transform);
        }

        private void UpdateCubeTransform()
        {
            float centerX = X + (Size.x / 2f);
            float centerZ = -(Y + (Size.y / 2f));

            _cube.transform.localPosition = new Vector3(centerX, 0, centerZ);
            _cube.transform.localScale = new Vector3(Size.x, 1, Size.y);
        }

        public void Expand()
        {
            int height = Controller.Colors.GetLength(0);
            int width = Controller.Colors.GetLength(1);

            bool expanded = true;
            int maxIterations = height * width;
            int iterations = 0;

            while (expanded && iterations < maxIterations)
            {
                iterations++;
                expanded = false;

                bool canExpandUp = Y > 0 && CanExpandVertical(-1);
                bool canExpandDown = Y + (int)Size.y < height && CanExpandVertical(1);
                bool canExpandLeft = X > 0 && CanExpandHorizontal(-1);
                bool canExpandRight = X + (int)Size.x < width && CanExpandHorizontal(1);

                if (canExpandUp)
                {
                    Y--;
                    Size = new Vector2(Size.x, Size.y + 1);
                    expanded = true;
                }
                if (canExpandDown)
                {
                    Size = new Vector2(Size.x, Size.y + 1);
                    expanded = true;
                }
                if (canExpandLeft)
                {
                    X--;
                    Size = new Vector2(Size.x + 1, Size.y);
                    expanded = true;
                }
                if (canExpandRight)
                {
                    Size = new Vector2(Size.x + 1, Size.y);
                    expanded = true;
                }

                if (!expanded) break;

                for (int i = Y; i < Y + (int)Size.y; i++)
                {
                    for (int j = X; j < X + (int)Size.x; j++)
                    {
                        Controller.Colors[i, j] = JellyColor;
                    }
                }
            }

            float newCenterX = X + (Size.x / 2f);
            float newCenterZ = -(Y + (Size.y / 2f));

            _cube.transform.DOScale(new Vector3(Size.x, 1, Size.y), 0.2f).SetEase(Ease.OutBack);
            _cube.transform.DOLocalMove(new Vector3(newCenterX, 0, newCenterZ), 0.2f).SetEase(Ease.OutBack);
        }


        public bool CanExpandHorizontal(int dir)
        {
            int newCol = dir == -1 ? X - 1 : X + (int)Size.x;
            if (newCol < 0 || newCol >= Controller.Colors.GetLength(1)) return false;

            for (int i = Y; i < Y + (int)Size.y; i++)
            {
                if (Controller.Colors[i, newCol] != JellyColor.None)
                    return false;
            }

            return true;
        }

        public bool CanExpandVertical(int dir)
        {
            int newRow = dir == -1 ? Y - 1 : Y + (int)Size.y;
            if (newRow < 0 || newRow >= Controller.Colors.GetLength(0)) return false;

            for (int j = X; j < X + (int)Size.x; j++)
            {
                if (Controller.Colors[newRow, j] != JellyColor.None)
                    return false;
            }

            return true;
        }

       

        public void DestroyCube()
        {
            if (_cube != null)
            {
                _cube.transform.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(_cube);
                });
            }
        }

        public void UpdateColor()
        {
            if (_cubeRenderer != null)
            {
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                _cubeRenderer.GetPropertyBlock(block);
                block.SetColor("_BaseColor", Common.GetColorFromJelly(JellyColor));
                _cubeRenderer.SetPropertyBlock(block);
                gameObject.name = JellyColor.ToString();
            }
        }
    }
}
