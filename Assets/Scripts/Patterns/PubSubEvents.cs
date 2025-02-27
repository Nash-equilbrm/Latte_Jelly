using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Patterns
{
    public enum EventID
    {
        OnInitLevel,

        OnSetBlockToSlot,
        OnDropToSlot,
        OnBlockSelected,
        OnBlockHovering,
        OnDestroyJelly,
        OnUIUpdateScore,
        OnFinishLevel,
        OnCleanupLevel,
        OnStartGameplay,
        OnFinishCleanupGridSpawner,
        OnFinishCleanupBlockSpawner,
        OnReplayBtnClicked,
        OnNoMoreMove,
    }
}
