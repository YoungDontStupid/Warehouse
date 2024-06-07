using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_management_core.DTO_s;

namespace warehouse_management_application.Storages
{
    public class StorageService(IRepository<Storage> repository) : IServise
    {
        private IRepository<Storage> Repository { get; init; } = repository;

        public async Task<IEnumerable<StorageDTO>> GetStoragesAsync(CancellationToken cancellationToken = default) =>
            (await Repository.Get(cancellationToken)).Select(x => (StorageDTO)x);

        public async Task<IEnumerable<StorageDTO>> GetStoragesItemsAsync(Guid storageId, CancellationToken cancellationToken = default)
        {
            var potentialStorage = (await Repository.GetWithoutTracking(x => x.Id.Value == storageId, cancellationToken)).FirstOrDefault() ??
                throw new StorageNotFoundException(storageId);
            return (IEnumerable<StorageDTO>)potentialStorage.Items.Cast<ItemDTO>();
        }

        public async Task<IEnumerable<EmployeeDTO>> GetStorageEmployeesAsync(Guid storageId, CancellationToken cancellationToken = default)
        {
            var potentialStorage = (await Repository.GetWithoutTracking(x => x.Id.Value == storageId, cancellationToken)).FirstOrDefault() ??
                throw new StorageNotFoundException(storageId);
            return potentialStorage.Items.Cast<EmployeeDTO>();
        }

        public async Task CreateOrUpdateStorageAsync(StorageDTO storage, CancellationToken cancellationToken = default)
        {
            Storage localStorage;
            if (storage.Id is not null)
            {
                localStorage = (await Repository.Get(x => x.Id.Value == storage.Id.Value, cancellationToken)).FirstOrDefault() ??
                    throw new StorageNotFoundException(storage.Id.Value);
                localStorage.Name = storage.Name;
                localStorage.Description = storage.Description;
                localStorage.Capacity = storage.Capacity;
                localStorage.Temperature = storage.Temperature;
                
            }
            else
                localStorage = new()
                {
                    Name = storage.Name,
                    Description = storage.Description,
                    Capacity = storage.Capacity,
                    Temperature = storage.Temperature
                };

            if (localStorage.Id is null)
                await Repository.Add(localStorage, cancellationToken);
            else
                await Repository.Update(localStorage, cancellationToken);
        }
            public async Task<Solution> SolveTransportTaskAsync(IEnumerable<StorageDTO> storages, IEnumerable<ItemDTO> itemsToMove, CancellationToken cancellationToken = default)
        {
            var storagesList = await GetStoragesAsync(cancellationToken);
            var itemsList = itemsToMove.Select(item => item.Id).ToList();

            var storageIds = storagesList.Select(s => s.Id).Distinct().ToList();
            var distances = new Dictionary<string, List<double>>();

            foreach (var storage in storagesList)
            {
                distances[storage.Id] = new List<double>();
                foreach (var otherStorage in storagesList.Where(s => s.Id!= storage.Id))
                    double distance = CalculateDistanceBetweenStorages(storage, otherStorage); 
                    distances[storage.Id].Add(distance);
                }
            }

            var route = TSPAlgorithm(itemsList, storageIds, distances);

            return new Solution(route);
        }

        private double CalculateDistanceBetweenStorages(StorageDTO start, StorageDTO end)
        {
            return Math.Abs(start.Id - end.Id) * 100; 
        }

    private List<Guid> TSPAlgorithm(List<Guid> items, List<string> nodes, Dictionary<string, List<double>> distances)
    {
        var currentLocation = nodes.First(); 
        var path = new List<Guid> { Guid.Parse(currentLocation) }; 

        while (path.Count < items.Count + 1)
        {
            var nextNode = nodes.Except(path).OrderBy(n => distances[currentLocation][n]).First();
            path.Add(Guid.Parse(nextNode)); 
            currentLocation = nextNode; 
        }
        path.Add(Guid.Parse(nodes.First()));

        return path;
}

        public class Solution
        {
            public List<Guid> Route { get; set; }

            public Solution(List<Guid> route)
            {
                Route = route;
            }
        }
    }
}
