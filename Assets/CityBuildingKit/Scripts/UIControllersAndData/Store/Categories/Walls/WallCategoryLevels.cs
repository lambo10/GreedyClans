using System.Collections.Generic;
using UIControllersAndData.Models;

namespace UIControllersAndData.Store.Categories.Walls
{
    [System.Serializable]
    public class WallCategoryLevels: INamed, IId
    {
        public string name;
        public int id;
        public List<WallsCategory> levels;
        public string GetName()
        {
            return name;
        }

        public int GetId()
        {
            return id;
        }
    }
}