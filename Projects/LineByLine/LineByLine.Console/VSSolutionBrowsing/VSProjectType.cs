using System;
using System.Diagnostics.CodeAnalysis;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// Contains information about a specific VisualStudio project type.
    /// </summary>
    public class VSProjectType : IEquatable<VSProjectType>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VSProjectType"/> class.
        /// </summary>
        /// <param name="projectTypeGuid">The project type GUID.</param>
        /// <param name="projectTypeName">Name of the project type.</param>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public VSProjectType(Guid projectTypeGuid, string projectTypeName)
        {
            this.projectTypeGuid = projectTypeGuid;
            this.projectTypeName = projectTypeName;
        }

        /// <summary>
        /// Gets the <see cref="VSProjectType"/> for C# projects.
        /// </summary>
        /// <value>The <see cref="VSProjectType"/> for C# projects.</value>
        public static VSProjectType CSharpProjectType
        {
            get { return cSharpProjectType; }
        }

        /// <summary>
        /// Gets the project type GUID.
        /// </summary>
        /// <value>The project type GUID.</value>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "guid")]
        public Guid ProjectTypeGuid
        {
            get { return projectTypeGuid; }
        }

        /// <summary>
        /// Gets the name of the project type.
        /// </summary>
        /// <value>The name of the project type.</value>
        public string ProjectTypeName
        {
            get { return projectTypeName; }
        }

        /// <summary>
        /// Gets the <see cref="VSProjectType"/> for solution folders.
        /// </summary>
        /// <value>The <see cref="VSProjectType"/> for solution folders.</value>
        public static VSProjectType SolutionFolderProjectType
        {
            get { return solutionFolderProjectType; }
        }

        /// <summary>
        ///     Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other" /> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">
        ///     An object to compare with this object.
        /// </param>
        public bool Equals(VSProjectType other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.projectTypeGuid.Equals(projectTypeGuid);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof(VSProjectType)) 
                return false;
            return Equals((VSProjectType)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return projectTypeGuid.GetHashCode();
        }

        /// <summary>
        /// Compares the two objects.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator ==(VSProjectType left, VSProjectType right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// Compares the two objects.
        /// </summary>
        /// <param name="left">The left object.</param>
        /// <param name="right">The right object.</param>
        /// <returns>The result of the operator.</returns>
        public static bool operator !=(VSProjectType left, VSProjectType right)
        {
            return !Equals(left, right);
        }

        private static readonly VSProjectType cSharpProjectType = new VSProjectType(
            new Guid("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"),
            "C# Project");

        private static readonly VSProjectType solutionFolderProjectType = new VSProjectType(
            new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}"),
            "Solution Folder");

        private readonly Guid projectTypeGuid;
        private readonly string projectTypeName;
    }
}