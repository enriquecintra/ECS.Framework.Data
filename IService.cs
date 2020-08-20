using System.Collections.Generic;

namespace ECS.Framework.Data
{
    public interface IService<T> : IRepository<T> where T : class
    {
    }
}
