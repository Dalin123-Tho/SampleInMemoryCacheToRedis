using MemoryCacheToRedis.Models;
using MemoryCacheToRedis.Service;
using MemoryCacheToRedis.Service.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MemoryCacheToRedis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext _dbContext;
        private readonly ICacheService _cacheService;
        public TestController(MyDbContext dbContext, ICacheService cacheService)
        {
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        private static object _lock = new object();

        #region RedisCache
        [HttpGet]
        [Route("rediscaches")]
        public List<Employee> GetAll()
        {
            //Get data from cache by key value
            var cacheData = _cacheService.GetData<List<Employee>>("rediscache");
            if (cacheData != null)
            {
                return cacheData;
            }
            // avoid multi-threading
            lock(_lock)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(5);
                //get from database and assign it into cache
                cacheData = _dbContext.Employees.ToList();
                _cacheService.SetData<List<Employee>>("rediscache", cacheData, expirationTime);
            }
            return cacheData;
        }

        [HttpPost("redispost")]
        public async Task<Employee> Post(Employee value)
        {
            var obj = await _dbContext.Employees.AddAsync(value);
            _cacheService.RemoveData("rediscache");
            _dbContext.SaveChanges();
            return obj.Entity;
        }
        #endregion


        #region MemoryCache
        [HttpGet]
        [Route("memorycaches")]
        public List<Employee> GetAllFromMemCache()
        {
            var cacheData = MemoryCacheService<List<Employee>>.Get("memorycache");
            if (cacheData != null)
            {
                return cacheData;
            }
            lock (_lock)
            {
                var expirationTime = DateTimeOffset.Now.AddMinutes(3);
                cacheData = _dbContext.Employees.ToList(); //From Database
                MemoryCacheService<List<Employee>>.Add("memorycache", cacheData, expirationTime);
            }
            return cacheData;
        }

        [HttpPost("memorypostcahce")]
        public async Task<Employee> PostMemoryCache(Employee value)
        {
            var obj = await _dbContext.Employees.AddAsync(value);
            MemoryCacheService<List<Employee>>.Delete("memorycache");
            _dbContext.SaveChanges();
            return obj.Entity;
        }
        #endregion
    }
}
