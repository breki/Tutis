using System;
using System.Collections.Generic;

namespace GisExperiments.ContoursLabeling
{
    public class IntervalSet
    {
        public IntervalSet(float totalLength)
        {
            this.totalLength = totalLength;
        }

        public float TotalIntervalsLength
        {
            get
            {
                float length = 0;
                for (int i = 0; i < intervalStarts.Count; i++)
                {
                    if (i % 2 == 0)
                    {
                        float start = intervalStarts[i];
                        float end;
                        if (i < intervalStarts.Count - 1)
                            end = intervalStarts[i + 1];
                        else
                            end = totalLength;
                        length += end - start;
                    }
                }

                return length;
            }
        }

        public void AddInterval(float from, float to)
        {
            if (to <= from)
                throw new ArgumentOutOfRangeException("to");

            if (intervalStarts.Count == 0)
            {
                intervalStarts.Add (from);
                intervalStarts.Add (to);
                return;
            }

            bool replaceFrom, replaceTo;
            int i = FindPlaceForValue(from, out replaceFrom);
            int j = FindPlaceForValue(to, out replaceTo);

            if (i % 2 == 1)
            {
                // the start is inside existing interval, so we don't add it

                if (i == j)
                {
                    // the new interval is inside existing interval, so we can ignore it
                    return;
                }

                if (j % 2 == 0)
                {
                    // the end is outside of an interval
                    // so we remove everything from i to j

                    int removeCount = j - i;
                    intervalStarts.RemoveRange (i, removeCount);
                    intervalStarts.Insert (i, to);
                }
                else
                {
                    int removeCount = j - i;
                    intervalStarts.RemoveRange (i, removeCount);
                }
            }
            else
            {
                // the start is outside existing interval

                if (replaceTo)
                    j++;

                if (i == j)
                {
                    // the whole new interval can be simply inserted without much hassle
                    intervalStarts.Insert (i, from);
                    intervalStarts.Insert (i + 1, to);
                }
                else
                {
                    int removeCount = j - i;
                    intervalStarts.RemoveRange(i, removeCount);
                    intervalStarts.Insert(i, from);

                    if (j % 2 == 0)
                    {
                        // the end is outside of an interval
                        intervalStarts.Insert (i + 1, to);
                    }
                }
            }
        }

        public void Validate()
        {
            for (int i = 0; i < intervalStarts.Count - 1; i++)
            {
                if (intervalStarts[i+1] == intervalStarts[i])
                    throw new InvalidOperationException("An interval is empty.");
                if (intervalStarts[i + 1] < intervalStarts[i])
                    throw new InvalidOperationException ("The set is not monotone.");
            }
        }

        private int FindPlaceForValue(float value, out bool replace)
        {
            replace = false;
            for (int i = 0; i < intervalStarts.Count; i++)
            {
                if (value < intervalStarts[i])
                    return i;

                if (value == intervalStarts[i])
                {
                    replace = true;
                    return i;
                }
            }

            return intervalStarts.Count;
        }

        private List<float> intervalStarts = new List<float>();
        private readonly float totalLength;
    }
}