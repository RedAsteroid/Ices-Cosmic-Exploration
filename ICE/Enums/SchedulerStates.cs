namespace ICE.Enums
{
    [Flags]
    internal enum IceState
    {
        Idle = 0,
        Start = 1,
        GrabMission = 2,
        ExecutingMission = 3,
        AbandonMission = 4,
        ForceTurnin = 5,
        ScoreCheck = 6,
        ManualMode = 7,
        Waiting = 8,

        HubReturn = 10,
        Repair = 11,
        Gambling = 12,
        RelicTurnin = 13,
        Spiritbond = 14,
        Shopping = 15,
        ArtifactSearch = 16,

        Craft = 20,
        Gather = 21,
        Fish = 22,
        DualClass = 23,

        ScoringMission = 30,
        AnimationLock = 31,
        TurninMission = 32,

    }
}