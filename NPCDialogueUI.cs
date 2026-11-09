using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IqraZainab.AINPC
{
    /// <summary>
    /// Minimal dialogue UI: an input field, a send button and a speech bubble with a typewriter effect.
    /// Wire the references in the Inspector and point <see cref="npc"/> at an <see cref="AINPC"/>.
    /// </summary>
    public class NPCDialogueUI : MonoBehaviour
    {
        [SerializeField] private AINPC npc;
        [SerializeField] private TMP_InputField input;
        [SerializeField] private Button sendButton;
        [SerializeField] private TMP_Text nameLabel;
        [SerializeField] private TMP_Text bubbleText;
        [SerializeField] private GameObject thinkingIndicator;
        [SerializeField, Range(0.005f, 0.1f)] private float charDelay = 0.025f;

        private Coroutine _typing;

        private void Awake()
        {
            if (npc != null && npc.Persona != null) nameLabel.text = npc.Persona.npcName;
            thinkingIndicator.SetActive(false);
        }

        private void OnEnable()
        {
            sendButton.onClick.AddListener(Send);
            input.onSubmit.AddListener(_ => Send());
            npc.onReply.AddListener(ShowReply);
            npc.onThinking.AddListener(SetThinking);
        }

        private void OnDisable()
        {
            sendButton.onClick.RemoveListener(Send);
            input.onSubmit.RemoveAllListeners();
            npc.onReply.RemoveListener(ShowReply);
            npc.onThinking.RemoveListener(SetThinking);
        }

        private void Send()
        {
            if (npc.IsBusy || string.IsNullOrWhiteSpace(input.text)) return;
            npc.Ask(input.text);
            input.text = string.Empty;
            input.ActivateInputField();
        }

        private void SetThinking(bool thinking)
        {
            thinkingIndicator.SetActive(thinking);
            sendButton.interactable = !thinking;
        }

        private void ShowReply(string reply)
        {
            if (_typing != null) StopCoroutine(_typing);
            _typing = StartCoroutine(Typewriter(reply));
        }

        private IEnumerator Typewriter(string text)
        {
            bubbleText.text = text;
            bubbleText.maxVisibleCharacters = 0;
            for (int i = 0; i <= text.Length; i++)
            {
                bubbleText.maxVisibleCharacters = i;
                yield return new WaitForSeconds(charDelay);
            }
        }
    }
}
