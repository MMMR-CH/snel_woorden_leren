using System;
using System.Collections;
using UnityEngine;
using SWL.Core.Domain.Levels;

namespace SWL.Features.Levels.Runners
{
    public sealed class MatchWordsImagesRunner : MonoBehaviour, SWL.Features.Levels.ILevelRunner
    {
        public event Action<LevelResult> Finished;
        private Coroutine _co;

        public void StartLevel(LevelSpec spec)
        {
            _co = StartCoroutine(FinishSuccess());
        }

        private IEnumerator FinishSuccess()
        {
            yield return new WaitForSeconds(1.0f);
            Finished?.Invoke(new LevelResult(true, coinsReward: 10, gemsReward: 0));
        }

        public void Dispose()
        {
            if (_co != null) StopCoroutine(_co);
            _co = null;
        }
    }
}
