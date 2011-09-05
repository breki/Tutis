namespace LineByLine.Console.Metrics
{
    /// <summary>
    /// Object class for holding lines of code data.
    /// </summary>
    public class LocStatsData
    {
        public LocStatsData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocStatsData"/> class.
        /// </summary>
        /// <param name="sloc">Single llines of code.</param>
        /// <param name="cloc">Comment lines of code.</param>
        /// <param name="eloc">Empty lines of code.</param>
        public LocStatsData(int sloc, int cloc, int eloc)
        {
            this.sloc = sloc;
            this.cloc = cloc;
            this.eloc = eloc;
        }

        /// <summary>
        /// Gets or sets the cloc.
        /// </summary>
        public int Cloc
        {
            get { return cloc; }
            set { cloc = value; }
        }

        /// <summary>
        /// Gets or sets the eloc.
        /// </summary>
        public int Eloc
        {
            get { return eloc; }
            set { eloc = value; }
        }

        /// <summary>
        /// Gets or sets the sloc.
        /// </summary>
        public int Sloc
        {
            get { return sloc; }
            set { sloc = value; }
        }

        public void Add(LocStatsData locData)
        {
            this.Cloc += locData.Cloc;
            this.Eloc += locData.Eloc;
            this.Sloc += locData.Sloc;
        }

        private int cloc;
        private int eloc;
        private int sloc;
    }
}
