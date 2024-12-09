using Microsoft.EntityFrameworkCore;
using StockReader.Models;
using StockReader.Services;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace StockReader.Tests.Services
{
    public class UserServiceTests
    {
        private readonly UserService _userService;
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public UserServiceTests()
        {
            // Set up InMemory database for testing
            _dbContextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                                .UseInMemoryDatabase(databaseName: "TestDatabase")
                                .Options;

            var context = new ApplicationDbContext(_dbContextOptions);
            _userService = new UserService(context);
        }

        // Test for GetRegisteredUsersAsync method
        [Fact]
        public async Task GetRegisteredUsersAsync_ShouldReturnEmptyList_WhenNoUsersExist()
        {
            // Arrange - no users in the database

            // Act
            var users = await _userService.GetRegisteredUsersAsync();

            // Assert
            Assert.Empty(users); // Expecting an empty list
        }

        [Fact]
        public async Task GetRegisteredUsersAsync_ShouldReturnListOfUsers_WhenUsersExist()
        {
            // Arrange
            var context = new ApplicationDbContext(_dbContextOptions);
            context.RegisteredUsers.Add(new RegisteredUser { Id = Guid.NewGuid(), UserName = "user1", UserPassword = "password1" });
            context.RegisteredUsers.Add(new RegisteredUser { Id = Guid.NewGuid(), UserName = "user2", UserPassword = "password2" });
            await context.SaveChangesAsync(); // Save changes to the in-memory database

            // Act
            var users = await _userService.GetRegisteredUsersAsync();

            // Assert
            Assert.NotEmpty(users); // The list should not be empty
            Assert.Equal(2, users.Count); // Expecting 2 users
        }

        // Test for AddUserAsync method
        [Fact]
        public async Task AddUserAsync_ShouldAddUser_WhenUserIsValid()
        {
            // Arrange
            var newUser = new RegisteredUser { Id = Guid.NewGuid(), UserName = "newuser", UserPassword = "newpassword" };

            // Act
            await _userService.AddUserAsync(newUser);

            var context = new ApplicationDbContext(_dbContextOptions);
            var addedUser = await context.RegisteredUsers.FirstOrDefaultAsync(u => u.UserName == "newuser");

            // Assert
            Assert.NotNull(addedUser); // The user should be added to the database
            Assert.Equal("newuser", addedUser.UserName); // The user's username should be "newuser"
        }

        [Fact]
        public async Task AddUserAsync_ShouldThrowException_WhenUserAlreadyExists()
        {
            // Arrange
            var context = new ApplicationDbContext(_dbContextOptions);
            var existingUser = new RegisteredUser { Id = Guid.NewGuid(), UserName = "existinguser", UserPassword = "password" };
            context.RegisteredUsers.Add(existingUser);
            await context.SaveChangesAsync();

            var duplicateUser = new RegisteredUser { Id = Guid.NewGuid(), UserName = "existinguser", UserPassword = "newpassword" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => _userService.AddUserAsync(duplicateUser)); // Expect an exception when adding a user with the same username
        }
    }
}
