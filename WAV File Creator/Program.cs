using System.Buffers.Binary;
using System.Text;

var docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

ASCIIEncoding aSCIIEncoding = new ASCIIEncoding();

//WAVE file formatting
Span<Byte> MAGIC = new byte[4];             //ID always "RIFF"
Span<Byte> SIZE = new byte[4];              //size of the whole boy(this chunk and all) in little endian encoding
Span<Byte> FORMAT = new byte[4];            //always "WAVE"

Span<Byte> CHUNK1ID = new byte[4];          //always "fmt " (with a space at the end)
Span<Byte> CHUNK1SIZE = new byte[4];        //size of the first chunk(almost always 16). little endian encoding
Span<Byte> AUDIOFORMAT = new byte[2];       //almost always 1. if not then compressed. little endian encoding
Span<Byte> NUMCHANNELS = new byte[2];       //number of channels. for this program it is 1 but can be any number. 1 is mono, 2 is sterio, etc. little endian encoding
Span<Byte> SMAPLERATE = new byte[4];        //almost always 44100. little endian encoding
Span<Byte> BYTERATE = new byte[4];          //SampleRate * NumChannels * (BitsPerSample/8). little endian encoding
Span<Byte> BLOCKALIGN = new byte[2];        //NumChannels * (BitsPerSample/8). little endian encoding
Span<Byte> BITSPERSAMPLE = new byte[2];     //8, 16, 32, etc. little endian encoding.

Span<Byte> CHUNK2ID = new byte[4];          //always "data"
Span<Byte> CHUNK2SIZE = new byte[4];        //size of the second chunk(mostly the data). little endian encoding
List<byte[]> DATA = new List<byte[]>();     //actual file data

aSCIIEncoding.GetBytes("RIFF", MAGIC);
BinaryPrimitives.WriteUInt32LittleEndian(SIZE, 0);
aSCIIEncoding.GetBytes("WAVE", FORMAT);

aSCIIEncoding.GetBytes("fmt ", CHUNK1ID);
BinaryPrimitives.WriteUInt32LittleEndian(CHUNK1SIZE, 16);
BinaryPrimitives.WriteUInt16LittleEndian(AUDIOFORMAT, 1);
BinaryPrimitives.WriteUInt16LittleEndian(NUMCHANNELS, 1);
BinaryPrimitives.WriteUInt32LittleEndian(SMAPLERATE, 44100);
BinaryPrimitives.WriteUInt32LittleEndian(BYTERATE, 44100 * 2);
BinaryPrimitives.WriteUInt16LittleEndian(BLOCKALIGN, 2);
BinaryPrimitives.WriteUInt16LittleEndian(BITSPERSAMPLE, 16);

aSCIIEncoding.GetBytes("data", CHUNK2ID);
BinaryPrimitives.WriteUInt32LittleEndian(CHUNK2SIZE, 0);

//actual data
bool up = true;
for(int i = 0; i < 44100; i++)
{
    byte[] tempdata = new byte[2];

    if(i % 100 == 0)
    {
        up = !up;
    }

    if(up)
    {
        BinaryPrimitives.WriteInt16LittleEndian(tempdata, 16383);
    }
    else
    {
        BinaryPrimitives.WriteInt16LittleEndian(tempdata, -16383);
    }

    DATA.Add(tempdata);
}

//Calculate sizes based on data
BinaryPrimitives.WriteUInt32LittleEndian(SIZE, (uint)((DATA.Count * 2) + 36));
BinaryPrimitives.WriteUInt32LittleEndian(CHUNK2SIZE, (uint)(DATA.Count * 2));

//Reverse data after creation so that it is in little endian format.
DATA.Reverse();

//Write the file (and delete old one if it exists)
if (File.Exists("test.wav"))
{
    File.Delete("test.wav");
}

using (FileStream streamWriter = new FileStream("test.wav", FileMode.Append))
{
    streamWriter.Write(MAGIC);
    streamWriter.Write(SIZE);
    streamWriter.Write(FORMAT);

    streamWriter.Write(CHUNK1ID);
    streamWriter.Write(CHUNK1SIZE);
    streamWriter.Write(AUDIOFORMAT);
    streamWriter.Write(NUMCHANNELS);
    streamWriter.Write(SMAPLERATE);
    streamWriter.Write(BYTERATE);
    streamWriter.Write(BLOCKALIGN);
    streamWriter.Write(BITSPERSAMPLE);

    streamWriter.Write(CHUNK2ID);
    streamWriter.Write(CHUNK2SIZE);
    foreach (byte[] bytes in DATA)
    {
        streamWriter.Write(bytes);
    }
}