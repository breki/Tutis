/*Sample file for counting the number of comment in .cs files
 The result should be:
 Single lines of code (sloc):   56
 comment lines of code (cloc):  14 
 Empty lines of code: (eloc):    9 */

using System;

// Single comment
namespace ProjectPilot.Framework.Metrics
{
    /** multiline xml comment: //not another comment
      another line of comment /*not another comment
      finished: */
    public class LocStatsData
    {
        
        /*old style single line comment*/
        public LocStatsData(int sloc, int cloc, int eloc)
        {
            this.sloc = sloc;
            this.cloc = cloc;
            this.eloc = eloc;
        }

        /// xml comment
        public int Cloc
        {
            get { return cloc; }
            set { cloc = value; }
        }

        //old style comment /*inside single line comment*/ should't be counted
        public int Eloc
        {
            get { return eloc; }
            set { eloc = value; }
        }

        //multiple single line comments //only one should be //counted
        public int Sloc
        {
            get { return sloc; }
            set { sloc = value; }
        }
        //empty line with spaces or tabs should also be counted as eloc
                            
        string text = "/*not a comment*/";
        string text = "//not a comment";
        
        private int cloc;
        private int eloc;
        private int sloc;
        
    }
}
