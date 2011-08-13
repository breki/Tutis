using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework.Reporting
{
    public interface IReport
    {
        GeneratedReport Generate (Book book, ReportParameters parameters);
    }
}
