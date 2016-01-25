using System;
using System.Collections.Generic;

namespace CourseProject
{
    class Message
    {
        public const int MessageMaxSize = 9999;
        public const int MessageBlockSize = 128;

        public int Size;
        public List<Packages> PackagesList;

        public Message(int size, bool isDatagram)
        {
            PackagesList = new List<Packages>();
            Size = size;
            while (size > MessageBlockSize)
            {
                PackagesList.Add(new Packages(MessageBlockSize, isDatagram));
                size -= MessageBlockSize;
            }
            if (size > 0)
                PackagesList.Add(new Packages(size, isDatagram));
        }


    }
}
