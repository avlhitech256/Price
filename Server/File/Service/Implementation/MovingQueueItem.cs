using System.Collections.Generic;

namespace File.Service.Implementation
{
    public class MovingQueueItem
    {
        #region Constructors

        public MovingQueueItem(string sourcePath, string destinationPath,
                                IEnumerable<string> searchPatterns = null)
        {
            SourcePath = sourcePath;
            DestinationPath = destinationPath;
            SearchPatterns = searchPatterns;
        }

        #endregion

        #region Properties

        public string SourcePath { get; set; }

        public string DestinationPath { get; set; }

        public IEnumerable<string> SearchPatterns { get; }

        #endregion
    }
}
