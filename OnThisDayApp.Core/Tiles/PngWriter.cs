using System;
using System.Globalization;
using System.IO;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace System.Windows.Media.Imaging
{

    static class PngChunkTypes
    {
        /// <summary>
        /// The first chunk in a png file. Can only exists once. Contains 
        /// common information like the width and the height of the image or
        /// the used compression method.
        /// </summary>
        public const string Header = "IHDR";
        /// <summary>
        /// The PLTE chunk contains from 1 to 256 palette entries, each a three byte
        /// series in the RGB format.
        /// </summary>
        public const string Palette = "PLTE";
        /// <summary>
        /// The IDAT chunk contains the actual image data. The image can contains more
        /// than one chunk of this type. All chunks together are the whole image.
        /// </summary>
        public const string Data = "IDAT";
        /// <summary>
        /// This chunk must appear last. It marks the end of the PNG datastream. 
        /// The chunk's data field is empty. 
        /// </summary>
        public const string End = "IEND";
        /// <summary>
        /// This chunk specifies that the image uses simple transparency: 
        /// either alpha values associated with palette entries (for indexed-color images) 
        /// or a single transparent color (for grayscale and truecolor images). 
        /// </summary>
        public const string PaletteAlpha = "tRNS";
        /// <summary>
        /// Textual information that the encoder wishes to record with the image can be stored in 
        /// tEXt chunks. Each tEXt chunk contains a keyword and a text string.
        /// </summary>
        public const string Text = "tEXt";
        /// <summary>
        /// This chunk specifies the relationship between the image samples and the desired 
        /// display output intensity.
        /// </summary>
        public const string Gamma = "gAMA";
        /// <summary>
        /// The pHYs chunk specifies the intended pixel size or aspect ratio for display of the image. 
        /// </summary>
        public const string Physical = "pHYs";
    }

    sealed class PngHeader
    {
        /// <summary>
        /// The dimension in x-direction of the image in pixels.
        /// </summary>
        public int Width;
        /// <summary>
        /// The dimension in y-direction of the image in pixels.
        /// </summary>
        public int Height;
        /// <summary>
        /// Bit depth is a single-byte integer giving the number of bits per sample 
        /// or per palette index (not per pixel). Valid values are 1, 2, 4, 8, and 16, 
        /// although not all values are allowed for all color types. 
        /// </summary>
        public byte BitDepth;
        /// <summary>
        /// Color type is a integer that describes the interpretation of the 
        /// image data. Color type codes represent sums of the following values: 
        /// 1 (palette used), 2 (color used), and 4 (alpha channel used).
        /// </summary>
        public byte ColorType;
        /// <summary>
        /// Indicates the method  used to compress the image data. At present, 
        /// only compression method 0 (deflate/inflate compression with a sliding 
        /// window of at most 32768 bytes) is defined.
        /// </summary>
        public byte CompressionMethod;
        /// <summary>
        /// Indicates the preprocessing method applied to the image 
        /// data before compression. At present, only filter method 0 
        /// (adaptive filtering with five basic filter types) is defined.
        /// </summary>
        public byte FilterMethod;
        /// <summary>
        /// Indicates the transmission order of the image data. 
        /// Two values are currently defined: 0 (no interlace) or 1 (Adam7 interlace).
        /// </summary>
        public byte InterlaceMethod;
    }

    public static partial class WriteableBitmapExtensions
    {
        private const int MaxBlockSize = 0xFFFF;


        private static Stream _stream;
        private static WriteableBitmap _image;

        private const double DefaultDensityX = 75;
        private const double DefaultDensityY = 75;



        private static bool IsWritingUncompressed = false;
        private static bool IsWritingGamma = true;
        private static double Gamma = 2.2f;

        public static void WritePNG(this WriteableBitmap bmp, System.IO.Stream destination)
        {
            Encode(bmp, destination);
        }

        public static void Encode(WriteableBitmap image, Stream stream)
        {


            _image = image;

            _stream = stream;

            // Write the png header.
            stream.Write(
                new byte[] 
                { 
                    0x89, 0x50, 0x4E, 0x47, 
                    0x0D, 0x0A, 0x1A, 0x0A 
                }, 0, 8);

            PngHeader header = new PngHeader();
            header.Width = image.PixelWidth;
            header.Height = image.PixelHeight;
            header.ColorType = 6;
            header.BitDepth = 8;
            header.FilterMethod = 0;
            header.CompressionMethod = 0;
            header.InterlaceMethod = 0;

            WriteHeaderChunk(header);

            WritePhysicsChunk();
            WriteGammaChunk();

            if (IsWritingUncompressed)
            {
                WriteDataChunksFast();
            }
            else
            {
                WriteDataChunks();
            }
            WriteEndChunk();

            stream.Flush();
        }


        private static void WritePhysicsChunk()
        {
            int dpmX = (int)Math.Round(DefaultDensityX * 39.3700787d);
            int dpmY = (int)Math.Round(DefaultDensityY * 39.3700787d);

            byte[] chunkData = new byte[9];

            WriteInteger(chunkData, 0, dpmX);
            WriteInteger(chunkData, 4, dpmY);

            chunkData[8] = 1;

            WriteChunk(PngChunkTypes.Physical, chunkData);
        }

        private static void WriteGammaChunk()
        {
            if (IsWritingGamma)
            {
                int gammeValue = (int)(Gamma * 100000f);

                byte[] fourByteData = new byte[4];

                byte[] size = BitConverter.GetBytes(gammeValue);
                fourByteData[0] = size[3]; fourByteData[1] = size[2]; fourByteData[2] = size[1]; fourByteData[3] = size[0];

                WriteChunk(PngChunkTypes.Gamma, fourByteData);
            }
        }

        private static void WriteDataChunksFast()
        {
            byte[] pixels = _image.ToByteArray();

            // Convert the pixel array to a new array for adding
            // the filter byte.
            // --------------------------------------------------
            byte[] data = new byte[_image.PixelWidth * _image.PixelHeight * 4 + _image.PixelHeight];

            int rowLength = _image.PixelWidth * 4 + 1;

            for (int y = 0; y < _image.PixelHeight; y++)
            {
                data[y * rowLength] = 0;

                Array.Copy(pixels, y * _image.PixelWidth * 4, data, y * rowLength + 1, _image.PixelWidth * 4);
            }
            // --------------------------------------------------

            Adler32 adler32 = new Adler32();
            adler32.Update(data);

            using (MemoryStream tempStream = new MemoryStream())
            {
                int remainder = data.Length;

                int blockCount;
                if ((data.Length % MaxBlockSize) == 0)
                {
                    blockCount = data.Length / MaxBlockSize;
                }
                else
                {
                    blockCount = (data.Length / MaxBlockSize) + 1;
                }

                // Write headers
                tempStream.WriteByte(0x78);
                tempStream.WriteByte(0xDA);

                for (int i = 0; i < blockCount; i++)
                {
                    // Write the length
                    ushort length = (ushort)((remainder < MaxBlockSize) ? remainder : MaxBlockSize);

                    if (length == remainder)
                    {
                        tempStream.WriteByte(0x01);
                    }
                    else
                    {
                        tempStream.WriteByte(0x00);
                    }

                    tempStream.Write(BitConverter.GetBytes(length), 0, 2);

                    // Write one's compliment of length
                    tempStream.Write(BitConverter.GetBytes((ushort)~length), 0, 2);

                    // Write blocks
                    tempStream.Write(data, (int)(i * MaxBlockSize), length);

                    // Next block
                    remainder -= MaxBlockSize;
                }

                WriteInteger(tempStream, (int)adler32.Value);

                tempStream.Seek(0, SeekOrigin.Begin);

                byte[] zipData = new byte[tempStream.Length];
                tempStream.Read(zipData, 0, (int)tempStream.Length);

                WriteChunk(PngChunkTypes.Data, zipData);
            }
        }



        private static void WriteDataChunks()
        {
            byte[] pixels = _image.ToByteArray();

            byte[] data = new byte[_image.PixelWidth * _image.PixelHeight * 4 + _image.PixelHeight];

            int rowLength = _image.PixelWidth * 4 + 1;

            for (int y = 0; y < _image.PixelHeight; y++)
            {
                byte compression = 0;
                if (y > 0)
                {
                    compression = 2;
                }
                data[y * rowLength] = compression;

                for (int x = 0; x < _image.PixelWidth; x++)
                {
                    // Calculate the offset for the new array.
                    int dataOffset = y * rowLength + x * 4 + 1;

                    // Calculate the offset for the original pixel array.
                    int pixelOffset = (y * _image.PixelWidth + x) * 4;

                    data[dataOffset + 0] = pixels[pixelOffset + 0];
                    data[dataOffset + 1] = pixels[pixelOffset + 1];
                    data[dataOffset + 2] = pixels[pixelOffset + 2];
                    data[dataOffset + 3] = pixels[pixelOffset + 3];

                    if (y > 0)
                    {
                        int lastOffset = ((y - 1) * _image.PixelWidth + x) * 4;

                        data[dataOffset + 0] -= pixels[lastOffset + 0];
                        data[dataOffset + 1] -= pixels[lastOffset + 1];
                        data[dataOffset + 2] -= pixels[lastOffset + 2];
                        data[dataOffset + 3] -= pixels[lastOffset + 3];
                    }
                }
            }

            byte[] buffer = null;
            int bufferLength = 0;

            MemoryStream memoryStream = null;
            try
            {
                memoryStream = new MemoryStream();

                using (DeflaterOutputStream zStream = new DeflaterOutputStream(memoryStream))
                {
                    zStream.Write(data, 0, data.Length);
                    zStream.Flush();
                    zStream.Finish();

                    bufferLength = (int)memoryStream.Length;
                    buffer = memoryStream.GetBuffer();
                }
            }
            finally
            {
                if (memoryStream != null)
                {
                    memoryStream.Dispose();
                }
            }

            int numChunks = bufferLength / MaxBlockSize;

            if (bufferLength % MaxBlockSize != 0)
            {
                numChunks++;
            }

            for (int i = 0; i < numChunks; i++)
            {
                int length = bufferLength - i * MaxBlockSize;

                if (length > MaxBlockSize)
                {
                    length = MaxBlockSize;
                }

                WriteChunk(PngChunkTypes.Data, buffer, i * MaxBlockSize, length);
            }
        }

        private static void WriteEndChunk()
        {
            WriteChunk(PngChunkTypes.End, null);
        }

        private static void WriteHeaderChunk(PngHeader header)
        {
            byte[] chunkData = new byte[13];

            WriteInteger(chunkData, 0, header.Width);
            WriteInteger(chunkData, 4, header.Height);

            chunkData[8] = header.BitDepth;
            chunkData[9] = header.ColorType;
            chunkData[10] = header.CompressionMethod;
            chunkData[11] = header.FilterMethod;
            chunkData[12] = header.InterlaceMethod;

            WriteChunk(PngChunkTypes.Header, chunkData);
        }


        private static void WriteChunk(string type, byte[] data)
        {
            WriteChunk(type, data, 0, data != null ? data.Length : 0);
        }

        private static void WriteChunk(string type, byte[] data, int offset, int length)
        {
            WriteInteger(_stream, length);

            byte[] typeArray = new byte[4];
            typeArray[0] = (byte)type[0];
            typeArray[1] = (byte)type[1];
            typeArray[2] = (byte)type[2];
            typeArray[3] = (byte)type[3];

            _stream.Write(typeArray, 0, 4);

            if (data != null)
            {
                _stream.Write(data, offset, length);
            }

            Crc32 crc32 = new Crc32();
            crc32.Update(typeArray);

            if (data != null)
            {
                crc32.Update(data, offset, length);
            }

            WriteInteger(_stream, (uint)crc32.Value);
        }



        private static void WriteInteger(byte[] data, int offset, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);
            Array.Copy(buffer, 0, data, offset, 4);
        }

        private static void WriteInteger(Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);

            stream.Write(buffer, 0, 4);
        }

        private static void WriteInteger(Stream stream, uint value)
        {
            byte[] buffer = BitConverter.GetBytes(value);

            Array.Reverse(buffer);

            stream.Write(buffer, 0, 4);
        }



    }

}