using System.Collections.Generic;
using TTest.RoguelikeSkillSelection.Data;
using TTest.RoguelikeSkillSelection.UI;
using UnityEngine;
using UnityEngine.UI;

namespace TTest.RoguelikeSkillSelection.Samples.SkillSelectionDemo
{
    public sealed class SkillSelectionDemoController : MonoBehaviour
    {
        [SerializeField] private RoguelikeSkillSelectionPanel selectionPanel;
        [SerializeField] private SkillSelectionDemoInput input;
        [SerializeField] private List<SkillChoiceDefinition> choices = new();
        [SerializeField] private Text resultText;

        private void OnEnable()
        {
            if (input != null)
                input.ShowChoicesRequested += ShowChoices;
        }

        private void OnDisable()
        {
            if (input != null)
                input.ShowChoicesRequested -= ShowChoices;
        }

        private void Start() => ShowChoices();

        public void ShowChoices()
        {
            selectionPanel.Present(choices, OnChoiceSelected);
        }

        private void OnChoiceSelected(SkillChoiceDefinition choice)
        {
            resultText.text = $"Selected: {choice.DisplayName}";
            Debug.Log($"Selected skill: {choice.Id}", this);
        }
    }
}
