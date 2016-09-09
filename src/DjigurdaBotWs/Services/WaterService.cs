using DjigurdaBotWs.Repositories;

namespace DjigurdaBotWs.Services
{
    public interface IWaterService
    {
        int GetBottlesCount();
        int UseBottle();
        void SetBottlesCount(int count);
    }

    public class WaterService : IWaterService
    {
        private readonly IWaterRepository _waterRepository;

        public WaterService(IWaterRepository waterRepository)
        {
            _waterRepository = waterRepository;
        }

        public int GetBottlesCount()
        {
            return _waterRepository.GetBottlesCount();
        }

        public int UseBottle()
        {
            int count =  _waterRepository.GetBottlesCount();
            count--;

            _waterRepository.SetBottlesCount(count);
            return count;
        }

        public void SetBottlesCount(int count)
        {
            _waterRepository.SetBottlesCount(count);
        }
    }
}
