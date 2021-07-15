using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Repository
{
    public class DapperUnitOfWork : IDisposable, IDapperUnitOfWork
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        private IPersonRepository _personRepository;
        public DapperUnitOfWork(SqlConnection sqlConnection)
        {
            _connection = sqlConnection;
            _connection.Open();
            _transaction = _connection.BeginTransaction();

        }

        public IPersonRepository PersonRepository
        {
            set { }
            get
            {
                return _personRepository ?? (_personRepository = new PersonRepository(_transaction));
            }
        }


        public async Task Commit()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                _transaction.Dispose();
                _transaction = _connection.BeginTransaction();
                ResetRepositories();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
        private void ResetRepositories()
        {
            PersonRepository = null;
        }

    }

}
