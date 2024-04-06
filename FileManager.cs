using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Virtual_array
{
    public class FileManager
    {
        public static void ReadFileToPage(string signature, string fileName, int bitmapSize, int elementsCountOnPage, int pageCountOnFile, Page page)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                int signatureSize = signature.Length * sizeof(Byte);

                Byte[] bufferElements = new Byte[elementsCountOnPage * sizeof(int)];
                Byte[] outputBitMap = new byte[bitmapSize];
                int[] outputElements = new int[elementsCountOnPage];
                Byte[] tmp = new byte[sizeof(int)];

                int readingBitmapFormula = signatureSize + bitmapSize * page.PageNumber;
                int readingElementsFormula = signatureSize + bitmapSize * pageCountOnFile + elementsCountOnPage * sizeof(int) * page.PageNumber;

                fileStream.Seek(readingBitmapFormula, SeekOrigin.Begin);
                fileStream.Read(outputBitMap, 0, outputBitMap.Length);

                fileStream.Seek(readingElementsFormula, SeekOrigin.Begin);
                fileStream.Read(bufferElements, 0, bufferElements.Length);



                for (int i = 0; i < outputElements.Length; i++)
                {
                    Array.Copy(bufferElements, i * sizeof(int), tmp, 0, tmp.Length);
                    outputElements[i] = BitConverter.ToInt32(tmp, 0);
                }



                page.SetBitMapData(outputBitMap);
                page.SetElementsData(outputElements);
            }
        }
        public static void WriteFile(string signature, string fileName, int bitmapSize, int elementsCountOnPage, int pageCountOnFile)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
            {
                int[] elements = new int[elementsCountOnPage * pageCountOnFile];
                Byte[] signatureBuffer = Encoding.Default.GetBytes(signature);
                Byte[] bitMapBuffer = new Byte[bitmapSize * pageCountOnFile];
                Byte[] elementsBuffer = new Byte[elements.Length * sizeof(int)];
                Byte[] buffer = new byte[1];//array for convert int to byte
                Byte[] tmp;
                int byteIndex = 0;

                for (int i = 0; i < bitMapBuffer.Length; i++)
                    bitMapBuffer[i] = 0b0_0000_0000;

                for (int i = 0; i < elements.Length; i++)
                    elements[i] = 0;

                for (int i = 0; i < elements.Length; i++)
                {
                    tmp = BitConverter.GetBytes(elements[i]);
                    Array.Copy(tmp, 0, elementsBuffer, byteIndex, tmp.Length);
                    byteIndex += tmp.Length;
                }


                fileStream.Write(signatureBuffer, 0, signatureBuffer.Length);
                fileStream.Write(bitMapBuffer, 0, bitMapBuffer.Length);
                fileStream.Write(elementsBuffer, 0, elementsBuffer.Length);
            }
        }

        public void UpdatePageDataInFile(string signature, string fileName, int bitmapSize, int elementsCountOnPage, int pageCountOnFile, Page page)
        {
            int signatureSize = signature.Length * sizeof(Byte);

            int wrtingBitmapFormula = signatureSize + bitmapSize * page.PageNumber;
            int wrtingElementsFormula = signatureSize + bitmapSize * pageCountOnFile + elementsCountOnPage * sizeof(int) * page.PageNumber;
            Byte[] elementsBuffer = new Byte[elementsCountOnPage * sizeof(int)];
            Byte[] tmp;
            int byteIndex = 0;


            for (int i = 0; i < page.Elements.Length; i++)
            {
                tmp = BitConverter.GetBytes(page.Elements[i]);
                Array.Copy(tmp, 0, elementsBuffer, byteIndex, tmp.Length);
                byteIndex += tmp.Length;
            }

            using (FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Write))
            {
                fileStream.Seek(wrtingBitmapFormula, SeekOrigin.Begin);
                fileStream.Write(page.BitMap, 0, page.BitMap.Length);


                fileStream.Seek(wrtingElementsFormula, SeekOrigin.Begin);
                fileStream.Write(elementsBuffer, 0, elementsBuffer.Length);
            }
        }

    }
}
