﻿/*
 *  Licensed to SharpSoftware under one or more contributor
 *  license agreements. See the NOTICE file distributed with this work for 
 *  additional information regarding copyright ownership.
 * 
 *  SharpSoftware licenses this file to you under the Apache License, 
 *  Version 2.0 (the "License"); you may not use this file except in 
 *  compliance with the License. You may obtain a copy of the License at
 * 
 *       http://www.apache.org/licenses/LICENSE-2.0
 * 
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.Tours;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.CVRP.GA
{
    /// <summary>
    /// A tour exchange cross over operator. Builds a new candidate based on tours from two other candidates.
    /// </summary>
    internal class TourExchangeCrossOverOperator : CrossOverOperator<CVRPCandidate>
    {
        private readonly Operator<CVRPCandidate> _postOperator;

        /// <summary>
        /// Creates a new cross over operator.
        /// </summary>
        /// <param name="postOperator">The operator to apply after cross over, if any.</param>
        public TourExchangeCrossOverOperator(Operator<CVRPCandidate> postOperator = null)
        {
            _postOperator = postOperator;
        }
        
        public override string Name { get; } = "CROSS_EX_TOURS";
        
        public override CVRPCandidate Apply(CVRPCandidate candidate1, CVRPCandidate candidate2)
        {
            var candidate = new CVRPCandidate()
            {
                Solution = new CVRPSolution(),
                Problem = candidate1.Problem,
                Fitness = 0
            };
            var visits = new HashSet<int>(candidate.Problem.Visits);
            visits.Remove(candidate.Problem.Departure);
            if (candidate.Problem.Arrival.HasValue) visits.Remove(candidate.Problem.Arrival.Value);
            
            // try to use as many tours as possible.
            var success = true;
            while (success)
            {
                success = false;
                
                // try to select a tour from solution1 to use in the given solution.
                success |= this.SelectAndMoveTour(visits, candidate1, candidate);
                
                // try to select a tour from solution2 to use in the given solution.
                success |= this.SelectAndMoveTour(visits, candidate1, candidate);
            }

            _postOperator?.Apply(candidate);

            return candidate;
        }

        private bool SelectAndMoveTour(HashSet<int> visits, CVRPCandidate source, CVRPCandidate target)
        {
            if (visits.Count == 0)
            { // no more visits, no more tours to copy over.
                return false;
            }
            
            // TODO: figure out if it makes senses to keep track of previously select tours and exclude them.
            Tour tour = null;
            if (target.Count == 0)
            { // target is empty, select random tour.
                tour = source.Tour(Strategies.Random.RandomGenerator.Default.Generate(source.Count));
            }
            else
            {
                // search for a tour that has the least overlapping visits.
                // TODO: this may give smaller (and worse) tours an advantage. Perhaps change this to a metric with the most visits inserted or something similar.
                var bestT = -1;
                var bestOverlap = int.MaxValue;
                for (var t = 0; t < source.Count; t++)
                {
                    tour = source.Tour(t);
                    var tourOverlap = 0;
                    var hasUnplaced = false;
                    foreach (var v in tour)
                    {
                        if (!visits.Contains(v))
                        {
                            tourOverlap++;
                        }
                        else
                        {
                            hasUnplaced = true;
                        }
                    }

                    if (tourOverlap > tour.Count / 4)
                    {
                        continue;
                    }
                    if (hasUnplaced &&
                        tourOverlap < bestOverlap)
                    {
                        bestOverlap = tourOverlap;
                        bestT = t;
                    }
                }

                if (bestT < 0)
                {
                    return false;
                }

                tour = source.Tour(bestT);
            }

            // copy over tour.
            var bestTour = tour;
            var targetT = target.AddNew();
            var previous = bestTour.First;
            foreach (var pair in bestTour.Pairs())
            {
                if (!visits.Remove(pair.To))
                { // TODO: do we need to close the tour here or not?
                    continue;
                }
                target.InsertAfter(targetT, previous, pair.To);

                previous = pair.To;
            }
            return true;
        }
        
        private static readonly ThreadLocal<TourExchangeCrossOverOperator> DefaultLazy = new ThreadLocal<TourExchangeCrossOverOperator>(() => new TourExchangeCrossOverOperator());
        public static TourExchangeCrossOverOperator Default => DefaultLazy.Value;
    }
}