using Brejc.Common.FileSystem;

namespace SamsungTvChannelsTool
{
    public class ApplyChannelsOrderCommand : ICommand
    {
        public ApplyChannelsOrderCommand(IFileSystem fileSystem, IZipper zipper)
        {
            this.fileSystem = fileSystem;
            this.zipper = zipper;
            throw new System.NotImplementedException();
        }

        public string CommandName
        {
            get { return "apply-channels"; }
        }

        public string CommandUsage
        {
            get { return "apply-channels <channels file> <scm file>"; }
        }

        public string CommandDescription
        {
            get { return "applies the channels order file to the specified .scm file"; }
        }

        public int Execute(string[] args)
        {
            // read channels order file

            // unzip the SCM file

            // read all records from the CableD and index them by the channel name

            // start preparing a new CableD file
            // for each record in channels order file
            {
                // if the record is not empty, insert the CableD record (with the channel number and checksum corrected)
                // if the record is empty, insert an empty CableD record
            }

            // save the file in place of the existing one

            // zip back the SCM file
            throw new System.NotImplementedException();
        }

        private readonly IFileSystem fileSystem;
        private readonly IZipper zipper;
    }
}