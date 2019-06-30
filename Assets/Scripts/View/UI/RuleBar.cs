using System;
using System.Collections.Generic;
using UnityEngine;

public class RuleBar : MonoBehaviour
{
    [System.Serializable]
    public class RuleBarItemWithGameRule
    {
        [SerializeField] private GameRule rule = default(GameRule);
        [SerializeField] private RuleBarItem ruleBarItemPrefab = null;

        public GameRule Rule => rule;
        public RuleBarItem RuleBarItemPrefab => ruleBarItemPrefab;

    }

    [SerializeField] private RuleDescriptionPanel ruleDescriptionPanel = null;
    [SerializeField] private RuleBarItemWithGameRule[] ruleBarItemWithGameRuleArray = null;

    private Dictionary<GameRule, RuleBarItem> ruleBarItemByRule;

    public void Init(GameRule _activeGameRules) {
        ruleBarItemByRule = new Dictionary<GameRule, RuleBarItem>();

        foreach (RuleBarItemWithGameRule ruleBarItemWithGameRuleItem in ruleBarItemWithGameRuleArray) {
            ruleBarItemByRule[ruleBarItemWithGameRuleItem.Rule] = ruleBarItemWithGameRuleItem.RuleBarItemPrefab;
        }

        if (_activeGameRules != GameRule.None) {
            gameObject.SetActive(true);

            // foreach (Transform childItem in transform) {
            //     Destroy(childItem.gameObject);
            // }

            Type gameRuleType = typeof(GameRule);
            Array gameRulesArray = Enum.GetValues(gameRuleType);
            foreach (Enum gameRuleItem in gameRulesArray) {
                if (_activeGameRules.HasFlag(gameRuleItem)) {
                    GameRule currentRule = (GameRule) Enum.ToObject(gameRuleType, gameRuleItem);
                    RuleBarItem currentRuleBarItemPrefab;

                    if (ruleBarItemByRule.TryGetValue(currentRule, out currentRuleBarItemPrefab)) {
                        RuleBarItem ruleBarItemInstance = Instantiate(currentRuleBarItemPrefab, transform, false);
                        ruleBarItemInstance.RuleDescriptionPanel = ruleDescriptionPanel;
                    }
                }
            }
        }
    }
}
