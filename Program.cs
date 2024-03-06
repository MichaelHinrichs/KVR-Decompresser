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
            string magic = new(br.ReadChars(4));
            if (magic != "kxrf")
                throw new System.Exception("Not a KVR file.");

            br.BaseStream.Position = 50;
            using FileStream FS = File.Create(Path.GetDirectoryName(args[0]) + "//" + Path.GetFileNameWithoutExtension(args[0]));
            using (var ds = new DeflateStream(new MemoryStream(br.ReadBytes((int)(br.BaseStream.Length - 50))), CompressionMode.Decompress))
                ds.CopyTo(FS);
        }
    }
}
