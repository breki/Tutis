﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;
using Freude.DocModel;

namespace Freude.Parsing
{
    public class InternalLinkIdBuilder
    {
        public void AppendText(string text)
        {
            Contract.Requires(text != null);
            currentPartBuilder.Append(text);
        }

        public void AddSeparator()
        {
            FinalizeCurrentPart();
        }

        public InternalLinkId Build(ParsingContext context)
        {
            Contract.Requires(context != null);

            if (currentPartBuilder.Length > 0)
                FinalizeCurrentPart();

            if (parts.Count == 0)
            {
                context.ReportError("Empty internal link");
                return null;
            }

            List<string> namespaceParts = new List<string>();

            for (int i = 0; i < parts.Count - 1; i++)
            {
                string partText = parts[i].Trim();

                if (partText.Length == 0)
                {
                    context.ReportError("Internal link has an empty namespace");
                    return null;
                }

                namespaceParts.Add(partText);
            }

            string pageName = parts[parts.Count - 1].Trim();

            if (pageName.Length == 0)
            {
                context.ReportError ("Internal link has an empty page name");
                return null;
            }
            
            return new InternalLinkId(pageName, namespaceParts);
        }

        private void FinalizeCurrentPart()
        {
            parts.Add(currentPartBuilder.ToString());
            currentPartBuilder.Clear();
        }

        private readonly List<string> parts = new List<string>();
        private readonly StringBuilder currentPartBuilder = new StringBuilder();
    }
}