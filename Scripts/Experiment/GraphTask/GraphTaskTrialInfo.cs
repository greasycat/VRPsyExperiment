namespace Experiment.GraphTask
{
    public class GraphTaskTrialInfo : TrialInfo
    {
        public bool Success { set; get; }
        public string StartingObject { set; get; }
        public string TargetObject { set; get; }
        
        public GraphTaskTrialInfo(int trialNumber = -1, int participantId = -1, string startingObject = "null", string targetObject = "null", bool success = false, float timeTaken = -1f) : base(trialNumber, participantId, timeTaken)
        {
            StartingObject = startingObject;
            TargetObject = targetObject;
            Success = success;
        }

        public static GraphTaskTrialInfo CreateFromCsv(string row)
        {
            var fields = row.Split(',');
            return fields.Length < 2 ? new GraphTaskTrialInfo() : new GraphTaskTrialInfo(int.Parse(fields[0]),  -1, fields[1], fields[2]);
        }

        public override string ToString()
        {
            return $"{TrialNumber}, {ParticipantId}, {StartingObject}, {TargetObject}, {Success}, {TimeTaken}";
        }
    }
}