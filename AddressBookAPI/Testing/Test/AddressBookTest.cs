using System.Collections.Generic;
using BusinessLayer.Interface;
using BusinessLayer.Service;
using ModelLayer.Model;
using Moq;
using NUnit.Framework;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace Testing.TestClasses
{
    public class AddressBookServiceBLTests
    {
        private Mock<IAddressBookServiceBL> _addressBookServiceMock;

        [SetUp]
        public void Setup()
        {
            _addressBookServiceMock = new Mock<IAddressBookServiceBL>();
        }

        [Test]
        public void GetContacts_ReturnsListOfContacts()
        {
            var contacts = new List<AddressBookEntry>
            {
                new() { Id = 1, Name = "Test", PhoneNumber = "1234567890", Address = "XYZ-Nagar" },
                new() { Id = 2, Name = "Test2", PhoneNumber = "9876543210", Address = "ABC-Nagar" }
            };

            _addressBookServiceMock.Setup(service => service.GetContacts()).Returns(contacts);

            var result = _addressBookServiceMock.Object.GetContacts();

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetContact_ValidId_ReturnsContact()
        {
            var contact = new AddressBookEntry
            {
                Id = 1,
                Name = "Test",
                PhoneNumber = "1234567890",
                Address = "XYZ-Nagar"
            };

            _addressBookServiceMock.Setup(service => service.GetContact(1)).Returns(contact);

            var result = _addressBookServiceMock.Object.GetContact(1);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void GetContact_InvalidId_ReturnsNull()
        {
            _addressBookServiceMock.Setup(service => service.GetContact(99)).Returns((AddressBookEntry)null);

            var result = _addressBookServiceMock.Object.GetContact(99);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void CreateContact_ValidInput_ReturnsCreatedContact()
        {
            var request = new AddressBookEntry
            {
                Name = "Test",
                PhoneNumber = "1234567890",
                Address = "XYZ-Nagar"
            };

            var response = new AddressBookEntry
            {
                Id = 1,
                Name = "Test",
                PhoneNumber = "1234567890",
                Address = "XYZ-Nagar"
            };

            _addressBookServiceMock.Setup(service => service.CreateContact(request)).Returns(response);

            var result = _addressBookServiceMock.Object.CreateContact(request);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void UpdateContact_ValidInput_ReturnsUpdatedContact()
        {
            var updatedContact = new AddressBookEntry
            {
                Id = 1,
                Name = "Test",
                PhoneNumber = "1234567890",
                Address = "XYZ-Nagar"
            };

            _addressBookServiceMock.Setup(service => service.UpdateContact(1, updatedContact)).Returns(updatedContact);

            var result = _addressBookServiceMock.Object.UpdateContact(1, updatedContact);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Test"));
        }

        [Test]
        public void UpdateContact_InvalidId_ReturnsNull()
        {
            var updatedContact = new AddressBookEntry
            {
                Name = "Test",
                PhoneNumber = "1234567890",
                Address = "XYZ-Nagar"
            };

            _addressBookServiceMock.Setup(service => service.UpdateContact(99, updatedContact)).Returns((AddressBookEntry)null);

            var result = _addressBookServiceMock.Object.UpdateContact(99, updatedContact);

            Assert.That(result, Is.Null);
        }

        [Test]
        public void DeleteContact_ValidId_ReturnsTrue()
        {
            _addressBookServiceMock.Setup(service => service.DeleteContact(1)).Returns(true);

            var result = _addressBookServiceMock.Object.DeleteContact(1);

            Assert.That(result, Is.True);
        }

        [Test]
        public void DeleteContact_InvalidId_ReturnsFalse()
        {
            _addressBookServiceMock.Setup(service => service.DeleteContact(99)).Returns(false);

            var result = _addressBookServiceMock.Object.DeleteContact(99);

            Assert.That(result, Is.False);
        }
    }
}
