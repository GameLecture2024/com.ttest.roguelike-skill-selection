# TTest Roguelike Skill Selection

This is intentionally a personal/project package. It owns roguelike skill-choice data and a UGUI panel, while pooling remains in `com.ttest.unity-utilities`.

## Scene setup

1. Create `SkillChoiceDefinition` assets from **TTest/Roguelike/Skill Choice**.
2. Make a card prefab with `SkillChoiceItemView`, a `Button`, optional icon `Image`, and two legacy UGUI `Text` fields.
3. Add `RoguelikeSkillSelectionPanel` to a scene object and assign its panel, layout/content transform, and card prefab.
4. Call `Present(choices, ApplyChoice)` from the run/level-up system.

The caller owns random choice generation, ownership rules, costs, and applying the selected skill. That policy should remain outside this view package.
