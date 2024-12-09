using Moq;
using Xunit;
using StockReader.Services;
using Blazored.SessionStorage;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;
using System.Threading;

namespace StockReader.Tests.Services
{
    public class AuthorizationServiceTests
    {
        private readonly Mock<ISessionStorageService> _mockSessionStorage;
        private readonly Mock<NavigationManager> _mockNavigationManager;
        private readonly AuthorizationService _authorizationService;

        public AuthorizationServiceTests()
        {
            _mockSessionStorage = new Mock<ISessionStorageService>();
            _mockNavigationManager = new Mock<NavigationManager>();
            _authorizationService = new AuthorizationService(_mockNavigationManager.Object, _mockSessionStorage.Object);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnTrue_WhenCredentialsAreCorrect()
        {
            // Arrange
            var username = "testuser";
            var password = "testpassword";

            // Mock the session storage to return correct username and password
            _mockSessionStorage.Setup(s => s.GetItemAsync<string>("username", It.IsAny<CancellationToken>()))
                            .ReturnsAsync(username);
            _mockSessionStorage.Setup(s => s.GetItemAsync<string>("password", It.IsAny<CancellationToken>()))
                            .ReturnsAsync(password);

            // Act
            var result = await _authorizationService.LoginAsync(username, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFalse_WhenCredentialsAreIncorrect()
        {
            // Arrange
            var username = "testuser";
            var password = "wrongpassword";

            // Mock the session storage to return correct username but incorrect password
            _mockSessionStorage.Setup(s => s.GetItemAsync<string>("username", It.IsAny<CancellationToken>()))
                            .ReturnsAsync("testuser");
            _mockSessionStorage.Setup(s => s.GetItemAsync<string>("password", It.IsAny<CancellationToken>()))
                            .ReturnsAsync("testpassword");

            // Act
            var result = await _authorizationService.LoginAsync(username, password);

            // Assert
            Assert.False(result);
        }
    }
}