using System;
using System.Collections.Generic;
using System.Linq;

namespace Vim.DotNetUtilities
{
    public static class Sampling
    {
        /// <summary>
        /// Returns the interpolated samples between p0 and p1.
        /// </summary>
        public static IEnumerable<T> InterpolateSamplesExclusive<T>(T p0, T p1, int numSamples, Func<T, T, float, T> interpolateFunc)
            //
            // numSamples: 2
            //
            // p0 > *
            //           _
            //      *     |
            //      |     |-- RESULT 
            //      *    _|
            //       
            // p1 > *
            //
            => Enumerable.Range(0, numSamples)
                .Select(i => interpolateFunc(p0, p1, (i + 1) / (float)(numSamples + 1)));

        /// <summary>
        /// Returns the interpolated samples between p0 and p1, including both p0 and p1.
        /// </summary>
        public static IEnumerable<T> InterpolateSamplesInclusive<T>(T start, T end, int numSamples, Func<T, T, float, T> lerpFunc)
            //
            // numSamples: 2
            //           _
            // p0 > *     |
            //      |     |
            //      *     |
            //      |     |-- RESULT 
            //      *     |
            //      |     |
            // p1 > *    _|
            //
            => InterpolateSamplesExclusive(start, end, numSamples, lerpFunc)
                .Prepend(start)
                .Append(end);

        /// <summary>
        /// Returns lists of interpolated samples between the startPoints and their respective endPoints.
        /// </summary>
        public static IEnumerable<IList<T>> InterpolateSampleSequenceInclusive<T>(
                int startIndex,
                int count,
                IList<T> startPoints,
                IList<T> endPoints,
                int numSamples,
                Func<T, T, float, T> lerpFunc)
            //
            // numSamples: 2
            //
            //                  startIndex  startIndex + count
            //                       v           v
            //
            // startPoints >   * ... *   *   *   * ... *
            //                       |   |   |   |
            //                       *   *   *   *
            //                       |   |   |   |
            //                       *   *   *   *
            //                       |   |   |   |
            //   endPoints >   * ... *   *   *   * ... *
            //
            //                      |_____________|
            //                             |
            //                           RESULT
            //
            => Enumerable.Range(startIndex, count)
                .Select(index =>
                    InterpolateSamplesInclusive(startPoints[index], endPoints[index], numSamples, lerpFunc).ToList());
    }
}
