using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DurjoyBDNews24.Application.Interfaces
{
    public interface ISlugService
    {
        string Generate(string title);
        Task<string> GenerateUniqueAsync(string title);
    }
}
