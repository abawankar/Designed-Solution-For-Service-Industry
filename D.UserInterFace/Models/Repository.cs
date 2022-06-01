using System.Collections.Generic;

namespace D.UserInterFace.Models
{
    public abstract class Repository<T>
    {
        public abstract T GetById(int id);
        public abstract IList<T> GetAll();
        public abstract void Edit(T obj);
        public abstract void Insert(T obj);
        public abstract bool Delete(int id);

    }



}