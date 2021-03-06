using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataLayer;

namespace BusinessLayer
{
    public abstract class BaseService<T> : IDisposable
    {
        protected MyDBDataContext _context;
        
        public BaseService()
        {
            if (_context == null)
            {
                _context = new MyDBDataContext();
            }
        }
        public void Dispose()
        {
            _context.SubmitChanges();
            _context.Dispose();
        }

        public MyDBDataContext Context { get { return _context; } }

        //// Insert object T
        //// Return id of new T
        //public abstract int Insert(T e);

        //// Update object T
        //// Return id of T update
        //public abstract int Update(T e);

        //// Delete object by ID
        //// Return id of object
        //public abstract int Delete(int entityId);

        //// Get entity by ID
        //public abstract T GetById(int entityId);

        //// Get list entity (all)
        //public abstract List<T> List();
    }
}
