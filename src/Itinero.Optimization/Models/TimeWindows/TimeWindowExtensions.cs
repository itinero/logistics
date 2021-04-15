using System.Collections.Generic;
using System.Text;

namespace Itinero.Optimization.Models.TimeWindows
{
    internal static class TimeWindowExtensions
    {
        internal static bool IsValid(this TimeWindow timeWindow)
        {
            if (timeWindow.Times == null) return true;

            for (var i = 1; i < timeWindow.Times.Count; i++)
            {
                if (timeWindow.Times[i] < timeWindow.Times[i - 1]) return false;
            }

            return true;
        }

        internal static string ToJsonArray(this TimeWindow timeWindow)
        {
            if (timeWindow.Times == null) return string.Empty;
            
            var builder = new StringBuilder();
            for (var t = 0; t < timeWindow.Times.Count; t++)
            {
                if (t % 2 == 0)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(',');
                    }
                    builder.Append('[');
                    builder.Append(timeWindow.Times[t].ToInvariantString());
                }
                else
                {
                    builder.Append(',');
                    builder.Append(timeWindow.Times[t].ToInvariantString());
                    builder.Append(']');
                }
            }

            if (timeWindow.Times.Count % 2 != 0)
            {
                builder.Append(float.MaxValue.ToInvariantString());
                builder.Append(']');
            }
            return builder.ToInvariantString();
        }

        internal static IEnumerable<(float start, float end)> ToRanges(this IReadOnlyList<float> times)
        {
            if (times == null) yield break;
            
            var previous = -1f;
            foreach (var current in times)
            {
                if (previous >= 0)
                {
                    yield return (previous, current);
                    previous = -1;
                }
                else
                {
                    previous = current;
                }
            }

            if (previous >= 0)
            {
                yield return (previous, float.MaxValue);
            }
        }
    }
}