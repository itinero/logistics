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

using Itinero.LocalGeo;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Vehicles;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Models.Visits.Costs;

namespace Itinero.Optimization.Models.Mapping
{
    /// <summary>
    /// Represents a mapped model.
    /// </summary>
    public class MappedModel
    {
        /// <summary>
        /// Gets or sets travel costs between the visits.
        /// </summary>
        public TravelCostMatrix[] TravelCosts { get; set; }
        
        /// <summary>
        /// Gets or sets the visits (including any depots).
        /// </summary>
        /// <returns></returns>
        public Visit[] Visits { get; set; }

        /// <summary>
        /// Gets or sets the vehicle pool.
        /// </summary>
        /// <returns></returns>
        public VehiclePool VehiclePool { get; set; }

        /// <summary>
        /// Serializes this model to json.
        /// </summary>
        /// <remarks>The <see cref="IO.Json.JsonSerializer"/> needs to be setup properly.</remarks>
        /// <returns></returns>
        public string ToJson()
        {
            return IO.Json.JsonSerializer.ToJsonFunc(this);
        }

        /// <summary>
        /// Deserializes a model from json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static MappedModel FromJson(string json)
        {
            return IO.Json.JsonSerializer.FromJsonFunc(json, typeof(MappedModel)) as MappedModel;
        }
    }
}