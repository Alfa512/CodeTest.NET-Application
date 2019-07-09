using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Data.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        private IDataContext _context;
        public UserRepository(IDataContext context) : base(context)
        {
            _context = context;
        }

        public override void Add(User user) //where User : class
        {

        }
    }
}
