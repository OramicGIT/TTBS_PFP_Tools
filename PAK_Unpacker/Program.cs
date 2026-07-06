using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TtbsUnpacker
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string filePath = args.Length > 0 ? args[0] : "";
            
            while (!File.Exists(filePath))
            {
                Console.Write("Enter the path to the .pfp file: ");
                filePath = Console.ReadLine()?.Trim('\"');
            }

            using (BinaryReader binaryReader = new BinaryReader(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                byte[] signature = binaryReader.ReadBytes(4);
                if (Encoding.ASCII.GetString(signature) == "PFPK")
                {
                    Encoding enc = Encoding.UTF8;
                    var fileno = binaryReader.ReadInt32();
                    
                    var filepath = new List<string>();
                    var filesize = new List<int>();
                    var filepos = new List<int>();

                    Directory.CreateDirectory("data");
                    
                    for (int i = 0; i < fileno; i++)
                    {
                        var fnmoji = binaryReader.ReadByte();
                        filepath.Add(enc.GetString(binaryReader.ReadBytes(fnmoji)));
                        filepos.Add(binaryReader.ReadInt32());
                        filesize.Add(binaryReader.ReadInt32());
                    }

                    for (int i = 0; i < fileno; i++)
                    {
                        string relativePath = Path.Combine("data", filepath[i].Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar));
                        
                        binaryReader.BaseStream.Seek(filepos[i], SeekOrigin.Begin);
                        var filedata = binaryReader.ReadBytes(filesize[i]);

                        Console.WriteLine($"Extracting: {relativePath}");

                        var folderName = Path.GetDirectoryName(relativePath);
                        if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

                        using (BinaryWriter binaryWriter = new BinaryWriter(new FileStream(relativePath, FileMode.Create, FileAccess.Write)))
                        {
                            binaryWriter.Write(filedata);
                        }
                    }
                    Console.WriteLine("Extraction complete!");
                }
                else
                {
                    Console.WriteLine("Error: The selected file is not a valid *.pfp file.");
                }
            }
        }
    }
}
