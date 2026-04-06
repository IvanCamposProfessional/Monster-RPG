using UnityEngine;

public static class KnowledgeFlags
{
    //─────────────────────────────────────────
    //WORLD FLAGS
    //Flags de progresion del mundo y la historia
    //Uso: GameManager.Instance.Knowledge.AddWorldKnowledge(KnowledgeFlags.World.HubUnlocked)
    //─────────────────────────────────────────
    public static class World
    {
        //Zonas del HUB
        public const string HubZone1Unlocked = "hub_zone1_unlocked";
        public const string HubZone2Unlocked = "hub_zone2_unlocked";

        //Progresion de historia principal
        public const string IntroCompleted = "intro_completed";
        public const string FirstRunCompleted = "first_run_completed";

        //Edificios desbloqueados
        public const string LabUnlocked = "building_lab_unlocked";
        public const string ObservatoryUnlocked = "building_observatory_unlocked";
    }

    // ─────────────────────────────────────────
    // NPC FLAGS
    // Flags de interacciones y misiones de NPCs
    // Uso: GameManager.Instance.Knowledge.AddNPCKnowledge(KnowledgeFlags.NPC.ResearcherMet)
    // ─────────────────────────────────────────
    public static class NPC
    {
        //NPC de ejemplo
        public const string ExampleNPCMet = "npc_examplenpc_met";
        public const string ExampleNPCQuest1Started = "npc_examplenpc_quest1_started";
        public const string ExampleNPCQuest1Done = "npc_examplenpc_quest1_done";
    }
}
