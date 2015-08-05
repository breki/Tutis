﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Freude.DocModel;

namespace Freude.ProjectServices
{
    [ContractClass(typeof(IProjectBuilderContract))]
    public interface IProjectBuilder
    {
        IEnumerable<string> ListBuiltFiles(FreudeProject project);
    }

    [ContractClassFor(typeof(IProjectBuilder))]
    internal abstract class IProjectBuilderContract : IProjectBuilder
    {
        IEnumerable<string> IProjectBuilder.ListBuiltFiles(FreudeProject project)
        {
            Contract.Requires(project != null);
            Contract.Ensures(Contract.Result<IEnumerable<string>>() != null);
            throw new System.NotImplementedException();
        }
    }
}