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

using System;
using System.Collections.Generic;
using System.Threading;
using Itinero.Optimization.Solvers.Shared.Operators;
using Itinero.Optimization.Solvers.Shared.Seeds;

namespace Itinero.Optimization.Solvers.CVRP_ND.Operators
{
    /// <summary>
    /// A seed placement operator. This operator uses a seed function to start a new tour.
    /// </summary>
    internal class SeedPlacementOperator : PlacementOperator<CVRPNDCandidate>
    {
        private readonly Func<CVRPNDCandidate, ICollection<int>, int> _seedFunc;

        /// <summary>
        /// Creates a seed placement operator.
        /// </summary>
        /// <param name="seedFunc">The seed function to select a new seed.</param>
        public SeedPlacementOperator(Func<CVRPNDCandidate, ICollection<int>, int> seedFunc = null)
        {
            _seedFunc = seedFunc ?? ((candidate, visits) => SeedHeuristics.GetSeedRandom(visits));
        }
        
        public override string Name => "SEED_PLACE";

        public override bool Apply(CVRPNDCandidate candidate)
        {            
            var visits = new HashSet<int>(candidate.Problem.Visits);
            for (var t = 0; t < candidate.Solution.Count; t++)
            {
                foreach (var visit in candidate.Solution.Tour(t))
                {
                    visits.Remove(visit);
                }
            }

            return this.Apply(candidate, visits);
        }

        public override bool Apply(CVRPNDCandidate candidate, ICollection<int> visits)
        {
            var seed = _seedFunc(candidate, visits);
            if (seed < 0)
            {
                return false;
            }
            
            candidate.AddNew(seed);
            visits.Remove(seed);
            
            return true;
        }
        
        private static readonly ThreadLocal<SeedPlacementOperator> DefaultLazy = new ThreadLocal<SeedPlacementOperator>(() => new SeedPlacementOperator());
        public static SeedPlacementOperator Default => DefaultLazy.Value;
    }
}