/*
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

namespace Itinero.Optimization.Tests.Strategies.TestProblems.MinimizeInteger
{
    internal static class Operators
    {
        /// <summary>
        /// A simple test operator that does nothing.
        /// </summary>
        /// <returns></returns>
        public static Func<IntegerCandidate, bool> Empty = (candidate) => false;

        /// <summary>
        /// A simple test operator that subtracts random numbers.
        /// </summary>
        /// <returns></returns>
        public static readonly Func<IntegerCandidate, bool> SubtractRandom = (candidate) => 
        {
            if (candidate.Value == 0)
            {
                return false;
            }
            var r = Itinero.Optimization.Strategies.Random.RandomGenerator.Default.Generate(4) + 1;
            candidate.Value -= r;
            if (candidate.Value >= 0) return true;
            candidate.Value = 0;
            return true;
        };

        /// <summary>
        /// A simple test operator that adds or subtracts small random numbers.
        /// </summary>
        public static Func<IntegerCandidate, bool> MutateRandom = (candidate) =>
        {
            var r = Itinero.Optimization.Strategies.Random.RandomGenerator.Default.Generate(8);
            candidate.Value += r - 4;
            if (candidate.Value >= 0) return true;
            candidate.Value = 0;
            return true;
        };
    }
}