using DG.Tweening;
using Patterns;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.CameraControl
{
    public class CameraController : MonoBehaviour
    {
        public Vector3 _rotationWhenInitGame;
        public Vector3 _rotationWhenPlayGame;


        private void Start()
        {
            transform.rotation = Quaternion.Euler(_rotationWhenInitGame);
            OnStartGameplay();
        }

        private void OnEnable()
        {
            this.PubSubRegister(EventID.OnFinishLevel, OnFinishLevel);
            this.PubSubRegister(EventID.OnStartGameplay, OnStartGameplay);
        }

        private void OnDisable()
        {
            this.PubSubUnregister(EventID.OnFinishLevel, OnFinishLevel);
            this.PubSubUnregister(EventID.OnStartGameplay, OnStartGameplay);
        }

        private void OnStartGameplay(object obj = null)
        {
            transform.DORotate(_rotationWhenPlayGame, 1f).SetEase(Ease.InOutExpo);
        }

        private void OnFinishLevel(object obj)
        {
            transform.DORotate(_rotationWhenInitGame, 1f).SetEase(Ease.InOutExpo);
        }
    }
}

