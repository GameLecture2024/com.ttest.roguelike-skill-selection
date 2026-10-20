using System;
using TTest.RoguelikeSkillSelection.Data;
using UnityEngine;
using UnityEngine.UI;

namespace TTest.RoguelikeSkillSelection.UI
{
    public sealed class SkillChoiceItemView : MonoBehaviour
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private Text titleText;
        [SerializeField] private Text descriptionText;

        private SkillChoiceDefinition _choice;
        private Action<SkillChoiceDefinition> _onSelected;

        private void Awake() => selectButton.onClick.AddListener(Select);
        private void OnDestroy() => selectButton.onClick.RemoveListener(Select);

        public void Bind(SkillChoiceDefinition choice, Action<SkillChoiceDefinition> onSelected)
        {
            _choice = choice ?? throw new ArgumentNullException(nameof(choice));
            _onSelected = onSelected;
            titleText.text = choice.DisplayName;
            descriptionText.text = choice.Description;
            iconImage.sprite = choice.Icon;
            iconImage.enabled = choice.Icon != null;
        }

        public void Clear()
        {
            _choice = null;
            _onSelected = null;
        }

        private void Select()
        {
            if (_choice != null)
                _onSelected?.Invoke(_choice);
        }
    }
}
