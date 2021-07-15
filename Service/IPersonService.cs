using Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IPersonService
    {
        Task AddPerson(Person person);
    }
}
