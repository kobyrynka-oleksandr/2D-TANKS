# 🎮 2D Tanks

A top-down 2D tank shooter where you defend your city's flag
against waves of enemy tanks. Survive as many stages as possible and compete
for the highest score on the global leaderboard.

---

## 🕹️ Gameplay

The player controls a tank and must protect the city's **flag** (an object
with HP) from incoming enemy waves. The game ends if enemies destroy the
flag or kill the player. The map contains **destructible blocks** that
provide cover for the flag. Progress through stages by eliminating all
enemies, and pick up **power-up bonuses** to gain an edge in battle.

---

## 👾 Enemies

| Enemy | Behavior |
|---|---|
| 🔴**Destroyer** | Targets the flag only; ignores the player and never changes route |
| 🟠**Killer** | Targets the player only; completely ignores the flag; constantly chases and attacks |
| 🟡**Scout** | Targets the flag by default, but reacts to the player via two detection zones |

### Scout Detection Zones
- **AggrZone** *(smaller)* — if the player enters this zone, the Scout switches to
  chasing the player (Killer behavior)
- **ChaseZone** *(larger)* — if the player leaves this zone, the Scout loses interest
  and returns to attacking the flag

---

## 🌊 Enemy Spawning System

Enemies are spawned in **stage-based waves** managed by two components:
`GameManager` and `SpawnPoint`.

### Stage Progression
- Each stage spawns **N enemies**, where N equals the current stage number
  (Stage 1 → 1 enemy, Stage 2 → 2 enemies, etc.)
- Enemies are randomly selected from the available enemy prefabs each spawn
- Once **all enemies are eliminated**, a short cooldown begins and the next
  stage starts automatically

### Spawn Points
- The map has multiple **SpawnPoint** objects placed around the edges
- Enemies are distributed across spawn points using round-robin:
  `spawnPoint[i % pointCount]`
- Each SpawnPoint has an internal **queue** — if multiple enemies are
  assigned to the same point, they spawn one by one with a configurable
  delay between each (`DelayBetweenSpawns`)
- A spawn point tracks **occupancy**: the next enemy in the queue waits
  until the point is free before spawning

---

## 💥 Shooting Mechanic

The same mechanic applies to both the player and enemies:

1. Tank fires → a shell spawns and travels in the direction the tank is facing
2. The shell flies until it hits: the player, an enemy, a wall, or a destructible block
3. Shells can destroy certain blocks on the map (enemy)

---

## ⚡ Bonuses

Bonuses spawn **2 times per stage** at points on the map. If not
collected by the end of the stage, they disappear. New bonuses spawn fresh
each stage — **no stacking**.

| Bonus | Effect |
|---|---|
| 💚 **Heal** | Restores a portion of the player's HP |
| ⚡ **Speed** | Temporarily increases movement speed |
| 🔥 **Double Damage** | Temporarily doubles damage dealt to enemies |

---

## 🎮 Controls

Implemented using Unity's **New Input System** with PC and Mobile support.

### 🖥️ PC
| Input | Action |
|---|---|
| `W` / `S` | Move forward / backward |
| `A` / `D` | Rotate left / right |
| `Space` / `LMB` / `Enter`| Fire |
| `Escape` | Pause |

### 📱 Mobile
| Input | Action |
|---|---|
| D-Pad *(left side)* | Move and rotate |
| Fire button *(right side)* | Fire |

---

## 🖥️ UI & Screens

### Main Menu
- **Play** — start the game
- **Settings** — audio controls: background music & SFX (volume sliders + toggles)
- **Leaderboard** — global top scores sorted by stage reached

### HUD (In-Game)
- Current stage number
- Player HP bar
- Flag HP bar
- Enemy HP bar (displayed above each enemy)
- Active bonus icons (shown while a bonus effect is active)

### Pause Screen
- **Resume** — continue the game
- **Quit to Main Menu** — with confirmation dialog:
  *"Progress will not be saved. Are you sure?"*

### Game Over Screen
- Stage reached
- Name entry field (3 characters — arcade-style)
- **Restart** button
- **Quit to Main Menu** button

---

## 🏆 Leaderboard

Global leaderboard powered by **Unity Gaming Services (UGS) Leaderboards**.  
Scores are stored in the cloud and visible to all players. After each game,
enter your name to submit your best stage to the leaderboard.
> ⚠️ Leaderboard requires an active internet connection
