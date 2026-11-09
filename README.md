<h1 align="center">🧙 Unity AI NPC</h1>

<p align="center">
  <b>Drop-in Unity scripts that make NPCs talk to players using AI (Anthropic Claude), with persona, memory and typewriter dialogue.</b>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Unity-2021.3%2B-000000?style=for-the-badge&logo=unity&logoColor=white" />
  <img src="https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=csharp&logoColor=white" />
  <img src="https://img.shields.io/badge/Claude%20API-D97757?style=for-the-badge&logo=anthropic&logoColor=white" />
  <img src="https://img.shields.io/badge/License-MIT-00C9FF?style=for-the-badge" />
</p>

---

## ✨ Features

- 🧠 **AI-powered dialogue:** NPCs answer anything the player types, fully in character
- 🎭 **Persona system:** a designer-friendly `ScriptableObject` (name, role, personality, world lore, goals)
- 💾 **Rolling memory:** NPCs remember the last *N* messages
- ⌨️ **Typewriter UI** with a "thinking…" indicator (TextMeshPro)
- 🚶 **Proximity trigger:** walk up and press **E** to talk
- 📴 **Offline demo mode:** works with no API key (keyword replies)
- 🔌 **Event-driven:** `onReply` and `onThinking` UnityEvents, easy to hook into animation or voice

## 📦 Scripts

| Script | What it does |
|---|---|
| `AINPC.cs` | Core brain: builds the request, calls the API with `UnityWebRequest`, manages memory |
| `NPCPersona.cs` | `ScriptableObject` that turns persona fields into a system prompt |
| `NPCDialogueUI.cs` | Input field, send button, speech bubble with typewriter effect |
| `NPCInteractionTrigger.cs` | Opens and closes the dialogue when the player is in range |

## 🧠 Architecture

```
 Player types ─► NPCDialogueUI ─► AINPC.Ask()
                                   │  system prompt ◄── NPCPersona
                                   │  history[]  (rolling memory)
                                   ▼
                        UnityWebRequest → Claude API
                                   │
              onReply(string) ◄────┘ ──► typewriter bubble / animations / TTS
```

## 🚀 Setup (5 minutes)

1. Copy the 4 `.cs` files into `Assets/Scripts/AINPC/`
2. Install **TextMeshPro** (Window ▸ Package Manager)
3. **Create a persona:** `Assets ▸ Create ▸ AI NPC ▸ Persona`, then fill in name, role and lore
4. On your NPC GameObject:
   - Add **AINPC**, assign the persona, paste your API key (or leave it empty for offline mode)
   - Add a **Collider** (set as trigger) + **NPCInteractionTrigger**
5. Build a simple Canvas: `TMP_InputField`, `Button`, two `TMP_Text` fields and a "thinking" icon, then add **NPCDialogueUI** and wire the references
6. Tag your player **Player**, press ▶ Play, walk up to the NPC and press **E** 🎉

## 🔐 Security Note

For quick prototypes, the key can sit on the component. **For a shipped game, never put API keys in the client.** Route requests through your own backend or serverless function, and point `ApiUrl` there.

## 🗺️ Roadmap

- [ ] Text-to-speech voice lines
- [ ] Emotion tags → animator triggers (happy, angry, surprised)
- [ ] Quest hooks: let the AI give and complete quests via function calling
- [ ] Sample scene with a low-poly village

## 👩‍💻 Author

**Iqra Zainab**: AI & Unity Game Developer | Metaverse & Web3

[![GitHub](https://img.shields.io/badge/GitHub-181717?style=flat-square&logo=github)](https://github.com/Dev-IqraZainab)
[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?style=flat-square&logo=linkedin)](https://www.linkedin.com/in/iqra-zainab/)

📄 Licensed under the [MIT License](LICENSE).
