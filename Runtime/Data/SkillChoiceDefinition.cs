using UnityEngine;

namespace TTest.RoguelikeSkillSelection.Data
{
    [CreateAssetMenu(menuName = "TTest/Roguelike/Skill Choice", fileName = "SkillChoice")]
    public sealed class SkillChoiceDefinition : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string displayName;
        [TextArea(2, 5)] [SerializeField] private string description;
        [SerializeField] private Sprite icon;

        public string Id => id;
        public string DisplayName => displayName;
        public string Description => description;
        public Sprite Icon => icon;

        private void OnValidate()
        {
            id = id?.Trim();
            displayName = displayName?.Trim();
        }
    }
}
