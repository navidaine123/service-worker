using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories
{
    public interface IPersonRepository
    {
        Task<int> Add(Person entity);
        Task Update(Person entity);
        Task Remove(int tblPersonId);
    }
}
