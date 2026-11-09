using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace IqraZainab.AINPC
{
    /// <summary>
    /// An NPC that talks to the player using a large language model (Anthropic Claude).
    /// Attach to any GameObject, assign an <see cref="NPCPersona"/>, and call <see cref="Ask"/>.
    /// Keeps a short rolling memory so conversations feel natural.
    /// </summary>
    public class AINPC : MonoBehaviour
    {
        [Header("Persona")]
        [SerializeField] private NPCPersona persona;

        [Header("API")]
        [Tooltip("Leave empty to run in offline demo mode.")]
        [SerializeField] private string apiKey = "";
        [SerializeField] private string model = "claude-sonnet-4-5";
        [SerializeField, Range(64, 1024)] private int maxTokens = 200;

        [Header("Memory")]
        [Tooltip("How many previous messages the NPC remembers.")]
        [SerializeField, Range(2, 30)] private int memorySize = 10;

        [Header("Events")]
        public UnityEvent<string> onReply;
        public UnityEvent<bool> onThinking;

        private const string ApiUrl = "https://api.anthropic.com/v1/messages";
        private readonly List<Message> _history = new List<Message>();
        private bool _busy;

        public NPCPersona Persona => persona;
        public bool IsBusy => _busy;

        /// <summary>Send the player's line to the NPC. The reply arrives via <see cref="onReply"/>.</summary>
        public void Ask(string playerMessage)
        {
            if (_busy || string.IsNullOrWhiteSpace(playerMessage)) return;
            StartCoroutine(AskRoutine(playerMessage.Trim()));
        }

        /// <summary>Forget the conversation (e.g. when the player walks away).</summary>
        public void ResetMemory() => _history.Clear();

        private IEnumerator AskRoutine(string playerMessage)
        {
            _busy = true;
            onThinking?.Invoke(true);
            Remember("user", playerMessage);

            if (string.IsNullOrEmpty(apiKey))
            {
                yield return new WaitForSeconds(0.6f);
                Finish(OfflineReply(playerMessage));
                yield break;
            }

            var body = new RequestBody
            {
                model = model,
                max_tokens = maxTokens,
                system = persona != null ? persona.BuildSystemPrompt() : "You are a friendly villager.",
                messages = _history.ToArray()
            };

            using (var request = new UnityWebRequest(ApiUrl, "POST"))
            {
                byte[] json = Encoding.UTF8.GetBytes(JsonUtility.ToJson(body));
                request.uploadHandler = new UploadHandlerRaw(json);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("content-type", "application/json");
                request.SetRequestHeader("x-api-key", apiKey);
                request.SetRequestHeader("anthropic-version", "2023-06-01");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[AINPC] Request failed: {request.error}\n{request.downloadHandler.text}");
                    _history.RemoveAt(_history.Count - 1); // let the player retry
                    Finish("Hmm... my mind went blank. Could you say that again?", remember: false);
                    yield break;
                }

                var response = JsonUtility.FromJson<ResponseBody>(request.downloadHandler.text);
                string reply = response?.content != null && response.content.Length > 0
                    ? response.content[0].text
                    : "...";
                Finish(reply);
            }
        }

        private void Finish(string reply, bool remember = true)
        {
            if (remember) Remember("assistant", reply);
            _busy = false;
            onThinking?.Invoke(false);
            onReply?.Invoke(reply);
        }

        private void Remember(string role, string content)
        {
            _history.Add(new Message { role = role, content = content });
            // Keep memory bounded, and always start with a user turn (API requirement).
            while (_history.Count > memorySize || (_history.Count > 0 && _history[0].role != "user"))
                _history.RemoveAt(0);
        }

        /// <summary>Simple keyword replies so the demo scene works without a key.</summary>
        private string OfflineReply(string msg)
        {
            string m = msg.ToLowerInvariant();
            string name = persona != null ? persona.npcName : "Villager";
            if (m.Contains("hello") || m.Contains("hi")) return $"Well met, traveler! I'm {name}.";
            if (m.Contains("quest")) return "The old well is full of glowing frogs at night... someone should check it out.";
            if (m.Contains("shop") || m.Contains("buy")) return "My shop opens at sunrise. Bring gold, not excuses!";
            return "(Offline mode) Add an API key on the AINPC component and I'll truly come alive!";
        }

        // ---------- JSON models (JsonUtility-friendly) ----------
        [Serializable] private class Message { public string role; public string content; }

        [Serializable]
        private class RequestBody
        {
            public string model;
            public int max_tokens;
            public string system;
            public Message[] messages;
        }

        [Serializable] private class ContentBlock { public string type; public string text; }
        [Serializable] private class ResponseBody { public ContentBlock[] content; }
    }
}
