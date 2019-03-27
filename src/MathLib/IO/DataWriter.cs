using System.IO;
using System.Text;

namespace MathLib.IO
{
    public class DataWriter
    {
        public static void CreateDataFile(string fileName, string data)
        {
            File.Delete(fileName);
            FileStream outFile = File.Create(fileName);
            byte[] info = new UTF8Encoding(true).GetBytes(data);
            outFile.Write(info, 0, info.Length);
            outFile.Close();
        }
    }
}
