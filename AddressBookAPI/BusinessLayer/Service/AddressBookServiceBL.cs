using System;
using System.Collections.Generic;
using AutoMapper;
using BusinessLayer.Interface;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace BusinessLayer.Service
{
    public class AddressBookServiceBL : IAddressBookServiceBL
    {
        private readonly IAddressBookServiceRL _addressBookServiceRL;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;

        public AddressBookServiceBL(IAddressBookServiceRL addressBookServiceRL, IMapper mapper, IRedisCacheService redisCacheService)
        {
            _addressBookServiceRL = addressBookServiceRL;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }

        public IEnumerable<AddressBookEntry> GetContacts()
        {
            string cacheKey = "AllContacts";

            var cachedContacts = _redisCacheService.GetCache(cacheKey);

            if (!string.IsNullOrEmpty(cachedContacts))
            {
                return System.Text.Json.JsonSerializer.Deserialize<IEnumerable<AddressBookEntry>>(cachedContacts);
            }

            var contacts = _addressBookServiceRL.GetContacts();
            var mappedContacts = _mapper.Map<IEnumerable<AddressBookEntry>>(contacts);

            _redisCacheService.SetCache(cacheKey, System.Text.Json.JsonSerializer.Serialize(mappedContacts), 30);
            return mappedContacts;
        }

        public AddressBookEntry GetContact(int id)
        {
            string cacheKey = $"Contact_{id}";
            var cachedContact = _redisCacheService.GetCache(cacheKey);

            if (!string.IsNullOrEmpty(cachedContact))
            {
                return System.Text.Json.JsonSerializer.Deserialize<AddressBookEntry>(cachedContact);
            }

            var contact = _addressBookServiceRL.GetContact(id);
            if (contact != null)
            {
                var mappedContact = _mapper.Map<AddressBookEntry>(contact);
                _redisCacheService.SetCache(cacheKey, System.Text.Json.JsonSerializer.Serialize(mappedContact), 30);
                return mappedContact;
            }

            return null;
        }

        public AddressBookEntry CreateContact(AddressBookEntry contact)
        {
            var newContact = _mapper.Map<AddressBookEntity>(contact);
            var addedContact = _addressBookServiceRL.CreateContact(newContact);
            var mappedContact = _mapper.Map<AddressBookEntry>(addedContact);

            _redisCacheService.RemoveCache("AllContacts");

            return mappedContact;
        }

        public AddressBookEntry UpdateContact(int id, AddressBookEntry contact)
        {
            var existingContact = _addressBookServiceRL.GetContact(id);
            if (existingContact == null)
                return null;

            _mapper.Map(contact, existingContact);
            var updatedContact = _addressBookServiceRL.UpdateContact(id, existingContact);
            var mappedUpdatedContact = _mapper.Map<AddressBookEntry>(updatedContact);

            string cacheKey = $"Contact_{id}";
            _redisCacheService.SetCache(cacheKey, System.Text.Json.JsonSerializer.Serialize(mappedUpdatedContact), 30);

            _redisCacheService.RemoveCache("AllContacts");

            return mappedUpdatedContact;
        }

        public bool DeleteContact(int id)
        {
            bool isDeleted = _addressBookServiceRL.DeleteContact(id);
            if (isDeleted)
            {
                string cacheKey = $"Contact_{id}";
                _redisCacheService.RemoveCache(cacheKey);
                _redisCacheService.RemoveCache("AllContacts");
            }
            return isDeleted;
        }
    }
}
