using System;
using System.Collections.Generic;
using System.IO;
using MbUnit.Framework;
using TreasureChest.Diagnostics;
using TreasureChest.Tests.SampleModule;

namespace TreasureChest.Tests
{
    public class DiagnosticsTests : ChestTestFixtureBase
    {
        [Test]
        public void DumpWholeChest()
        {
            Chest
                .Add<IServiceX, IndependentComponentA> ()
                .Add<IServiceY, ServiceImplYThatDependsOnServiceX> ();

            Lease<IServiceY> lease = Chest.Fetch<IServiceY>();
            
            foreach (KeyValuePair<object, IRegistrationHandler> pair in Chest.DependencyGraph.EnumerateObjects ())
            {
                Console.Out.WriteLine("{0} ({1})", pair.Key, pair.Value);

                Console.Out.WriteLine("depends on:");
                foreach (object obj in Chest.DependencyGraph.GetObjectsThatAreNecessaryFor (pair.Key))
                    Console.Out.WriteLine(obj);

                Console.Out.WriteLine("is necessary for:");
                foreach (object obj in Chest.DependencyGraph.GetObjectsThatDependOn(pair.Key))
                    Console.Out.WriteLine(obj);

                Console.Out.WriteLine ("---");
            }
        }

        [Test]
        public void GenerateDot()
        {
            Chest
                .Add<IServiceX, IndependentComponentA> ()
                .Add<IServiceY, ServiceImplYThatDependsOnServiceX> ();

            Lease<IServiceY> lease = Chest.Fetch<IServiceY> ();
            
            DependencyDotGraphGenerator generator = new DependencyDotGraphGenerator();
            string dot = generator.GenerateDotGraph(Chest);

            File.WriteAllText("graph.dot", dot);
        }
    }
}