using System;
using System.IO;

namespace DjigurdaBotWs.Repositories
{
    public class WaterFileRepository : IWaterRepository
    {
        private readonly string _fileName;

        public WaterFileRepository()
        {
            _fileName = "data/water.dat";
        }

        public int GetBottlesCount()
        {
            string fileContent = File.ReadAllText(_fileName);
            if (string.IsNullOrEmpty(fileContent)) return 0;

            return Convert.ToInt32(fileContent);
        }

        public void SetBottlesCount(int count)
        {
            File.WriteAllText(_fileName, count.ToString());
        }
    }
}
