using System;

namespace UIControllersAndData.Store.Categories.Unit
{
    [System.Serializable]
    public class UnitInfo
    {
        public int id;
        public int level;
        public int count;
        public bool readyToUse;
        public bool isUpgrade;
        public string startOfUpgrade;
        public int timeToBuild;
    }
}