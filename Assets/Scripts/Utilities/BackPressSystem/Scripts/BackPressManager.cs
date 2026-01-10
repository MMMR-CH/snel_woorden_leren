using System.Collections.Generic;
using UnityEngine;

namespace MC.Utility.BackPressSystem
{
    public class BackPressManager : SingletonBehaviour<BackPressManager>
    {
        private readonly Stack<IBackPressable> _backPressStack = new();
        private readonly List<BackPressManipulator> _backPressManipulators = new();

        private float _lastBackPressedTime;
        private const float BackPressCooldownDuration = 0.3f;

        private bool HasBackPressCooldown => Time.time < _lastBackPressedTime + BackPressCooldownDuration;

        public bool IsBackPressBlocked => _backPressManipulators.Count > 0;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnBackButtonPressed();
            }
        }

        private void OnBackButtonPressed()
        {
            Debug.Log("Back Button Pressed");
            if (HasBackPressCooldown)
            {
                Debug.LogWarning($"Back press is not working!: {nameof(HasBackPressCooldown)}");
                return;
            }

            if ((IsBackPressBlocked))
            {
                Debug.LogWarning($"Back press is blocked");
                return;
            }

            if (_backPressStack.Count <= 0) return;

            var topItem = _backPressStack.Pop();
            topItem?.OnBackPress();
            Debug.Log($"Pop item by back press : {topItem}");

            _lastBackPressedTime = Time.time;
        }

        public void AddManipulator(BackPressManipulator manipulator)
        {
            _backPressManipulators.Add(manipulator);
        }

        public void RemoveManipulator(BackPressManipulator manipulator)
        {
            if (manipulator.IsSuperManipulator)
            {
                _backPressManipulators.Clear();
            }
            else
            {
                _backPressManipulators.Remove(manipulator);
            }
        }

        public void AddToBackPressStack(IBackPressable backPressable)
        {
            if (_backPressStack.Contains(backPressable))
            {
                // Debug.LogError($"Stack already contains {backPressable}, so do not push!");
                return;
            }

            _backPressStack.Push(backPressable);
            foreach (var item in _backPressStack)
            {
                Debug.LogWarning(item);
            }
        }

        public void PopDesiredItemIfPossible(IBackPressable backPressable)
        {
            if (_backPressStack.Count <= 0)
            {
                return;
            }

            var topItem = _backPressStack.Peek();
            if (topItem != backPressable)
            {
                // Debug.LogError("Desired item is different than top item, so do not pop!");
                return;
            }

            _backPressStack.Pop();
            // Debug.Log($"Pop item by method : {topItem}");
        }
    }
}