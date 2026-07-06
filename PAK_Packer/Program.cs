// TtbsPacker.Program
using System;
using System.IO;
using System.Text;

internal class Program
{
	private static void Main(string[] args)
	{
		Console.WriteLine("Enter path you want to pack:");
		string text = Console.ReadLine();
		if (string.IsNullOrWhiteSpace(text) || !Directory.Exists(text))
		{
			Console.WriteLine("Error: This path doesn't exist'.");
			Console.ReadKey();
			return;
		}
		int num = 0;
		string[] files = Directory.GetFiles(text, "*", SearchOption.AllDirectories);
		string path = Path.Combine(text, "..", "data.pfp.new");
		using (FileStream output = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
		{
			using BinaryWriter binaryWriter = new BinaryWriter(output, Encoding.UTF8);
			binaryWriter.Write(Encoding.ASCII.GetBytes("PFPK"));
			binaryWriter.Write(files.Length);
			num += 8;
			string[] array = files;
			foreach (string path2 in array)
			{
				string s = ConvertPath(text, path2);
				num += 1 + Encoding.UTF8.GetBytes(s).Length + 4 + 4;
			}
			string[] array2 = files;
			foreach (string text2 in array2)
			{
				string text3 = ConvertPath(text, text2);
				binaryWriter.Write((byte)text3.Length);
				binaryWriter.Write(Encoding.UTF8.GetBytes(text3));
				binaryWriter.Write(num);
				int num2 = (int)new FileInfo(text2).Length;
				binaryWriter.Write(num2);
				num += num2;
			}
			string[] array3 = files;
			foreach (string path3 in array3)
			{
				using FileStream fileStream = new FileStream(path3, FileMode.Open, FileAccess.Read);
				byte[] array4 = new byte[fileStream.Length];
				fileStream.Read(array4, 0, array4.Length);
				binaryWriter.Write(array4);
			}
			Console.WriteLine("Packed successfully.");
		}
		Console.WriteLine("Press any key to exit...");
		Console.ReadKey();
	}

	private static string ConvertPath(string selectedFolder, string path)
	{
		return path.Replace(selectedFolder, string.Empty).Replace('\\', '/').TrimStart('/');
	}
}

