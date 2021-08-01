using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoGeo
{
    public abstract class ViewController
    {
        public abstract void LogPrintf(string value);
        public abstract void LogSuccess(string value);
        public abstract void LogError(string value);
        public abstract void ReportProgress(int value, int max);
        public abstract void CheckOut(string szInput);
    }

    public enum Genre
    {
        Other = 0, Action, BeatEmUp, Sports, Driving, Platformer, Mahjong, Shooter, Quiz, Fighting, Puzzle
    };
    public class Header
    {
       
        public Header()
        {
            header1 = 78;
            header2 = 69;
            header3 = 79;
            version = 1;
            Year = 2020;
            NGH = 0;
            Screenshot = 0;
            Genre = Genre.Other;
            Name = "New Game";
            Manu = "SNK";
        }

        public byte header1, header2, header3, version;
        public UInt32 PSize { get; set; } // xxxx-p1,2.bin for program data (max 1+8MB)
        public UInt32 SSize { get; set; } //  xxxx-s1.bin for FIX (tiles) data (max 512KB, CMC42 style banking is enabled by NeoSD by default for unknown games)
        public UInt32 MSize { get; set; } // xxxx-m1.bin for Z80 program (max 256KB)  
        public UInt32 V1Size { get; set; } // xxxx-v1,2,3,4.bin for ADPCM samples (max 16MB). Combined ADPCM-A and ADPCM-B regions as if NEO-PCM chip was present in the cart.
        public UInt32 V2Size { get; set; }
        public UInt32 CSize { get; set; } // xxxx-c1,2,34,5,6,7,8.bin for sprites (max 64MB)

        public UInt32 Year { get; set; }
        public Genre Genre { get; set; }
        public UInt32 Screenshot { get; set; }
        public UInt32 NGH { get; set; }
        public string Name { get; set; } // [33];
        public string Manu { get; set; } // [17];
        public byte[] Filler; // [128 + 290];  //fill to 512
        public byte[] Filler2; //[4096 - 512];	//fill to 4096

    }

    public class NeoData
    {
        static public string IdentifyBank(string filename)
        {
            if (filename.EndsWith("p1.bin"))
                return "P";
            if (filename.EndsWith("p2.bin"))
                return "P";
            if (filename.EndsWith("s1.bin"))
                return "S";
            if (filename.EndsWith("m1.bin"))
                return "M";
            if (filename.EndsWith("-c1.bin") ||
                filename.EndsWith("-c2.bin") ||
                filename.EndsWith("-c3.bin") ||
                filename.EndsWith("-c4.bin") ||
                filename.EndsWith("-c5.bin") ||
                filename.EndsWith("-c6.bin") ||
                filename.EndsWith("-c7.bin") ||
                filename.EndsWith("-c8.bin")
                )
                return "C";
            return null;
        }
        public NeoData(string _Name, UInt32 _length, UInt32 _offset, int _index)
        {
            Name = _Name;
            Offset = _offset;
            Length = _length;
            Index = _index;
        }

        public NeoData(string _Name, UInt32 _length, UInt32 _offset, int _index, byte[] fileBytes)
        {
            Name = _Name;
            Offset = _offset;
            Length = _length;
            Index = _index;

            Bytes = new byte[_length];
            Buffer.BlockCopy(fileBytes, (int)_offset, Bytes, 0, (int)_length);
        }
        public override string ToString()
        {
            return Name;
        }
        byte[] Bytes;
        public int Index { get; set; }
        public UInt32 Offset { get; set; }
        public UInt32 Length { get; set; }
        public string Name { get; set; }
        public string LengthHeader
        {
            get
            {
                return String.Format("{0:n0}", Length);
            }
        }
        public string Key
        {
            get
            {
                return Name;
            }
        }

        internal void Write(BinaryWriter bw)
        {
            Bytes = File.ReadAllBytes(Name);
            bw.Write(Bytes, 0, (int) Length);
        }
    }

    public class NeoFile
    {
        public NeoFile(ViewController viewController)
        {
            Info = new Header();
            Files = new List<NeoData>();
            VC = viewController;


        }

        ViewController VC;

        public Header Info { get; set; }
        public string Path { get; set; }
        public List<NeoData> Files { get; set; }
        uint Length;

        void WriteHeader(BinaryWriter bw)
        {
            bw.Write(Info.header1);
            bw.Write(Info.header2);
            bw.Write(Info.header3);
            bw.Write(Info.PSize);
            bw.Write(Info.SSize);
            bw.Write(Info.MSize);
            bw.Write(Info.V1Size);
            bw.Write(Info.V2Size);
            bw.Write(Info.CSize);
            bw.Write(Info.Year);
            bw.Write((int)Info.Genre);
            bw.Write(Info.Screenshot);
            bw.Write(Info.NGH);

            byte[] _Name = Encoding.ASCII.GetBytes(Info.Name); Array.Resize(ref _Name, 33); bw.Write(_Name, 0, 33);
            byte[] _Manu = Encoding.ASCII.GetBytes(Info.Manu); Array.Resize(ref _Manu, 17); bw.Write(_Manu, 0, 17);

            bw.Write(Info.Filler, 0, 512);
            bw.Write(Info.Filler2, 0, 4096);
        }

        public void Save(string path)
        {
            Path = path;
            VC.LogPrintf("Packaging " + path);
            using (BinaryWriter bw = new BinaryWriter(File.Open(path, FileMode.Create)))
            {
                WriteHeader(bw);
                foreach(var file in Files)
                {
                    VC.ReportProgress((int)file.Offset, (int)Length);
                    file.Write(bw);
                    
                }
                VC.ReportProgress(0 , 1);
            }
            VC.LogPrintf("Done packaging");
        }
        public void AddFile(string path)
        {
            FileInfo fi = new FileInfo(path);
            NeoData f = new NeoData(path,  (uint) fi.Length, 0, Files.Count);
            Files.Add(f);
            var bank = NeoData.IdentifyBank(path);
            if (bank == "P")
                Info.PSize +=  (uint)fi.Length;
            else if (bank == "S")
                Info.SSize += (uint)fi.Length;
            else if (bank == "M")
                Info.MSize += (uint)fi.Length;
            else if (bank == "C")
                Info.CSize += (uint)fi.Length;
            else if (bank == "V1")
                Info.V1Size += (uint)fi.Length;
        }
        public void AddFiles(string[] files)
        {
            foreach(var path in files) {
                AddFile(path);
            }
        }
        public void Load(string value)
        {
            if (!System.IO.File.Exists(value))
            {
                
                return;
            }
            Path = value;
            Info = new Header();
            UInt32 offset = 0;
            byte[] fileBytes = File.ReadAllBytes(Path);

            Info.header1 = fileBytes[0];
            Info.header2 = fileBytes[1];
            Info.header3 = fileBytes[2];
            Info.version = fileBytes[3];

            offset = 4;

            Info.PSize = BitConverter.ToUInt32(fileBytes,  (int)offset); offset += 4;
            Info.SSize = BitConverter.ToUInt32(fileBytes,  (int)offset); offset += 4;
            Info.MSize = BitConverter.ToUInt32(fileBytes,  (int)offset); offset += 4;
            Info.V1Size = BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.V2Size = BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.CSize = BitConverter.ToUInt32(fileBytes,  (int)offset); offset += 4;

            Info.Year = BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.Genre = (Genre) BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.Screenshot = BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.NGH = BitConverter.ToUInt32(fileBytes, (int)offset); offset += 4;
            Info.Name = Encoding.UTF8.GetString(fileBytes, (int)offset, 33); offset += 33; Info.Name.Trim();
            Info.Manu = Encoding.UTF8.GetString(fileBytes, (int)offset, 17); offset += 17; Info.Manu.Trim();

            Files = new List<NeoData>();
            Files.Add(new NeoData(Info.Name + "-p1.bin", Info.PSize,  offset, Files.Count, fileBytes)); offset += Info.PSize;
            Files.Add(new NeoData(Info.Name + "-s1.bin", Info.SSize,  offset, Files.Count, fileBytes)); offset += Info.SSize;
            Files.Add(new NeoData(Info.Name + "-m1.bin", Info.MSize,  offset, Files.Count, fileBytes)); offset += Info.MSize;
            Files.Add(new NeoData(Info.Name + "-v1.bin", Info.V1Size, offset, Files.Count, fileBytes)); offset += Info.V1Size;
            Files.Add(new NeoData(Info.Name + "-v2.bin", Info.V2Size, offset, Files.Count, fileBytes)); offset += Info.V2Size;
            Files.Add(new NeoData(Info.Name + "-c1.bin", Info.CSize,  offset, Files.Count, fileBytes)); offset += Info.CSize;

            Length = offset;


            return;
        }

        internal void Build()
        {
            Save(Path);
        }
    }
}

