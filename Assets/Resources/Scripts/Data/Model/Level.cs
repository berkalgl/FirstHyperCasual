using System;
using System.Collections.Generic;

public static class Level
{
    [Serializable]
    public class LevelData
    {
        public List<Item> items;
        public List<Level> levels;

        [Serializable]
        public class Item : BaseItem
        {
            public List<int> ids;
        }

        [Serializable]
        public class BaseItem
        {
            public string name;
            public string type;
            public bool isPartial;
            public int width = 1;
            public int height = 1;
        }

        [Serializable]
        public class Level
        {
            public int levelNo;
            public int width;
            public int height;
            public Game game;
            public List<int> data;

            [Serializable]
            public class Game
            {
                public int lengthThreshold;
            }


            [Serializable]
            public class Size
            {
                public float width;
                public float height;
            }
        }
    }
}