using System;

namespace Virtual_array
{
    public struct Page
    {
        public int PageNumber { get; }
        public bool IsModifited { get; set; }
        public DateTime RecordingDate { get; private set;  }
        public Byte [] BitMap { get; }
        public  int [] Elements { get; }

        

        public Page(int pageNumber, int pageSizeInByte, int bitMapSize, int elementsCountOnPage)
        {
            if (pageNumber < 0)
                throw new ArgumentException("Wrong page number");
            else if (pageSizeInByte < 0)
                throw new ArgumentException("Wrong page size");
            else
            {
               
                PageNumber = pageNumber;
                IsModifited = false;
                RecordingDate = DateTime.Now;


                BitMap = new Byte[bitMapSize];
                Elements = new int[elementsCountOnPage];
            }
        }

        public void UpdateRecordingDate()
        {
            RecordingDate = DateTime.Now;
        }
        public void SetBitMapData(Byte[] outputBitMap)
        {
            outputBitMap.CopyTo(BitMap, 0);
        }

        public void SetElementsData(int[] outputElements)
        {
            outputElements.CopyTo(Elements, 0);
        }

        public bool CheckBit(int bitMapByteIndex, int discharge)
        {
            int checker = (int)Math.Pow(2, discharge);
            if ((BitMap[bitMapByteIndex] & checker) == checker)
                return true;
            else return false;
        }

        public void SetBit(int bitMapByteIndex, int discharge)
        {
            int setter = (int)Math.Pow(2, discharge);

            BitMap[bitMapByteIndex] = (Byte)(BitMap[bitMapByteIndex] | setter);
        }
    }
}
