using System;
using System.Collections.Generic;
using UnityEngine;

namespace SWL.Presentation.UI.Navigation
{
    public sealed class TabRouter : MonoBehaviour
    {
        [Serializable]
        public struct TabBinding
        {
            public TabId tabId;
            public ScreenView screen;
        }

        [SerializeField] private TabBinding[] tabs;
        [SerializeField] private TabId initialTab = TabId.Home;

        private readonly Dictionary<TabId, ScreenView> _map = new();
        private TabId _current;

        private void Awake()
        {
            _map.Clear();
            foreach (var t in tabs)
            {
                if (t.screen == null) continue;
                _map[t.tabId] = t.screen;
            }

            // start: hide all
            foreach (var kv in _map)
                kv.Value.Hide();

            Show(initialTab);
        }

        public void Show(TabId tab)
        {
            if (_map.Count == 0) return;

            if (_map.TryGetValue(_current, out var cur))
                cur.Hide();

            _current = tab;

            if (_map.TryGetValue(_current, out var next))
                next.Show();
        }
    }
}
