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

using System.Threading;
using Itinero.Optimization.Solvers.CVRP.Operators;
using Itinero.Optimization.Solvers.CVRP.SCI;
using Itinero.Optimization.Strategies;
using Itinero.Optimization.Strategies.GA;

namespace Itinero.Optimization.Solvers.CVRP.GA
{
    /// <summary>
    /// A solver using a GA and the edge-assembly crossover.
    /// </summary> 
    internal class GASolver : Strategy<CVRProblem, CVRPCandidate>
    {
        private readonly GAStrategy<CVRProblem, CVRPCandidate> _gaStrategy;
        
        /// <summary>
        /// Creates a new GA solver.
        /// </summary>
        /// <param name="generator">The generator strategy, if any.</param>
        /// <param name="mutation">The mutation operator, if any.</param>
        /// <param name="crossOver">The cross over operator, if any.</param>
        /// <param name="settings">The settings, if not default.</param>
        public GASolver(Strategy<CVRProblem, CVRPCandidate> generator = null, Operator<CVRPCandidate> mutation = null,
            CrossOverOperator<CVRPCandidate> crossOver = null, GASettings settings = null)
        {
            generator = generator ?? new SeededCheapestInsertionStrategy();
            mutation = mutation ?? new RedoPlacementOperator();
            crossOver = crossOver ?? new TourExchangeCrossOverOperator();
            settings = settings ?? GASettings.Default;

            _gaStrategy = new GAStrategy<CVRProblem, CVRPCandidate>(
                generator, crossOver, mutation, settings);
        }

        public override string Name => _gaStrategy.Name;

        public override CVRPCandidate Search(CVRProblem problem)
        {
            return _gaStrategy.Search(problem);
        }
        
        private static readonly ThreadLocal<GASolver> DefaultLazy = new ThreadLocal<GASolver>(() => new GASolver());
        public static GASolver Default => DefaultLazy.Value;
    }
}