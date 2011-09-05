﻿using System.Collections.Generic;

namespace LineByLine.Console.VSSolutionBrowsing
{
    /// <summary>
    /// Holds information about content items inside of a VisualStudio project.
    /// </summary>
    public class VSProjectItem
    {
        public VSProjectItem(string itemType)
        {
            this.itemType = itemType;
        }

        public string Item
        {
            get { return item; }
            set { item = value; }
        }

        public string ItemType
        {
            get { return itemType; }
        }

        public IDictionary<string, string> ItemProperties
        {
            get { return itemProperties; }
        }

        private string item;
        private readonly Dictionary<string, string> itemProperties = new Dictionary<string, string>();
        private readonly string itemType;
        public const string Content = "Content";
        public const string CompileItem = "CompileItem";
        public const string NoneItem = "None";
        public const string ProjectReference = "ProjectReference";
        public const string Reference = "Reference";
    }
}