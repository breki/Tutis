using System.Collections;
using MongoDB.Bson;

namespace NoSqlPlaying
{
    public class Node
    {
        public ObjectId _id { get; set; }
        public int OsmId { get; set; }
        //public Point Loc { get; set; }
        //public Hashtable Tags { get; set; }
    }

    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
    }
}