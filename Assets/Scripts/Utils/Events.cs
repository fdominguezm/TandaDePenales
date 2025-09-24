using System;

namespace PenaltyShootout.Utils
{
    public static class Events
    {
        public static Action<int,int> OnScoreChanged; // localScore, awayScore
        public static Action<string> OnRoundMessage; // mensaje en UI
        public static Action<int,bool> OnShotResult; // shotIndex, wasGoal
    }
}
