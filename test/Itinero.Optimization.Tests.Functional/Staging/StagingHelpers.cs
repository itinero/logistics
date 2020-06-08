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
using System.IO;
using Itinero.Optimization.Models.TimeWindows;
using Itinero.Optimization.Models.Visits;
using Itinero.Optimization.Tests.Functional.Performance;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Itinero.Optimization.Tests.Functional.Staging
{    
    /// <summary>
    /// Some helper methods to setup test-problems.
    /// </summary>
    public static class StagingHelpers
    {
        public static string EmbeddedResourceRoot = "Itinero.Optimization.Tests.Functional.";

        /// <summary>
        /// Gets a feature collection for the given embedded resource.
        /// </summary>
        public static FeatureCollection GetFeatureCollection(this string embeddedResourcePath)
        {
            using var stream = typeof(PerformanceInfoConsumer).Assembly.GetManifestResourceStream(EmbeddedResourceRoot + embeddedResourcePath);
            using var streamReader = new StreamReader(stream);
            var json = streamReader.ReadToEnd();
            return json.ToFeatures();
        }
        
        /// <summary>
        /// Extracts an array of Itinero coordinates from the given feature collection.
        /// </summary>
        public static Itinero.LocalGeo.Coordinate[] GetLocations(this FeatureCollection features)
        {
            var locations = new List<Itinero.LocalGeo.Coordinate>();

            foreach (var feature in features.Features)
            {
                if (feature.Geometry is Point)
                {
                    locations.Add(new Itinero.LocalGeo.Coordinate((float)feature.Geometry.Coordinate.Y,
                        (float)feature.Geometry.Coordinate.X));
                }
            }
            return locations.ToArray();
        }
        
        /// <summary>
        /// Extracts an array of visits and details encoded on the features from the given feature collection.
        /// </summary>
        public static Visit[] GetVisits(this FeatureCollection features)
        {
            var visits = new List<Visit>();

            foreach (var feature in features.Features)
            {
                if (!(feature.Geometry is Point)) continue;
                
                var visit = new Visit()
                {
                    Longitude = (float)feature.Geometry.Coordinate.X,
                    Latitude = (float)feature.Geometry.Coordinate.Y
                };

                if (feature.Attributes.TryGetValueInt32("time-window-start", out var timeWindowStart) &&
                    feature.Attributes.TryGetValueInt32("time-window-end", out var timeWindowEnd))
                {
                    visit.TimeWindow = new TimeWindow()
                    {
                        Times = new []{ (float)timeWindowStart, timeWindowEnd}
                    };
                }

                visits.Add(visit);
            }
            return visits.ToArray();
        }
    }
}