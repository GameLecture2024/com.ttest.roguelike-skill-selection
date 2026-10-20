using System;
using System.Collections.Generic;
using TTest.RoguelikeSkillSelection.Data;
using TTest.UnityUtilities.Pooling;
using UnityEngine;

namespace TTest.RoguelikeSkillSelection.UI
{
    public sealed class RoguelikeSkillSelectionPanel : MonoBehaviour
    {
        [SerializeField] private GameObject panelRoot;
        [SerializeField] private Transform contentRoot;
        [SerializeField] private SkillChoiceItemView itemPrefab;
        [SerializeField] private bool closeAfterSelection = true;

        private readonly List<SkillChoiceItemView> _activeItems = new();
        private GameObjectPool _itemPool;
        private Action<SkillChoiceDefinition> _onChoiceSelected;

        private void Awake()
        {
            if (panelRoot == null || contentRoot == null || itemPrefab == null)
                throw new MissingReferenceException("Assign Panel Root, Content Root, and Item Prefab.");

            _itemPool = new GameObjectPool(itemPrefab.gameObject, $"{name} Item Pool", transform);
            panelRoot.SetActive(false);
        }

        private void OnDestroy() => _itemPool?.Dispose();

        public void Present(IReadOnlyList<SkillChoiceDefinition> choices, Action<SkillChoiceDefinition> onChoiceSelected)
        {
            if (choices == null)
                throw new ArgumentNullException(nameof(choices));

            ClearItems();
            _onChoiceSelected = onChoiceSelected;
            panelRoot.SetActive(choices.Count > 0);

            for (var index = 0; index < choices.Count; index++)
            {
                var choice = choices[index];
                if (choice == null)
                    continue;

                var item = _itemPool.Spawn(contentRoot).GetComponent<SkillChoiceItemView>();
                item.Bind(choice, Select);
                _activeItems.Add(item);
            }
        }

        public void Close()
        {
            ClearItems();
            _onChoiceSelected = null;
            panelRoot.SetActive(false);
        }

        private void Select(SkillChoiceDefinition choice)
        {
            _onChoiceSelected?.Invoke(choice);
            if (closeAfterSelection)
                Close();
        }

        private void ClearItems()
        {
            foreach (var item in _activeItems)
            {
                item.Clear();
                _itemPool.Despawn(item.gameObject);
            }

            _activeItems.Clear();
        }
    }
}
