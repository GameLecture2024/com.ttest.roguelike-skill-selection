# Skill Selection Demo 실습

먼저 `TTest Unity Utilities`와 이 패키지를 모두 설치한 뒤, Package Manager에서 **TTest Roguelike Skill Selection > Samples > Skill Selection Demo**의 **Import**를 누른다.

1. **TTest Samples > Roguelike Skill Selection > Create Demo Scene**을 연다.
2. `Create Demo Scene`을 누른다. Canvas, EventSystem, 카드 프리팹, 스킬 에셋 3개, 데모 씬이 만들어진다.
3. 생성된 `SkillSelectionDemo.unity`를 실행한다.
4. 카드 하나를 누르고, `Show Choices` 버튼 또는 `R` 키/게임패드 Start 버튼으로 선택 창을 다시 연다.

`SkillSelectionDemoInput`은 Unity Input System의 `InputAction`으로 입력을 등록한다. 데모 전용 입력이므로 패키지의 게임 규칙이나 UI 코드에는 입력 구현을 섞지 않는다.

생성 창은 기본적으로 Package Manager Import 경로인 `Assets/Samples/TTest Roguelike Skill Selection/0.1.0/Skill Selection Demo`에 씬과 에셋을 저장한다. 실제 게임에서는 `ShowChoices`에 전달할 목록을 레벨업 시스템이 만들고, `OnChoiceSelected`에서 플레이어의 스킬 상태를 변경한다.
