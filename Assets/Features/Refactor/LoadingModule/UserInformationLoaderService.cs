using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Features.Refactor
{
    public class UserInformationLoaderService : ILoadService
    {
        private static readonly int MAX_LOAD_RETRIES = 6;
        private static readonly int[] RETRY_DELAY_INTERVAL = {3,10};
        private static readonly int BATCH_MAX_SIZE = 50;
        
        private Dictionary<string,int> loadingPathsRetries = new Dictionary<string, int>();
        
        public bool IsLoaded {get; private set; }

        public event Action OnLoadFailed;
        public async Task<T> LoadFromAPI<T>(string path, string token) where T : class
        {
            loadingPathsRetries[path] = loadingPathsRetries.ContainsKey(path) ? loadingPathsRetries[path]++ : 1;
            try
            {
                //here I would do something more generic on API Service, instead of this concrete method
                var data = APIServices.DatabaseServer.GetUserInfo(token) as T;
                IsLoaded = true;
                loadingPathsRetries.Remove(path);
                return data;
            }
            catch (Exception e)
            {
                if (loadingPathsRetries[path] < MAX_LOAD_RETRIES)
                {
                    await new WaitForSeconds(GetDelayToRetry());
                    await LoadFromAPI<T>(path,token);
                }
                else
                {
                    OnLoadFailed?.Invoke();
                }
            }
            return null;
        }
        
        public bool IsLoadingFromAPI(string path)
        {
            return loadingPathsRetries.ContainsKey(path);
        }

        public async Task LoadNFTImagePerBatch(Dictionary<UnitType, List<FeudalzUnit>> units)
        {
            const float delayUntilNextBatch = 10f;
            foreach (var unitType in units)
            {
                var batchCounter = 0;
                for(var i = 0; i < unitType.Value.Count; i++)
                {
                    if (batchCounter >= BATCH_MAX_SIZE)
                    {
                        unitType.Value[i].LoadNFTImage();
                        await new WaitForSeconds(delayUntilNextBatch);
                        batchCounter = 0;
                        continue;
                    }
                    unitType.Value[i].LoadNFTImage();
                    batchCounter++;
                }
            }
        }

        private static int GetDelayToRetry()
        {
            return UnityEngine.Random.Range(RETRY_DELAY_INTERVAL[0],RETRY_DELAY_INTERVAL[1]);
        }
    }
}