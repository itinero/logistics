﻿using System;
using System.Linq;
using Itinero.LocalGeo;
using Itinero.Optimization.Models.Mapping;
using Itinero.Optimization.Models.Mapping.Directed;
using Itinero.Optimization.Models.Mapping.Directed.Simplified;
using Itinero.Optimization.Models.Mapping.Rewriters;
using Itinero.Optimization.Models.Mapping.Rewriters.VisitPosition;

namespace Itinero.Optimization.Tests.Functional.TSP_D
{
    public static class TSPDTests
    {
        /// <summary>
        /// Runs some functional tests related to the TSPD.
        /// </summary>
        public static void Run()
        {
            Run1Wechelderzande();
            Run2Hengelo();
            Run3Problem1Spijkenisse();
            Run4Problem2Hengelo();
            Run5Problem3();
            Run6Problem4WechelderzandeWithSimplifcation();
            Run7WechelderzandeNoLeftVisits();
            Run8WechelderzandeNoLeftVisits();
            Run9Problem1Spijkenisse();
        }

        public static void Run1Wechelderzande()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // define test locations.
            var locations = new []
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(4.7884941101074220f, 51.256489871317920f), // non-resolvable location.
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(4.779181480407715f, 51.26855085674035f), // non-resolvable location.
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.299771179035815f, 4.7829365730285645f), // non-routable location.
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f),
                new Coordinate(51.252984777835955f, 4.776681661605835f)
            };

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, 0, turnPenalty: 60));
            func.Run("TSPD-1-wechel-closed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60));
            func.Run("TSPD-1-wechel-fixed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, null, turnPenalty: 60));
            func.Run("TSPD-1-wechel-open");
        }

        public static void Run2Hengelo()
        {
            // HENGELO
            // build routerdb and save the result.
            var hengelo = Staging.RouterDbBuilder.Build("query2");
            var vehicle = hengelo.GetSupportedVehicle("car");
            var router = new Router(hengelo);

            var locations = new Coordinate[]
            {
                new Coordinate(52.286143714914566f, 6.776547431945801f),
                new Coordinate(52.2790550797244f, 6.791224479675293f),
                new Coordinate(52.270363390765176f, 6.796717643737792f),
                new Coordinate(52.265294640450435f, 6.777276992797852f),
                new Coordinate(52.25807132666112f, 6.78380012512207f),
                new Coordinate(52.25851789290645f, 6.7974042892456055f),
                new Coordinate(52.26810484821601f, 6.8122100830078125f),
                new Coordinate(52.261433597272266f, 6.819033622741699f),
                new Coordinate(52.25318507282242f, 6.798820495605469f),
                new Coordinate(52.24934924915677f, 6.809506416320801f),
                new Coordinate(52.2905013499346f, 6.760196685791016f)
            };

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, 0, turnPenalty: 60));
            func.Run("TSPD-2-hengelo-closed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60));
            func.Run("TSPD-2-hengelo-fixed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, null, turnPenalty: 60));
            func.Run("TSPD-2-hengelo-open");  
        }

        public static void Run3Problem1Spijkenisse()
        {
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);
            
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("TSP_D.data.problem1-spijkenisse.geojson"));  

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, null, turnPenalty: 60));
            func.Run("TSPD-3-spijkenisse-open");  
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, 0, turnPenalty: 60));
            func.Run("TSPD-3-spijkenisse-closed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60));
            func.Run("TSPD-3-spijkenisse-fixed");
        }

        public static void Run4Problem2Hengelo()
        {
            // build routerdb and save the result.
            var hengelo = Staging.RouterDbBuilder.Build("query2");
            var vehicle = hengelo.GetSupportedVehicle("car");
            var router = new Router(hengelo);

            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("TSP_D.data.problem2-hengelo.geojson"));

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, null, turnPenalty: 60));
            func.Run("TSPD-4-hengelo-open");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, 0, turnPenalty: 60));
            func.Run("TSPD-4-hengelo-closed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60));
            func.Run("TSPD-4-hengelo-fixed");
        }

        public static void Run5Problem3()
        {
            // build routerdb and save the result.
            var hengelo = Staging.RouterDbBuilder.Build("query4");
            var vehicle = hengelo.GetSupportedVehicle("car");
            var router = new Router(hengelo);

            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("TSP_D.data.problem3.geojson"));

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, null, turnPenalty: 60));
            func.Run("TSPD-5-problem3-open");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, 0, turnPenalty: 60));
            func.Run("TSPD-5-problem3-closed");
            func = new Func<Result<Route>>(() => router.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60));
            func.Run("TSPD-5-problem3-fixed");
        }

        public static void Run6Problem4WechelderzandeWithSimplifcation()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // get test locations.
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("TSP_D.data.problem4-wechelderzande.geojson"));
            
            // create and configure the optimizer.
            var optimizer = router.Optimizer(new OptimizerConfiguration(modelMapperRegistry: new ModelMapperRegistry(
                ByEdgeDirectedModelMapper.Default)));

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, 0, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-6-wechel-closed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-6-wechel-fixed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, null, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-6-wechel-open");
        }
        
        

        public static void Run7WechelderzandeNoLeftVisits()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // define test locations.
            var locations = new []
            {
                new Coordinate(51.26783594640662f, 4.793686866760254f),
                new Coordinate(51.26631210979951f, 4.789481163024902f),
            };

            // add a new model mapper
            var optimizerConfiguration = new OptimizerConfiguration(modelMapperRegistry: 
                new ModelMapperRegistry(new ModelMapperWithRewriter(DirectedModelMapper.Default, new VisitPositionRewriter())));
            var optimizer = router.Optimizer(configuration: optimizerConfiguration);
            
            // calculate TSPD.
            var func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, 0, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-7-wechel-no-left-closed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-7-wechel-no-left-fixed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, null, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-7-wechel-no-left-open");
        }
        
        public static void Run8WechelderzandeNoLeftVisits()
        {
            // WECHELDERZANDE
            // build routerdb and save the result.
            var wechelderzande = Staging.RouterDbBuilder.Build("query1");
            var router = new Router(wechelderzande);

            // define test locations.
            var locations = new []
            {
                new Coordinate(51.270453873703080f, 4.8008108139038080f),
                new Coordinate(51.264197451065370f, 4.8017120361328125f),
                new Coordinate(51.267446600889850f, 4.7830009460449220f),
                new Coordinate(51.260733228426076f, 4.7796106338500980f),
                new Coordinate(51.256489871317920f, 4.7884941101074220f),
                new Coordinate(4.7884941101074220f, 51.256489871317920f), // non-resolvable location.
                new Coordinate(51.270964016530680f, 4.7894811630249020f),
                new Coordinate(51.26216325894976f, 4.779932498931885f),
                new Coordinate(51.26579184564325f, 4.777781367301941f),
                new Coordinate(4.779181480407715f, 51.26855085674035f), // non-resolvable location.
                new Coordinate(51.26855085674035f, 4.779181480407715f),
                new Coordinate(51.26906437701784f, 4.7879791259765625f),
                new Coordinate(51.259820134021695f, 4.7985148429870605f),
                new Coordinate(51.257040455587656f, 4.780147075653076f),
                new Coordinate(51.299771179035815f, 4.7829365730285645f), // non-routable location.
                new Coordinate(51.256248149311716f, 4.788386821746826f),
                new Coordinate(51.270054481615624f, 4.799646735191345f),
                new Coordinate(51.252984777835955f, 4.776681661605835f)
            };

            // add a new model mapper
            var optimizerConfiguration = new OptimizerConfiguration(modelMapperRegistry: 
                new ModelMapperRegistry(new ModelMapperWithRewriter(DirectedModelMapper.Default, new VisitPositionRewriter())));
            var optimizer = router.Optimizer(configuration: optimizerConfiguration);
            
            // calculate TSPD.
            var func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, 0, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-8-wechel-no-left-closed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-8-wechel-no-left-fixed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, null, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-8-wechel-no-left-open");
        }

        public static void Run9Problem1Spijkenisse()
        {
            // build routerdb and save the result.
            var spijkenisse = Staging.RouterDbBuilder.Build("query4");
            var vehicle = spijkenisse.GetSupportedVehicle("car");
            var router = new Router(spijkenisse);
            
            var locations = Staging.StagingHelpers.GetLocations(
                Staging.StagingHelpers.GetFeatureCollection("TSP_D.data.problem1-spijkenisse.geojson"));  
            
            // add a new model mapper
            var optimizerConfiguration = new OptimizerConfiguration(modelMapperRegistry: 
                new ModelMapperRegistry(new ModelMapperWithRewriter(DirectedModelMapper.Default, new VisitPositionRewriter())));
            var optimizer = router.Optimizer(configuration: optimizerConfiguration);

            // calculate TSPD.
            var func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, null, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-9-spijkenisse-no-left-open");  
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, 0, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-9-spijkenisse-no-left-closed");
            func = new Func<Result<Route>>(() => optimizer.Optimize("car", locations, out _, 0, locations.Length - 1, turnPenalty: 60).GetRoutes().First());
            func.Run("TSPD-9-spijkenisse-no-left-fixed");
        }
    }
}