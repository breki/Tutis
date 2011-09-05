using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// Represents a VisualStudio project.
    /// </summary>
    public class VSProject
    {
        public VSProject(string projectFileName)
        {
            this.projectFileName = projectFileName;
        }

        /// <summary>
        /// Gets a read-only collection of project configurations.
        /// </summary>
        /// <value>A read-only collection of project configurations.</value>
        public IList<VSProjectConfiguration> Configurations
        {
            get { return configurations; }
        }

        /// <summary>
        /// Gets a read-only collection of all .cs files in the solution.
        /// </summary>
        /// <value>A read-only collection of all the .cs files in the solution.</value>
        public IList<VSProjectItem> Items
        {
            get { return items; }
        }

        public string ProjectFileName
        {
            get { return projectFileName; }
        }

        /// <summary>
        /// Gets a read-only collection of project properties.
        /// </summary>
        /// <value>A read-only collection of project properties.</value>
        public IDictionary<string, string> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// Finds the VisualStudio project configuration specified by a condition.
        /// </summary>
        /// <param name="condition">The condition which identifies the configuration 
        /// (example: " '$(Configuration)|$(Platform)' == 'Release|AnyCPU' ").</param>
        /// <returns><see cref="VSProjectConfiguration"/> object if found; <c>null</c> if no configuration was found that meets the
        /// specified condition.</returns>
        public VSProjectConfiguration FindConfiguration (string condition)
        {
            foreach (VSProjectConfiguration configuration in configurations)
            {
                if (configuration.Condition.IndexOf(condition, StringComparison.OrdinalIgnoreCase) >= 0)
                    return configuration;
            }

            return null;
        }

        /// <summary>
        /// Loads the specified project file name.
        /// </summary>
        /// <param name="projectFileName">Name of the project file.</param>
        /// <returns>VSProject class containing project information.</returns>
        public static VSProject Load(string projectFileName)
        {
            using (Stream stream = File.OpenRead (projectFileName))
            {
                VSProject data = new VSProject(projectFileName) { propertiesDictionary = true };

                XmlReaderSettings xmlReaderSettings = new XmlReaderSettings
                                                          {
                                                              IgnoreComments = true,
                                                              IgnoreProcessingInstructions = true,
                                                              IgnoreWhitespace = true
                                                          };

                using (XmlReader xmlReader = XmlReader.Create(stream, xmlReaderSettings))
                {
                    xmlReader.Read();
                    while (false == xmlReader.EOF)
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.XmlDeclaration:
                                xmlReader.Read();
                                break;

                            case XmlNodeType.Element:
                                if (xmlReader.Name == "Project")
                                    data.ReadProject(xmlReader);

                                xmlReader.Read();
                                break;
                            default:
                                xmlReader.Read();
                                continue;
                        }
                    }
                }

                return data;
            }
        }

        /// <summary>
        /// Gets the List of VSProjectItem single type items.
        /// </summary>
        /// <param name="getItemType">Type of the item.</param>
        /// <returns>List of items of specific itemType.</returns>
        public IList<VSProjectItem> GetSingleTypeItems(string getItemType)
        {
            List<VSProjectItem> returnList = new List<VSProjectItem>();
            foreach (VSProjectItem item in Items)
            {
                if (item.ItemType == getItemType)
                    returnList.Add(item);
            }

            return returnList;
        }

        private void ReadProject(XmlReader xmlReader)
        {
            xmlReader.Read();

            while (xmlReader.NodeType != XmlNodeType.EndElement && false == xmlReader.EOF)
            {
                switch (xmlReader.Name)
                {
                    case "PropertyGroup":
                        if (propertiesDictionary)
                        {
                            ReadPropertyGroup(xmlReader);
                            propertiesDictionary = false;
                        }
                        else
                        {
                            configurations.Add(ReadPropertyGroup(xmlReader));
                        }

                        xmlReader.Read();
                        break;
                    case "ItemGroup":
                        ReadItemGroup (xmlReader);
                        xmlReader.Read ();
                        break;
                    default:
                        xmlReader.Read ();
                        continue;
                }
            }
        }

        private VSProjectConfiguration ReadPropertyGroup(XmlReader xmlReader)
        {
            VSProjectConfiguration configuration = new VSProjectConfiguration();

            if (xmlReader["Condition"] != null && propertiesDictionary == false)
            {
                configuration.Condition = xmlReader["Condition"];
            }

            xmlReader.Read();
            
            while (xmlReader.NodeType != XmlNodeType.EndElement)
            {
                if (propertiesDictionary)
                {
                    if (properties.ContainsKey(xmlReader.Name))
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Property '{0}' has already been added to the group. VS project '{1}'",
                                xmlReader.Name, 
                                ProjectFileName));

                    properties.Add(xmlReader.Name, xmlReader.ReadElementContentAsString());
                }
                else
                {
                    if (configuration.Properties.ContainsKey(xmlReader.Name))
                        throw new ArgumentException(
                            string.Format(
                                CultureInfo.InvariantCulture,
                                "Property '{0}' has already been added to the group. VS project '{1}'",
                                xmlReader.Name, 
                                ProjectFileName));

                    configuration.Properties.Add(xmlReader.Name, xmlReader.ReadElementContentAsString());
                }
            }

            return configuration;
        }

        private void ReadItemGroup(XmlReader xmlReader)
        {
            xmlReader.Read();

            while (xmlReader.NodeType != XmlNodeType.EndElement && false == xmlReader.EOF)
            {
                switch (xmlReader.Name)
                {
                    case "Content":
                        VSProjectItem contentItem = ReadItem(xmlReader, VSProjectItem.Content);
                        items.Add(contentItem);
                        break;

                    case "Compile":
                        VSProjectItem compileItems = ReadItem(xmlReader, VSProjectItem.CompileItem);
                        items.Add(compileItems);
                        break;

                    case "None":
                        VSProjectItem noneItem = ReadItem (xmlReader, VSProjectItem.NoneItem);
                        items.Add (noneItem);
                        break;

                    case "ProjectReference":
                        VSProjectItem projectReference = ReadItem (xmlReader, VSProjectItem.ProjectReference);
                        items.Add (projectReference);
                        break;

                    case "Reference":
                        VSProjectItem reference = ReadItem(xmlReader, VSProjectItem.Reference);
                        items.Add(reference);
                        break;

                    default:
                        xmlReader.Skip();
                        continue;
                }
            }
        }

        private static VSProjectItem ReadItem(XmlReader xmlReader, string itemType)
        {
            VSProjectItem item = new VSProjectItem(itemType) { Item = xmlReader["Include"] };

            if (false == xmlReader.IsEmptyElement)
            {
                xmlReader.Read ();

                while (true)
                {
                    if (xmlReader.NodeType == XmlNodeType.EndElement)
                        break;

                    ReadItemProperty(item, xmlReader);
                }
            }

            xmlReader.Read();

            return item;
        }

        private static void ReadItemProperty(VSProjectItem item, XmlReader xmlReader)
        {
            string propertyName = xmlReader.Name;
            string propertyValue = xmlReader.ReadElementContentAsString();
            item.ItemProperties.Add(propertyName, propertyValue);
        }

        private readonly List<VSProjectConfiguration> configurations = new List<VSProjectConfiguration>();
        private readonly List<VSProjectItem> items = new List<VSProjectItem>();
        private readonly string projectFileName;
        private readonly Dictionary<string, string> properties = new Dictionary<string, string>();
        private bool propertiesDictionary;
    }
}