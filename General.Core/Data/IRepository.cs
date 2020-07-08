using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        DbContext DbContext { get;  }

        /// <summary>
        /// 
        /// </summary>
        DbSet<TEntity> Entities { get; }

        /// <summary>
        /// 
        /// </summary>
        IQueryable<TEntity> Table { get; }

        /// <summary>
        /// 通过主键ID获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity getById(object id);

        void insert(TEntity entity, bool isSave = true);

        void update(TEntity entity, bool isSave = true);

        void delete(TEntity entity, bool isSave = true);

    }
}
