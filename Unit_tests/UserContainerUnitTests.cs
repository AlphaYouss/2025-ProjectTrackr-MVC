using System.Data;
using ProjectTrackr.Models;
using ProjectTrackr.Containers;
using Unit_tests.Stubs;

namespace Unit_tests
{
    public class UserContainerTests
    {
        private readonly UserContainerStubs ucs;
        private readonly UserContainer uc;

        public UserContainerTests()
        {
            ucs = new UserContainerStubs();
            uc = new UserContainer(ucs);
        }

        [Fact]
        public void CreateUser_Positive_ReturnsGuid()
        {
            ucs.successReturnValue = true;
            User user = new() { id = Guid.NewGuid() };

            Guid result = uc.CreateUser(user);

            Assert.NotEqual(Guid.Empty, result);
        }

        [Fact]
        public void CreateUser_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.CreateUser(null));
        }

        [Fact]
        public void EditEmail_Positive_ReturnsTrue()
        {
            ucs.successReturnValue = true;
            string newEmail = "test@example.com";

            bool result = uc.EditEmail(ucs.user, newEmail);

            Assert.True(result);
        }

        [Fact]
        public void EditEmail_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.EditEmail(null, null));
        }

        [Fact]
        public void EditPassword_Positive_ReturnsTrue()
        {
            ucs.successReturnValue = true;
            string newPassword = "password123";

            bool result = uc.EditPassword(ucs.user, newPassword);

            Assert.True(result);
        }

        [Fact]
        public void EditPassword_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.EditPassword(null, null));
        }

        [Fact]
        public void EmailExists_Positive_ReturnsTrue()
        {
            ucs.existReturnValue = true;
            string email = "test@example.com";

            bool result = uc.EmailExists(email);

            Assert.True(result);
        }

        [Fact]
        public void EmailExists_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.EmailExists(""));
        }

        [Fact]
        public void FetchPassword_Positive_ReturnsString()
        {
            ucs.existReturnValue = true;
            string usernameEmail = "test@example.com";

            string result = uc.FetchPassword(usernameEmail);

            Assert.False(string.IsNullOrEmpty(result));
        }

        [Fact]
        public void FetchPassword_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.FetchPassword(""));
        }

        [Fact]
        public void GetUserDetails_Positive_ReturnsDataTable()
        {
            ucs.existReturnValue = true;

            DataTable result = uc.GetUserDetails(ucs.user);

            Assert.NotNull(result);
        }

        [Fact]
        public void GetUserDetails_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.GetUserDetails(new User { id = Guid.Empty }));
        }

        [Fact]
        public void UserNameEmailExists_Positive_ReturnsTrue()
        {
            ucs.existReturnValue = true;
            string usernameEmail = "user@test.com";

            bool result = uc.UserNameEmailExists(usernameEmail);

            Assert.True(result);
        }

        [Fact]
        public void UserNameEmailExists_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.UserNameEmailExists(""));
        }

        [Fact]
        public void UserNameExists_Positive_ReturnsTrue()
        {
            ucs.existReturnValue = true;
            string username = "user1";

            bool result = uc.UserNameExists(username);

            Assert.True(result);
        }

        [Fact]
        public void UserNameExists_Negative_ThrowsNullReferenceException()
        {
            Assert.Throws<NullReferenceException>(() => uc.UserNameExists(""));
        }
    }
}