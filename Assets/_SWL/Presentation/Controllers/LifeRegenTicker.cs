using UnityEngine;
using SWL.App.Ports;
using SWL.App.UseCases;
using SWL.Presentation.UI.Widgets;
using SWL.App;
using SWL.Core.Domain.Player;
using System;

namespace SWL.Presentation.Controllers
{
    public sealed class LifeRegenTicker : MonoBehaviour
    {
        [SerializeField] private HUDView hudView;

        private TickLifeRegenUseCase _tick;
        private ITimeService _time;
        private long _lastSecond = -1;
        private PlayerProfileStore _store;

        /// <summary>
        /// Constructs this object.
        /// </summary>
        /// <param name="store">The player profile store.</param>
        /// <param name="tick">The tick life regen use case.</param>
        /// <param name="time">The time service.</param>
        /// <param name="hudView">The HUD view.</param>
        public void Construct(PlayerProfileStore store, TickLifeRegenUseCase tick, ITimeService time)
        {
            _store = store;
            _tick = tick;
            _time = time;
        }         
        
        private void Update()
        {
            if (_tick == null || _time == null || _store == null) return;

            var now = _time.UtcNowUnixSeconds;
            if (now == _lastSecond) return;
            _lastSecond = now;

            // 1) state update
            _tick.Tick(now);

            // 2) timer UI update (remaining time)
            if (hudView != null)
                UpdateTimerText(now);
        }

        private void UpdateTimerText(long now)
        {
            var p = _store.Profile;

            // hide timer if max life
            if (p.Life >= LifeRules.MaxLife)
            {
                hudView.SetLifeTimer("MAX");
                return;
            }

            // if timer is not initialized
            if (p.NextLifeRegenUnix <= 0)
            {
                hudView.SetLifeTimer("--:--");
                return;
            }

            var remaining = p.NextLifeRegenUnix - now;

            if (remaining <= 0)
            {
                // Regen tick will increase life a bit; let UI be stable “00:00”
                hudView.SetLifeTimer("00:00");
                return;
            }

            hudView.SetLifeTimer(FormatMmSs(remaining));
        }

        private static string FormatMmSs(long seconds)
        {
            // safety
            if (seconds < 0) seconds = 0;

            var ts = TimeSpan.FromSeconds(seconds);
            // for 30mn mm:ss is en
            var totalMinutes = (int)ts.TotalMinutes;
            return $"{totalMinutes:00}:{ts.Seconds:00}";
        }
    }
}

