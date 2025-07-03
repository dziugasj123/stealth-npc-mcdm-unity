# stealth-npc-mcdm-unity

This is a Unity 3D project where an NPC intelligently selects hiding spots using the **Weighted Sum Model (WSM)** — a multi-criteria decision-making method. The player must find the NPC before it escapes or remains hidden.

---

## Project Summary

- **A\* Pathfinding**: NPCs use a 2D grid-based A\* algorithm for movement.
- **Multi-Criteria Hiding Spot Selection**: NPC evaluate spots based on:
  - Distance from player
  - Safety score (dynamic based on visibility and proximity)
  - Escape route availability
- **Hiding spot iteration**: When the player comes within **20 grid tiles** of the NPC, the NPC dynamically re-evaluates all hiding spots using WSM logic and may switch to a safer location.
- **Game over**: The game ends when the player sees the NPC using a real-time Raycast visibility check.

---

## Requirements
- [Unity Hub](https://unity.com/download)
- Unity version: **2022.3.28f1 LTS**

## Gameplay demo
[![Watch the video](https://img.youtube.com/vi/VH9ivYNfitc/0.jpg)](https://youtu.be/VH9ivYNfitc)
