using UnityEngine;

namespace IqraZainab.AINPC
{
    /// <summary>
    /// Opens the dialogue UI when the player walks near the NPC and closes it when they leave.
    /// Requires a trigger Collider on the NPC and a Player tagged "Player".
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class NPCInteractionTrigger : MonoBehaviour
    {
        [SerializeField] private AINPC npc;
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private GameObject pressKeyHint;
        [SerializeField] private KeyCode talkKey = KeyCode.E;
        [SerializeField] private bool resetMemoryOnLeave = false;

        private bool _playerInRange;

        private void Reset() => GetComponent<Collider>().isTrigger = true;

        private void Start()
        {
            dialoguePanel.SetActive(false);
            if (pressKeyHint) pressKeyHint.SetActive(false);
        }

        private void Update()
        {
            if (_playerInRange && !dialoguePanel.activeSelf && Input.GetKeyDown(talkKey))
            {
                dialoguePanel.SetActive(true);
                if (pressKeyHint) pressKeyHint.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = true;
            if (pressKeyHint) pressKeyHint.SetActive(true);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;
            _playerInRange = false;
            dialoguePanel.SetActive(false);
            if (pressKeyHint) pressKeyHint.SetActive(false);
            if (resetMemoryOnLeave) npc.ResetMemory();
        }
    }
}
