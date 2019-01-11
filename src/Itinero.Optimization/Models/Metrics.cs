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
using Itinero.Profiles;

namespace Itinero.Optimization.Models
{
    /// <summary>
    /// Defines a few default metric names.
    /// </summary>
    public static class Metrics
    {
        /// <summary>
        /// The default name used for time.
        /// </summary>
        public const string Time = "time";

        /// <summary>
        /// The default name used for distance.
        /// </summary>
        public const string Distance = "distance";

        /// <summary>
        /// The default name used for weight.
        /// </summary>
        public const string Weight = "weight";
        
        /// <summary>
        /// Converts profile metrics into the equivalent model type.
        /// </summary>
        public static string ToModelMetric(this ProfileMetric profileMetric)
        {
            switch (profileMetric)
            {
                case ProfileMetric.DistanceInMeters:
                    return Models.Metrics.Distance;
                case ProfileMetric.TimeInSeconds:
                    return Models.Metrics.Time;
                case ProfileMetric.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(profileMetric), profileMetric, null);
            }
            return "custom";
        }
    }
}