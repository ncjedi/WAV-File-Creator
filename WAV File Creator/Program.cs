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

//tempo for notes. 1 means 1 second per stanza
int tempo = 2;

//actual data
StandardWaveformNote("C4", 4);
AddBlank(10);
StandardWaveformNote("C4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(20);

StandardWaveformNote("A4", 4);
AddBlank(10);
StandardWaveformNote("A4", 4);
AddBlank(10);
StandardWaveformNote("G4", 2);
AddBlank(20);

StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(20);

StandardWaveformNote("D4", 4);
AddBlank(10);
StandardWaveformNote("D4", 4);
AddBlank(10);
StandardWaveformNote("C4", 2);
AddBlank(20);

StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(20);

StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("D4", 2);
AddBlank(20);

StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(20);

StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("D4", 2);
AddBlank(20);

StandardWaveformNote("C4", 4);
AddBlank(10);
StandardWaveformNote("C4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(10);
StandardWaveformNote("G4", 4);
AddBlank(20);

StandardWaveformNote("A4", 4);
AddBlank(10);
StandardWaveformNote("A4", 4);
AddBlank(10);
StandardWaveformNote("G4", 2);
AddBlank(20);

StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("F4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(10);
StandardWaveformNote("E4", 4);
AddBlank(20);

StandardWaveformNote("D4", 4);
AddBlank(10);
StandardWaveformNote("D4", 4);
AddBlank(10);
StandardWaveformNote("C4", 2);
AddBlank(20);

//square A note
/*bool up = true;
for(int i = 0; i < 44100; i++)
{
    byte[] tempdata = new byte[2];

    if(i % 50 == 0)
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
}*/

//Calculate sizes based on data
BinaryPrimitives.WriteUInt32LittleEndian(SIZE, (uint)((DATA.Count * 2) + 36));
BinaryPrimitives.WriteUInt32LittleEndian(CHUNK2SIZE, (uint)(DATA.Count * 2));

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

Console.ReadKey();

//Triangle wave
void StandardWaveform(int target)
{
    int pos = 0;
    int tempSample;
    bool up = true;
    for (int i = 0; i < 44100; i++)
    {
        byte[] tempdata = new byte[2];
        tempSample = (int)(MathF.Round((pos / (float)target) * 16383));
        //Console.WriteLine(tempSample);

        if (up)
        {
            pos++;
        }
        else
        {
            pos--;
        }

        if (pos >= target)
        {
            up = false;
        }
        else if (pos <= -target)
        {
            up = true;
        }

        BinaryPrimitives.WriteInt16LittleEndian(tempdata, (short)tempSample);

        DATA.Add(tempdata);
    }
}

//Triangle wave with note list. 1 length is a whole note, 2 is a half note, 4 is quarter note, 8 is eighth, 16 is sixteenth.
void StandardWaveformNote(string note, int length)
{
    Dictionary<string, int> notes = new Dictionary<string, int>()
    {
        {"C1",689 },
        {"C#1",649 },
        {"D1",613 },
        {"D#1",580 },
        {"E1",538 },
        {"F1",513 },
        {"F#1",479 },
        {"G1",450 },
        {"G#1",424 },
        {"A1",401 },
        {"A#1",380 },
        {"B1",362 },

        {"C2",339 },
        {"C#2",320 },
        {"D2",302 },
        {"D#2",286 },
        {"E2",269 },
        {"F2",254 },
        {"F#2",240 },
        {"G2",225 },
        {"G#2",212 },
        {"A2",201 },
        {"A#2",190 },
        {"B2",179 },

        {"C3",170 },
        {"C#3",160 },
        {"D3",151 },
        {"D#3",142 },
        {"E3",134 },
        {"F3",127 },
        {"F#3",119 },
        {"G3",113 },
        {"G#3",106 },
        {"A3",100 },
        {"A#3",95 },
        {"B3",90 },

        {"C4",84 },
        {"C#4",80 },
        {"D4",75 },
        {"D#4",71 },
        {"E4",67 },
        {"F4",63 },
        {"F#4",60 },
        {"G4",56 },
        {"G#4",53 },
        {"A4",50 },
        {"A#4",47 },
        {"B4",45 },

        {"C5",42 },
        {"C#5",40 },
        {"D5",38 },
        {"D#5",35 },
        {"E5",34 },
        {"F5",32 },
        {"F#5",30 },
        {"G5",28 },
        {"G#5",27 },
        {"A5",25 },
        {"A#5",24 },
        {"B5",22 },
    };

    int target = notes[note];

    int pos = 0;
    int tempSample;
    bool up = true;
    for (int i = 0; i < (44100/length) * tempo; i++)
    {
        byte[] tempdata = new byte[2];
        tempSample = (int)(MathF.Round((pos / (float)target) * 16383));
        //Console.WriteLine(tempSample);

        if (up)
        {
            pos++;
        }
        else
        {
            pos--;
        }

        if (pos >= target)
        {
            up = false;
        }
        else if (pos <= -target)
        {
            up = true;
        }

        BinaryPrimitives.WriteInt16LittleEndian(tempdata, (short)tempSample);

        DATA.Add(tempdata);
    }
}

void AddBlank(int miliseconds)
{
    for (int i = 0;i < miliseconds * 44.1;i++)
    {
        byte[] tempdata = new byte[2];
        BinaryPrimitives.WriteInt16LittleEndian(tempdata, 0);
        DATA.Add(tempdata);
    }
}