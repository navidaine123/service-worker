using Repository.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IDapperUnitOfWork
    {
        IPersonRepository PersonRepository { get; }
        Task Commit();
    }
}
