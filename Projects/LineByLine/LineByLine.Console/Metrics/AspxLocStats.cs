using System;
using System.IO;

namespace LineByLine.Console.Metrics
{
    public class AspxLocStats : ILocStats
    {
        public LocStatsData CountLocStream(Stream stream)
        {
            int sloc = 0;
            int cloc = 0;
            int eloc = 0;

            bool inCommentMode = false;

            using (StreamReader reader = new StreamReader(stream))
            {
                //Looping through the file stream.
                while (true)
                {
                    string line = reader.ReadLine();

                    if (line == null) //End of file.
                        break;

                    if (line.Trim().Length == 0) //Empty line
                        eloc++;

                    sloc++; //Incrementig the sloc at each iteration.

                    //if we are in a multi line comment
                    if (inCommentMode == true)
                        cloc++;

                    int commentInIndex = line.IndexOf(@"<%--", StringComparison.Ordinal);
                    int commentOutIndex = line.IndexOf(@"--%>", StringComparison.Ordinal);

                    if (commentInIndex >= 0 &&
                        inCommentMode == false)
                    {
                        cloc++;
                        if (commentOutIndex < commentInIndex)
                            inCommentMode = true;
                    }

                    if (commentOutIndex >= 0 &&
                        inCommentMode == true)
                        inCommentMode = false;
                }
            }
            
            LocStatsData returnData = new LocStatsData(sloc, cloc, eloc);
            return returnData;
        }
    }
}
