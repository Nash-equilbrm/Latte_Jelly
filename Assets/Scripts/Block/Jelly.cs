using UnityEngine;
using DG.Tweening;
using Game.Level;
using Commons;
using System;
using System.Collections.Generic;
using Patterns;
using Game.Audio;
using Game.Config;

namespace Game.Block
{
    public class Jelly : MonoBehaviour
    {
        [field: SerializeField] public BlockController Controller { get; set; }
        [field: SerializeField] public JellyColor JellyColor { get; set; }

        [field: SerializeField] public Vector2 Size { get; set; }
        [field: SerializeField] public int X { get; set; }
        [field: SerializeField] public int Y { get; set; }

        [SerializeField] private GameObject _cube;
        [SerializeField] private Renderer _cubeRenderer;
        public LayerMask jellyLayerMask;
        private float _raycastDistance = 1f;

        public bool Expanding { get; set; }

        private void Start()
        {
            CreateCube();
        }

        private void CreateCube()
        {
            UpdateCubeTransform();
            _cube.gameObject.SetActive(true);
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
            Expanding = true;
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

            _cube.transform.DOScale(new Vector3(Size.x, 1, Size.y), 1f).SetEase(Ease.InOutExpo);
            _cube.transform.DOLocalMove(new Vector3(newCenterX, 0, newCenterZ), 1f).SetEase(Ease.InOutExpo).OnComplete(() =>
            {
                Expanding = false;
            });
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
                var self = this;
                DOTween.Sequence()
                    .Join(_cube.transform.DOLocalMove(_cube.transform.localPosition + new Vector3(0f, 1f, 1f), 1f).SetEase(Ease.InOutExpo))
                    .Join(_cube.transform.DOScale(Vector3.one * 0.1f, 1f).SetEase(Ease.InOutExpo))
                    .OnComplete(() =>
                    {
                        //Destroy(_cube);
                        self.PubSubBroadcast(EventID.OnDestroyJelly, JellyColor);

                        AudioManager.Instance.PlaySFX(Constants.SFX_JELLY_POP);

                        var vfxObj = ObjectPooling.Instance.GetPool(Constants.VFX_JELLY_POP).Get(position: _cube.transform.position);
                        var vfx = vfxObj.GetComponent<ParticleSystem>();
                        var main = vfx.main;
                        main.startColor = Common.GetColorFromJelly(JellyColor);
                        vfx.Play();
                        Destroy(gameObject);
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


        public void RemoveSelf()
        {
            Controller.RemoveSubBlock(this);
        }

        internal bool CheckRaycast()
        {
            string jellyTag = gameObject.tag;
            List<Jelly> matchingJellies = new List<Jelly>();
            var raycastPositions = GetRaycastPositions();
            Vector3[] directions = { Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
            foreach (var pos in raycastPositions) {
                var positionToRaycast = transform.position + new Vector3(pos.Item1, 0f, pos.Item2);
                for (int i = 0; i < directions.Length; i++)
                {
                    Debug.DrawRay(positionToRaycast, directions[i] * _raycastDistance, Color.red, 1f);

                    if (Physics.Raycast(positionToRaycast, directions[i], out RaycastHit hit, _raycastDistance, jellyLayerMask))
                    {
                        
                        if (hit.collider.gameObject != gameObject 
                            && hit.collider.transform.parent != transform.parent
                            && hit.collider.CompareTag(jellyTag))
                        {
                            Jelly otherJelly = hit.collider.transform.parent.parent.GetComponent<Jelly>();
                            if (otherJelly != null && otherJelly.JellyColor == JellyColor)
                            {
                                matchingJellies.Add(otherJelly);
                            }
                        }
                    }
                }
            }
            

            if (matchingJellies.Count > 0)
            {
                foreach (Jelly jelly in matchingJellies)
                {
                    if (jelly != null)
                    {
                        jelly.RemoveSelf();
                    }
                }
                RemoveSelf();
                return true;
            }

            return false;
        }


        private List<(float, float)> GetRaycastPositions()
        {
            List<(float, float)> res = new List<(float, float)>();
            for(int i = 1; i <= Size.x; ++i)
            {
                for (int j = 1; j <= Size.y; ++j)
                {
                    (float, float) coor = (X + i - 1, -(Y + j - 1));
                    coor.Item1 += 0.5f;
                    coor.Item2 -= 0.5f;
                    res.Add(coor);
                }
            }
            return res;
        }
    }
}
