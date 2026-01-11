using UnityEngine;
using SWL.App.Ports;
using SWL.App.UseCases;

namespace SWL.Presentation.Controllers
{
    public sealed class LifeRegenTicker : MonoBehaviour
    {
        private TickLifeRegenUseCase _tick;
        private ITimeService _time;

        private float _acc;

        public void Construct(TickLifeRegenUseCase tick, ITimeService time)
        {
            _tick = tick;
            _time = time;
        }

        private void Update()
        {
            if (_tick == null || _time == null) return;

            _acc += Time.unscaledDeltaTime;
            if (_acc < 1f) return; // 1 sn'de bir yeter
            _acc = 0f;

            _tick.Tick(_time.UtcNowUnixSeconds);
        }
    }
}
