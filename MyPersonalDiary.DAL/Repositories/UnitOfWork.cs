using AutoDependencyRegistration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyPersonalDiary.DAL.ApplicationDbContext;
using MyPersonalDiary.DAL.InterfacesRepositories;

namespace MyPersonalDiary.DAL.Repositories
{
    [RegisterClassAsScoped]
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IInviteRepository _inviteRepository;

        public UnitOfWork(AppDbContext context)
        {
            _context = context; 
        }

        public IInviteRepository InviteRepository => _inviteRepository ??= new InviteRepository(_context);

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
