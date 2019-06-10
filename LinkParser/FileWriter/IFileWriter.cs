using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LinkParser.FileWriter
{
    /// <summary>
    /// Provides methods to write results to file
    /// </summary>
    public interface IResultWriter
    {
        /// <summary>
        /// Write content to specific file wich is provided into class
        /// </summary>
        Task Write(List<string> content);
    }
}
