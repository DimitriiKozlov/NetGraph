using System.Collections.Generic;

namespace CourseProject
{
    class Packages
    {
        public bool IsDatagtam;
        public int Size;
        public List<int> Way;
        public int IdNext;
        public int Id;
        public bool WaitForAnswer;

        public Packages(int size, bool isDatagram)
        {
            IsDatagtam = isDatagram;
            Size = size;
            WaitForAnswer = false;
            IdNext = -1;
            Id = -1;
        }

        public bool Next(List<int> way = null)
        {
            if (way == null)
            {
                Way.RemoveAt(0);
                Id = Way[0];
                if (Way.Count < 2)
                    return true;
                IdNext = Way[1];
                return false;
            }
            Way = way;
            Way.RemoveAt(0);
            Id = Way[0];
            if (Way.Count < 2)
                return true;
            IdNext = Way[1];
            return false;
        }

        public void SetWay(List<int> way)
        {
            Way = way;
            Id = Way[0];
            IdNext = Way[1];
        }
    }
}
