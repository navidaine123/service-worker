
using Dapper;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{

    public class PersonRepository : IPersonRepository
    {
        private readonly IDbTransaction _transaction;
        private IDbConnection _connection;

        public PersonRepository(IDbTransaction transaction)
        {
            _transaction = transaction;
            _connection = transaction.Connection;
        }
        
        public async Task<int> Add(Person entity)
        {
            var key = _connection.ExecuteScalar<int>(@"INSERT INTO TblPerson(FirstName, LastName, Age)  
                                                VALUES(@FirstName, @LastName,@Age ); 
                                                SELECT  cast(SCOPE_IDENTITY() as int)",
                                                    entity, _transaction);

            entity.Id = key;

            return key;
        }

        public async Task Update(Person entity)
        {
                _connection.Execute(@"UPDATE TblPerson  
                                SET FirstName = @FirstName,  
                                    LastName = @LastName,  
                                    Age = @Age  
                                WHERE Id = @TblPersonId",
                                    new
                                    {
                                        FirstName = entity.FirstName,
                                        LastName = entity.LastName,
                                    }, _transaction);
           
        }

        public async Task Remove(int tblPersonId)
        {
            _connection.Execute(@"DELETE FROM TblPerson WHERE Id = @TblPersonId",
                                new
                                {
                                    Id = tblPersonId
                                }, _transaction);
        }
    }


}
