using NetFinal.DataModels;

namespace NetFinal.DataManagers.Users
{
    public abstract class IUserManager
    {
        public abstract User? AddUser();
        public abstract User? GetUser();
        public abstract void AddUserRating();
        public abstract void ShowUserRatings();
        public abstract void AddOccupation();
        public abstract void DisplayUsers();
        public abstract void UserRatingSearch();
        
    }
}