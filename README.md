# CoreMerge (2D Action + Inventory Merge Project)

2D 액션 플랫포머 전투에 인벤토리 머지(progress) 시스템을 결합한 소규모(Unity 6) 프로젝트입니다.
몬스터를 처치해 떨어지는 코어를 모으고, 동일 코어 2개를 머지해 상급 코어를 생성, 스킬을 해금하며 성장하는 구조를 가지고 있습니다.
<br><br>

### 🔹 1. 프로젝트 개요
---
| 항목        | 내용                           |
| --------- | ---------------------------- |
| **프로젝트명** | CoreMerge                    |
| **장르**    | 2D 액션 플랫포머 + 인벤토리 머지         |
| **엔진**    | Unity 6.2, C#                |
| **목적**    | FSM·UI·데이터 구조 설계 학습 / 포트폴리오용 |
| **기간**    | 1주 (Prototype)               |


<br><br>

### 🔹 2. 핵심 게임 콘셉트
---

- 플랫포머 전투 + 코어 머지 성장 시스템
- 몬스터 처치 → 코어 드랍 → 머지 → 상급 코어 → 스킬 해금
- 인벤토리 그리드(3×4)와 하단 코어 슬롯(3개) 기반 성장 빌드업
- 1~2 스테이지 + 보스 1종 중심의 스몰 스케일 구조

<br><br>

### 🔹 3. Core Game Loop
---
```css
[전투 시작]
      ↓
몬스터 처치 → 코어 드랍 → 인벤토리 수집
      ↓
동일 코어 2개 머지 → 상급 코어 생성 → 스킬 해금
      ↓
플레이어 스탯 상승 + 스킬 공격 가능
      ↓
보스방 진입 → 보스 격파 → 클리어

```

<br><br>
### 🔹 4. 주요 시스템
---
#### 플레이어 시스템 (State Pattern 기반)
- Idle / Run / Jump / Climb
- Attack / SlideAttack / SkillAttack / Dead
- PlayerStateMachine → Enter/Exit/HandleInput/UpdateLogic/UpdatePhysics 구조

#### 전투 시스템
- 몬스터 AI: Patrol → Chase → Attack
- 플레이어 공격 / 피격 / 무적 프레임 / 넉백
- 보스 페이즈(Phase) 2~3단계

#### 인벤토리·머지 시스템
- Grid Inventory (3×4)
- 드래그 & 드랍 / 슬롯 스왑
- 머지(2→1) 기본 구조
- 코어 슬롯 장착 시 플레이어 스탯 반영
- 상급 코어는 스킬 해금 역할

#### 스킬 시스템
- SkillData 기반 스킬 정보 관리
- 스킬 해금 → 쿨다운 → SkillAttack 상태 실행

#### UI 시스템
- HP Bar / 스킬 HUD
- 인벤토리 On/Off
- 보스 HP UI
- 효과음/이펙트/카메라 쉐이크
