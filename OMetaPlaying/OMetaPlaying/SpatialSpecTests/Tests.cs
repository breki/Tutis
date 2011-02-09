using System;
using MbUnit.Framework;
using OMetaSharp;

namespace SpatialSpecTests
{
    public class Tests
    {
        [Test, Ignore]
        public void Test()
        {
            string text = "name";

            string value = Grammars.ParseWith<SpatialSpecParserUsingOMetaSharp>(
                text, 
                p => p.String).As<string>();
            Assert.AreEqual("n", value);
        }

        [Test]
        //[Row("")]
        [Row(" tag =  value ")]
        [Row("tag=value")]
        [Row("tag=addr:post")]
        [Row("tag!=value")]
        [Row("tag!=2")]
        [Row("tag<2")]
        [Row("tag>=0.1223")]
        [Row("tag=\"value\"")]
        [Row("tag=\"\"")]
        [Row("\"tag  f\"=\"value  f\"")]
        [Row("tag=value and tag2=value2")]
        [Row("tag=value tag2=value2")]
        [Row("tag=value AND (tag2=value2 or tag3=value3)")]
        [Row("tag")]
        [Row("target [tag=value]")]
        [Row("target [tag=value] target2[tag=value]")]
        [Row("target [tag=value].target2[tag=value]")]
        [Row(" [] ")]
        [Row("target []")]
        [Row("node[historic=castle AND ruins=yes] area[historic=castle AND ruins=yes]")]
        [Row("[landuse=forest or natural=wood]")]
        [Row("relation [type=route route=foot]")]
        public void ValidSpecs(string spec)
        {
            SpatialSpec parser = new SpatialSpec();
            parser.Parse(spec);
        }

        [Test]
        [Row("tag=")]
        [Row("=value")]
        [Row("=0.1")]
        [Row(">0.1")]
        [Row(">value")]
        [Row("tag<value")]
        [Row("tag>=value")]
        [Row("tag>=\"0.1\"")]
        [Row("way[highway=footpath]..node[barrier=gate]")]
        public void InvalidSpecs(string spec)
        {
            SpatialSpec parser = new SpatialSpec();
            try
            {
                parser.Parse(spec);
                Assert.Fail();
            }
            catch (ParserException)
            {
            }
        }
    }
}