﻿using HotelListing.Model;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Data.Repository.IRepository
{
    public interface IGenericRepository<T> where T: class
    {
        Task<IList<T>> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
            ,Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
            );
        Task<IPagedList<T>> GetPageList(
            RequestParams requestParam
            ,Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null
            );
        Task<T> Get(Expression<Func<T, bool>> filter
            , Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task Insert(T entity);
        Task InsertRange(IEnumerable<T> entities);
        Task Delete(int id);
        void DeleteRange(IEnumerable<T> entities);
        void Update(T entity);
    }
}
