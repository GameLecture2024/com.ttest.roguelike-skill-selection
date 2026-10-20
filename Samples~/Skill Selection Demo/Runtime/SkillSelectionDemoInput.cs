using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TTest.RoguelikeSkillSelection.Samples.SkillSelectionDemo
{
    /// <summary>New Input System bindings used only by the imported sample.</summary>
    public sealed class SkillSelectionDemoInput : MonoBehaviour
    {
        [SerializeField] private Button showChoicesButton;

        private InputAction _showChoicesAction;

        public event Action ShowChoicesRequested;

        private void Awake()
        {
            _showChoicesAction = new InputAction("Show Choices", InputActionType.Button);
            _showChoicesAction.AddBinding("<Keyboard>/r");
            _showChoicesAction.AddBinding("<Gamepad>/start");
            if (showChoicesButton != null)
                showChoicesButton.onClick.AddListener(RequestShowChoices);
        }

        private void OnEnable()
        {
            _showChoicesAction.performed += OnShowChoices;
            _showChoicesAction.Enable();
        }

        private void OnDisable()
        {
            _showChoicesAction.performed -= OnShowChoices;
            _showChoicesAction.Disable();
        }

        private void OnDestroy()
        {
            if (showChoicesButton != null)
                showChoicesButton.onClick.RemoveListener(RequestShowChoices);
            _showChoicesAction.Dispose();
        }

        private void OnShowChoices(InputAction.CallbackContext context) => RequestShowChoices();

        private void RequestShowChoices() => ShowChoicesRequested?.Invoke();
    }
}
