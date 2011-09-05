using System;
using System.IO;

namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// The class is used to count lines of code and gather statistics
    /// about the no. of lines, no. of empty lines and no. of comments.
    /// </summary>
    public class CSharpLocStats : ILocStats
    {
        /// <summary>
        /// This method counts the loc statistics.
        /// </summary>
        /// <param name="stream">The stream representing a single
        /// file from which we count the loc statistics.</param>
        /// <returns>LocStatsData object with the results.</returns>
        public LocStatsData CountLocStream(Stream stream)
        {
            int sloc = 0;
            int cloc = 0;
            int eloc = 0;

            bool inCommentMode = false; //For indicating multiline comments.

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
                    
                    //The start of the comment '//' characters, if any present.
                    int commentIndex = line.IndexOf(@"//", StringComparison.Ordinal);

                    //The start of the 'old' comment '/*' start characters, if any present.
                    int oldIndexStart = line.IndexOf(@"/*", StringComparison.Ordinal);

                    //The start of the 'old' comment '/*' end characters, if any present.
                    int oldIndexEnd = line.IndexOf(@"*/", StringComparison.Ordinal);
                    
                    //Index of quotes if there are any. Used to point oout comments inside strings.
                    int quoteIndex1 = line.IndexOf("\"", StringComparison.Ordinal);
                    int quoteIndex2 = line.LastIndexOf("\"", StringComparison.Ordinal);

                    //First handle the old style commenting '/* */'.
                    if (oldIndexStart > -1 &&               //We found the old comment start sign
                        inCommentMode != true &&            //and we are not already in a comment
                        !(quoteIndex1 < oldIndexStart &&    //and that comment is not inside quotes = string.
                          oldIndexStart < quoteIndex2))
                    {
                        if (commentIndex == -1 ||           //Check that the old comment sign is not after a '//'
                            commentIndex > oldIndexStart)
                        {
                            if (oldIndexEnd > oldIndexStart) //If we have the old comment end sign '*/'in the same line
                                cloc++;                      //we increment the cloc and that's it.
                            else
                                inCommentMode = true;        //Else we mark that also the next line we are in a comment.
                        }
                    }
                    
                    //Now handle the '//' and '///' comments
                    if (commentIndex > -1 &&                    //If we found the comment sign
                        (oldIndexStart == -1 || oldIndexStart > commentIndex) &&  //and it is not inside a comment
                        inCommentMode == false &&               //and we are not already in a comment
                        !(quoteIndex1 < commentIndex &&         //and that comment is not inside quotes = string
                          commentIndex < quoteIndex2))
                        cloc++;                                 //we increment the cloc.

                    //If we found the end of the old style comment
                    if (oldIndexEnd > -1 &&
                        inCommentMode == true)
                    {
                        cloc++;
                        inCommentMode = false;
                    }
                }
            }

            //Return data.
            LocStatsData returnData = new LocStatsData(sloc, cloc, eloc);
            return returnData;
        }
    }
}
