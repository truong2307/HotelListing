using HotelListing.Core.IRepository;
using HotelListing.Core.Model;
using HotelListing.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using X.PagedList;

namespace HotelListing.Core.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        internal DbSet<T> dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            dbSet = context.Set<T>();
        }

        public async Task Delete(int id)
        {
            var entity = await dbSet.FindAsync(id);
            dbSet.Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> filter
            , Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = dbSet;
            if (include != null)
            {
                query = include(query);
                // example query => query.Include(i => i.Hotels);
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(filter);
            // query.AsNoTracking() : truy vấn data nhưng không được theo dõi kết quả truy vấn bởi ngữ cảnh (context)
            //  , chỉ thích hợp với get dữ liệu ra, dùng để tối ưu hóa hiệu suất
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> filter = null
            , Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null
            , Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                query = include(query);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<IPagedList<T>> GetPageList(RequestParams requestParam
            , Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null)
        {
            IQueryable<T> query = dbSet;

            if (include != null)
            {
                query = include(query);
            }

            return await query.AsNoTracking()
                .ToPagedListAsync(requestParam.PageNumber, requestParam.PageSize);
        }

        public async Task Insert(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            dbSet.Attach(entity);   // gắn entity request vào context với trạng thái unchanged giống dbset.Entry(entity).State  = EntityState.Unchanged;
            _context.Entry(entity).State = EntityState.Modified;
            //  _context.Entry() gắn data (entity) với trạng thái được chỉ định là modified
            //  và khi savechanges(), sẽ thực hiện câu lệnh update, thuộc tính nào đc thay đổi sẽ tự động cập nhật, các thuộc tính khác bỏ qua.
        }
    }
}
