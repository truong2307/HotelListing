using HotelListing.Data.Repository.IRepository;
using HotelListing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<Country> _countries;
        private IGenericRepository<Hotel> _hotels;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Country> Countries => _countries ?? new GenericRepository<Country>(_context);

        public IGenericRepository<Hotel> Hotels => _hotels ?? new GenericRepository<Hotel>(_context);

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this); // Thông báo cho Garbage Collector (GC) rằng đối tượng này (unitofwork) đã được dọn sạch hoàn toàn.
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
