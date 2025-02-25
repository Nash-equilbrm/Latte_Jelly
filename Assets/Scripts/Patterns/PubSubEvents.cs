using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public enum EventID
    {
        OnInitGame,
        OnStartInitGame,
        OnSpawnedGameobjects,
        OnGameStartCounting,
        OnFinishCounting,
        OnStartGameplay,
        OnHitFinishLine,
        OnFinishGame,
        OnReplayBtnClicked,
        OnHitCheckpoint,
        OnCounting,
    }
}
