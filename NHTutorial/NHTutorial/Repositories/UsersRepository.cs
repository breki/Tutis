using System;
using System.Collections.Generic;
using NHibernate;
using NHibernate.Criterion;
using NHTutorial.Model;

namespace NHTutorial.Repositories
{
    public interface IUsersRepository
    {
        void CreateUser(ISession session, User user);
        void DeleteUser(ISession session, int userId);
        void DeleteUser(ISession session, string userEmailAddress);
        User GetUserByEmailAddress(ISession session, string emailAddress);
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
            User user = GetUserByEmailAddress(session, userEmailAddress);
            session.Delete(
                "from Transaction t where t.User.Id=?`",
                user.Id,
                NHibernate.NHibernateUtil.Int32);

            session.Delete(user);
        }

        public User GetUserByEmailAddress(ISession session, string emailAddress)
        {
            IList<User> users = session.CreateCriteria<User>()
                .Add(Expression.Eq("EmailAddress", emailAddress))
                .List<User>();
            if (users.Count == 0)
                throw new KeyNotFoundException();

            return users[0];
        }
    }
}