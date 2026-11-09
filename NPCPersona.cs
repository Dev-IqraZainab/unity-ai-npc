using UnityEngine;

namespace IqraZainab.AINPC
{
    /// <summary>
    /// Designer-friendly ScriptableObject that defines who an NPC is.
    /// Create via: Assets ▸ Create ▸ AI NPC ▸ Persona
    /// </summary>
    [CreateAssetMenu(fileName = "NewPersona", menuName = "AI NPC/Persona")]
    public class NPCPersona : ScriptableObject
    {
        public string npcName = "Mira";
        [TextArea(2, 4)] public string role = "the blacksmith of Frogwood village";
        [TextArea(2, 4)] public string personality = "warm, witty, a bit grumpy before noon";
        [TextArea(3, 6)] public string worldKnowledge =
            "Frogwood is a swampy village. The old well glows at night. A dragon was last seen in the north hills.";
        [TextArea(2, 4)] public string goals = "sell swords, find her lost hammer";

        [Tooltip("Keep replies short so they fit in a speech bubble.")]
        [Range(1, 4)] public int maxSentences = 2;

        /// <summary>Builds the system prompt sent to the language model.</summary>
        public string BuildSystemPrompt()
        {
            return
                $"You are {npcName}, {role} in a video game.\n" +
                $"Personality: {personality}.\n" +
                $"What you know about the world: {worldKnowledge}\n" +
                $"Your goals: {goals}.\n" +
                $"Rules: stay in character, never mention being an AI, " +
                $"reply in at most {maxSentences} short sentences, and only use facts from your world knowledge.";
        }
    }
}
