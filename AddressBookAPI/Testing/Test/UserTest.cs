using BusinessLayer.Interface;
using ModelLayer.Model;
using Moq;
using NUnit.Framework;

namespace Testing.TestClasses
{
    public class UserTests
    {
        private Mock<IUserBL> _userServiceMock;

        [SetUp]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserBL>();
        }

        [Test]
        public void RegisterUser_ValidInput_ReturnsSuccess()
        {
            var userDto = new RegisterModel
            {
                FullName = "Test Name",
                Email = "test@example.com",
                Password = "test@1234"
            };

            _userServiceMock.Setup(service => service.RegisterUser(userDto))
                            .Returns(true);

            var result = _userServiceMock.Object.RegisterUser(userDto);

            Assert.That(result, Is.True);
        }

        [Test]
        public void RegisterUser_DuplicateEmail_ReturnsFalse()
        {
            var userDto = new RegisterModel
            {
                FullName = "Test Name",
                Email = "test@example.com",
                Password = "test@1234"
            };

            _userServiceMock.Setup(service => service.RegisterUser(userDto))
                            .Returns(false);

            var result = _userServiceMock.Object.RegisterUser(userDto);

            Assert.That(result, Is.False);
        }

        [Test]
        public void LoginUser_ValidCredentials_ReturnsToken()
        {
            var loginDto = new LoginModel
            {
                Email = "test@example.com",
                Password = "test@1234"
            };

            _userServiceMock.Setup(service => service.LoginUser(loginDto))
                            .Returns("jwt_token_here");

            var token = _userServiceMock.Object.LoginUser(loginDto);

            Assert.That(token, Is.Not.Null);
            Assert.That(token, Is.Not.Empty);
        }

        [Test]
        public void LoginUser_InvalidCredentials_ReturnsNull()
        {
            var loginDto = new LoginModel
            {
                Email = "wrong@example.com",
                Password = "wrong@1234"
            };

            _userServiceMock.Setup(service => service.LoginUser(loginDto))
                            .Returns((string)null);

            var token = _userServiceMock.Object.LoginUser(loginDto);

            Assert.That(token, Is.Null);
        }

   
    }
}
