using Common.LogModels;
using Data.Models;
using EasyCaching.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repository;
using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PersonService : IPersonService
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly IEasyCachingProviderFactory _cachingProviderFactory;
        private readonly IDapperUnitOfWork _dapperUow;
        private readonly ILogger<PersonService> _logger;
        private readonly IPersonRepository _personRepository;

        public PersonService(IEasyCachingProviderFactory cachingProviderFactory,IDapperUnitOfWork dapperUnitOfWork,ILogger<PersonService> logger)
        {
            _cachingProviderFactory = cachingProviderFactory;
            _dapperUow = dapperUnitOfWork;
            _logger = logger;
            _cachingProvider = cachingProviderFactory.GetCachingProvider("navid");
            _personRepository = _dapperUow.PersonRepository;

        }

        public async Task AddPerson(Person person)
        {
            try
            {
                var key =await _personRepository.Add(person);
                await _dapperUow.Commit();
                _cachingProvider.Set<string>(key.ToString(), JsonConvert.SerializeObject(person), TimeSpan.FromDays(100));
                _logger.LogInformation("Person has inserted{@SqlInsertLog}", new LogModel(person));
            }
            catch (Exception ex)
            {
                _logger.LogError("Person" + person + " insert has failed{@SqlInsertError}", new LogModel(new ErrorLogModel(ex)));
            }
        }
    }
}
