using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ChaosSoft.Core.IO
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

        public static void CreateDataFile(string fileName, double[,] data)
        {
            var output = new StringBuilder();

            for (int i = 0; i < data.GetLength(1); i++)
            {
                for (int j = 0; j < data.GetLength(0); j++)
                {
                    output.Append($"{NumFormatter.ToShort(data[j, i])}\t");
                }

                output.AppendLine();
            }

            CreateDataFile(fileName, output.ToString());
        }

        public static void CreateBytesDataFile(string fileName, double[][] data)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                new BinaryFormatter().Serialize(ms, data);
                byte[] bytes = ms.ToArray();

                using (FileStream fs = new FileStream(fileName, FileMode.Create))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Close();
                }
            }
        }
    }
}
