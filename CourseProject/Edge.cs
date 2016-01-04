namespace CourseProject
{
    class Edge
    {
        public readonly int IdVertex1;
        public readonly int IdVertex2;
        public string Content;
        public int ChildId;
        public bool Enable;

        public int ChannelWeight;
        public int ChannelCapacity;

        public Edge(int idV1, int idV2, int chId, int weight, int capacity, string text = "")
        {
            IdVertex1 = idV1;
            IdVertex2 = idV2;
            Content = text;
            ChildId = chId;
            Enable = true;

            ChannelWeight = weight;
            ChannelCapacity = capacity;
        }
    }
}
