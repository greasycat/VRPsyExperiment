using System;
using UnityEngine.Rendering;

namespace Experiment
{
    public class TrialInfo
    {
        public TrialInfo(int trialNumber, int participantId, float timeTaken)
        {
            TrialNumber = trialNumber;
            ParticipantId = participantId;
            TimeTaken = timeTaken;
        }

        public TrialInfo(string row)
        {
            
        }

        public int TrialNumber { set; get; }
        public int ParticipantId { set; get; }
        public float TimeTaken { set; get; }

        public static TrialInfo FromCsvRow(string row)
        {
            return new TrialInfo(-1, -1, -1.0f);
        }

    }
}