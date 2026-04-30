namespace Application.Helpers
{
    public static class SegmentHelper
    {
        public const double MaxSegmentSeconds = 15;
        public const double CompletionThreshold = 0.9;
        public const double ViewCountThreshold = 10;

        public static List<(double Start, double End)> MergeRanges(IEnumerable<(double Start, double End)> ranges)
        {
            var sorted = ranges.OrderBy(r => r.Start).ToList();
            if (sorted.Count == 0) return new();

            var merged = new List<(double Start, double End)> { sorted[0] };

            for (int i = 1; i < sorted.Count; i++)
            {
                var (start, end) = sorted[i];
                var last = merged[^1];

                if (start <= last.End)
                    merged[^1] = (last.Start, Math.Max(last.End, end));
                else
                    merged.Add((start, end));
            }

            return merged;
        }
        public static double CalculateTotalWatched(IEnumerable<(double Start, double End)> ranges)
            => ranges.Sum(r => r.End - r.Start);
        public static List<(double Start, double End)> NormalizeIncomingSegments(IEnumerable<double[]> rawSegments, double duration)
        {
            var valid = new List<(double Start, double End)>();

            foreach (var raw in rawSegments)
            {
                if (raw is not { Length: 2 }) continue;

                var start = Math.Clamp(raw[0], 0, duration);
                var end = Math.Clamp(raw[1], 0, duration);

                var size = end - start;
                if (end <= start || size > MaxSegmentSeconds) continue;

                valid.Add((start, end));
            }

            return valid;
        }
        public static double RoundToTwo(double value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}