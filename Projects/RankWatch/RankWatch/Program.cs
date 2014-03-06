using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace RankWatch
{
    public class Program
    {
        public static void Main (string[] args)
        {
            JsonSerializer serializer = new JsonSerializer ();
            serializer.Culture = CultureInfo.InvariantCulture;
            serializer.Formatting = Formatting.Indented;

            DateTime now = DateTime.Now;
            IEnumerable<string> keywords = LoadKeywords(args[0]);

            string historyFileName = args[1];
            RanksDb db;

            if (File.Exists(historyFileName))
            {
                using (Stream file = File.Open(historyFileName, FileMode.Open, FileAccess.Read))
                using (StreamReader sr = new StreamReader(file, Encoding.UTF8))
                using (JsonReader jr = new JsonTextReader(sr))
                    db = serializer.Deserialize<RanksDb>(jr);
            }
            else
                db = new RanksDb();

            GoogleSearchRequestBuilder requestBuilder = new GoogleSearchRequestBuilder ();
            RankFinder rankFinder = new RankFinder();
            foreach (string keyword in keywords)
            {
                RankInfo rank = rankFinder.FindRank(requestBuilder, keyword, true, 10);

                if (rank.Ranks.Count > 0)
                {
                    db.AddRank(now, keyword, rank.Ranks[0]);
                    UpdateHistoryFile (historyFileName, serializer, db);
                }
            }
        }

        private static IEnumerable<string> LoadKeywords(string fileName)
        {
            return File.ReadAllLines(fileName).Where(x => x.Trim().Length > 0).ToList();
        }

        private static void UpdateHistoryFile(string historyFileName, JsonSerializer serializer, RanksDb db)
        {
            string historyDir = Path.GetDirectoryName(historyFileName);
            if (!String.IsNullOrWhiteSpace(historyDir) && !Directory.Exists(historyDir))
                Directory.CreateDirectory(historyDir);

            using (Stream file = File.Open(historyFileName, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(file, Encoding.UTF8))
                serializer.Serialize(sw, db);
        }
    }
}
