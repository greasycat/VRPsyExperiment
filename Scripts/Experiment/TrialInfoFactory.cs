using Experiment.GraphTask;

namespace Experiment
{
    public static class TrialInfoFactory
    {
        public static TrialInfo FromCsvRow<T>(string row)
        {
            return typeof(T).Name switch
            {
                "TrialInfo" => new TrialInfo(row),
                "GraphTaskTrialInfo" => GraphTaskTrialInfo.CreateFromCsv(row),
                _ => null
            };
        }
    }
}