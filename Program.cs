//Written for Onigiri. https://store.steampowered.com/app/290470/
using System.IO;
using System.IO.Compression;

namespace KXR_Decompresser
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new BinaryReader(File.OpenRead(args[0]));
            if (new string(br.ReadChars(4)) != "kxrf")
                throw new System.Exception("Not a KXR file.");

            br.BaseStream.Position = 47;
            byte[] search = br.ReadBytes(3);
            string searchString;
            System.Collections.Generic.List<long> starts = new();
            while (br.BaseStream.Position < br.BaseStream.Length)
            {
                searchString = System.Text.Encoding.ASCII.GetString(search);
                if (System.Text.RegularExpressions.Regex.Match(searchString, @"\0x\^").Success)
                    starts.Add(br.BaseStream.Position);

                br.BaseStream.Position -= 2;
                search = br.ReadBytes(3);
            }

            string path = Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]);
            Directory.CreateDirectory(path);

            for (int i = 0; i < starts.Count - 1; i++)
            {
                br.BaseStream.Position = starts[i];
                if (i == 186)
                    i = 186;

                using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)((starts[i + 1] - 2) - starts[i]))), CompressionMode.Decompress))
                    ds.CopyTo(File.Create(path + "//" + i));
            }

            br.BaseStream.Position = starts[^1];
            using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)(br.BaseStream.Length - starts[^1]))), CompressionMode.Decompress))
                ds.CopyTo(File.Create(path + "//" + (starts.Count - 1)));
        }
    }
}
