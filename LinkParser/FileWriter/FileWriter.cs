using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LinkParser.FileWriter
{
    /// <summary>
    /// Provides methods to write results to file
    /// </summary>
    public class ResultFileWriter : IResultWriter
    {
        private string _filePath = "result.txt";
        
        public async Task WriteAsync(List<string> content)
        {
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), _filePath);
            using (FileStream fstream = new FileStream(filePath, FileMode.Create))
            {
                try
                {
                    byte[] array = System.Text.Encoding.Default.GetBytes(string.Join(System.Environment.NewLine, content));
                    await fstream.WriteAsync(array, 0, array.Length);
                    Console.WriteLine($"Result has been written to file {filePath}");
                }
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e);
                }
                catch (IOException e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}
