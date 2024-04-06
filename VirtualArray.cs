using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Virtual_array
{
    public class VirtualArray
    {
        public long ArraySize { get; }
        public Page[] PageBuffer { get; }
        public int PageCountOnBuffer { get; }
        public string fileName { get;}
        public FileManager FileManager { get; }

        private int elementsCountOnPage;
        private int bitMapSize;
        private int pageCountOnFile;
        private int pageSizeInByte;
        private string signature;
        public VirtualArray(long arraySize, int pageSizeInByte, int pageCountOnBuffer, FileManager fileManager, string fileName = "file", string signature = "VM")
        {
            if (arraySize <= 0)
                throw new ArgumentException("wrong array size");
            else if (pageSizeInByte > arraySize)
                throw new ArgumentException("");
            else if (pageCountOnBuffer <= 0 || pageCountOnBuffer > 10)
                throw new ArgumentException("The number of pages in the buffer should be from 1 to 10");
            else if (fileManager == null)
                throw new ArgumentNullException("Null file manager object");
            else
            {
                ArraySize = arraySize;
                PageCountOnBuffer = pageCountOnBuffer;
                PageBuffer = new Page[PageCountOnBuffer];
                FileManager = fileManager;
                this.fileName = fileName;

                pageCountOnFile = (int)arraySize / pageSizeInByte;
                this.pageSizeInByte = pageSizeInByte;
                elementsCountOnPage = pageSizeInByte / sizeof(int);
                this.signature = signature;

                bitMapSize = elementsCountOnPage / (sizeof(Byte) * 8); //x bytes of 8 bits = size in bits
                if (elementsCountOnPage % 8 != 0)
                    bitMapSize++;

                ReadPageData();
            }
        }

        public VirtualArray(Page[] PageBuffer)//for test
        {
            this.PageBuffer = PageBuffer;
        }

        private void ReadPageData()
        {
          for(int i = 0; i<PageBuffer.Length; i++)
                PageBuffer[i] = new Page(i, pageSizeInByte, bitMapSize, elementsCountOnPage);

            if (File.Exists(fileName) != true)
                FileManager.WriteFile(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile);
            
            for (int i = 0; i < PageBuffer.Length; i++)
                FileManager.ReadFileToPage(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile, PageBuffer[i]);

        }
        private int GetPageNumberByElementIndex(int elementIndex)
        {
            return elementIndex / elementsCountOnPage;
        }

        private int GetElementNumInPage(int elementIndex)
        {
            return elementIndex % elementsCountOnPage;
        }

        private int GetBitMapByteIndex(int elementIndex)
        {
            return elementIndex / 8;
        }

        private int GetDischarge(int elementIndex)
        {
            return Math.Abs(sizeof(Byte)*8 - elementIndex % 8 - 1);
            //return elementIndex % elementsCountOnPage;
        }

        private int GetOldestPageNumber(Page[] buffer)
        {
            int num;
            Page[] arr = buffer.OrderBy(x => x.RecordingDate).ToArray();

            num = arr[0].PageNumber;

            return num;
        }

        public int GetPageNumberInBufferByElementIndex(int elementIndex)
        {
            int absolutePageNum = GetPageNumberByElementIndex(elementIndex);
            bool isInBuffer = false;
            int OldestPageNum;
            int pageNumInBuffer = 0;

            for (int i = 0; i < PageBuffer.Length; i++)
                if (PageBuffer[i].PageNumber == absolutePageNum)
                { 
                    isInBuffer = true;
                    pageNumInBuffer = i;
                    break;
                }

            if (isInBuffer == false)
            {
                OldestPageNum = GetOldestPageNumber(PageBuffer);

                if (PageBuffer[OldestPageNum].IsModifited == true)
                    FileManager.UpdatePageDataInFile(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile, PageBuffer[OldestPageNum]);

                Page requiredPage = new Page(absolutePageNum, pageSizeInByte, bitMapSize, elementsCountOnPage);
                FileManager.ReadFileToPage(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile, requiredPage);

                for (int i = 0; i < PageBuffer.Length; i++)
                    if (PageBuffer[i].PageNumber == OldestPageNum)
                        pageNumInBuffer = i;

                PageBuffer[pageNumInBuffer] = requiredPage;

            }

            return pageNumInBuffer;
        }

        public void ReadElementByIndex(int elementIndex)
        {
            int pageNumInBuffer = GetPageNumberInBufferByElementIndex(elementIndex);
            int elementNumInPage = GetElementNumInPage(elementIndex);
            int bitMapByteIndex = GetBitMapByteIndex(elementIndex);
            int discharge = GetDischarge(elementIndex);
            int result;
            bool flag;

            result = PageBuffer[pageNumInBuffer].Elements[elementNumInPage];

            if (PageBuffer[pageNumInBuffer].CheckBit(bitMapByteIndex, discharge) == true)
                flag = true;
            else flag = false;

            PageBuffer[pageNumInBuffer].UpdateRecordingDate();

            Console.WriteLine($"Value: {result}, flag: {flag}");
        }

        public void WriteElementByIndex(int elementIndex, int inputValue)
        {
            int pageNumInBuffer = GetPageNumberInBufferByElementIndex(elementIndex);
            int elemetNumInPage = GetElementNumInPage(elementIndex);
            int bitMapByteIndex = GetBitMapByteIndex(elementIndex);
            int discharge = GetDischarge(elementIndex);

            PageBuffer[pageNumInBuffer].Elements[elemetNumInPage] = inputValue;

            PageBuffer[pageNumInBuffer].SetBit(bitMapByteIndex, discharge);
            PageBuffer[pageNumInBuffer].UpdateRecordingDate();
            PageBuffer[pageNumInBuffer].IsModifited = true;

            FileManager.UpdatePageDataInFile(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile,PageBuffer[pageNumInBuffer]);
            
            Console.WriteLine("Completed successfully");
        }
        public void SaveAllPages()
        {
            foreach (var p in PageBuffer)
                FileManager.UpdatePageDataInFile(signature, fileName, bitMapSize, elementsCountOnPage, pageCountOnFile, p);
        }
    }
}
