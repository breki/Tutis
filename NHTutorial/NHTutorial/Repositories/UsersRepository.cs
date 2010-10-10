using NHibernate;
using NHTutorial.Model;

namespace NHTutorial.Repositories
{
    public interface IUsersRepository
    {
        void CreateUser(ISession session, User user);
        void DeleteUser(ISession session, int userId);
        void DeleteUser(ISession session, string userEmailAddress);
    }

    public class UsersRepository : IUsersRepository
    {
        public void CreateUser(ISession session, User user)
        {
            session.Save(user);
        }

        public void DeleteUser(ISession session, int userId)
        {
            session.Delete("from User where Id=?", userId);
        }

        public void DeleteUser(ISession session, string userEmailAddress)
        {
            session.Delete(
                "from User u where u.EmailAddress=?", 
                userEmailAddress,
                NHibernate.NHibernateUtil.String);
        }
    }
}