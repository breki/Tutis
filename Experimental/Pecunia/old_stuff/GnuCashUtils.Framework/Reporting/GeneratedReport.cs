using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework.Reporting
{
    public class GeneratedReport
    {
        public string ImageFileName
        {
            get { return imageFileName; }
            set { imageFileName = value; }
        }

        public string ThumbnailFileName
        {
            get { return thumbnailFileName; }
            set { thumbnailFileName = value; }
        }

        private string imageFileName;
        private string thumbnailFileName;
    }
}
