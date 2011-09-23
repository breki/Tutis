using System;
using System.Diagnostics.CodeAnalysis;

namespace LineByLine.Console.VSSolutionBrowsing
{
    public abstract class VSProjectInfo
    {
        public VSSolution OwnerSolution
        {
            get { return ownerSolution; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public Guid ProjectGuid
        {
            get { return projectGuid; }
        }

        public string ProjectName
        {
            get { return projectName; }
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public Guid ProjectTypeGuid
        {
            get { return projectTypeGuid; }
        }

        public abstract void Parse (VSSolutionFileParser parser);

        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        protected VSProjectInfo(
            VSSolution ownerSolution,
            Guid projectGuid,
            string projectName,
            Guid projectTypeGuid)
        {
            this.ownerSolution = ownerSolution;
            this.projectTypeGuid = projectTypeGuid;
            this.projectName = projectName;
            this.projectGuid = projectGuid;
        }

        private readonly VSSolution ownerSolution;
        private readonly Guid projectGuid;
        private readonly string projectName;
        private readonly Guid projectTypeGuid;
    }
}
