using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public AddressBookServiceBL(IAddressBookServiceRL addressBookServiceRL, IMapper mapper)
        {
            _addressBookServiceRL = addressBookServiceRL;
            _mapper = mapper;
        }

        public IEnumerable<AddressBookEntry> GetContacts()
        {
            var contacts = _addressBookServiceRL.GetContacts();
            return _mapper.Map<IEnumerable<AddressBookEntry>>(contacts);
        }

        public AddressBookEntry GetContact(int id)
        {
            var contact = _addressBookServiceRL.GetContact(id);
            return contact != null ? _mapper.Map<AddressBookEntry>(contact) : null;
        }

        public AddressBookEntry CreateContact(AddressBookEntry contact)
        {
            var newContact = _mapper.Map<AddressBookEntity>(contact);
            var addedContact = _addressBookServiceRL.CreateContact(newContact);
            return _mapper.Map<AddressBookEntry>(addedContact);
        }

        public AddressBookEntry UpdateContact(int id, AddressBookEntry contact)
        {
            var existingContact = _addressBookServiceRL.GetContact(id);
            if (existingContact == null)
                return null;

            _mapper.Map(contact, existingContact);
            var updatedContact = _addressBookServiceRL.UpdateContact(existingContact);
            return _mapper.Map<AddressBookEntry>(updatedContact);
        }

        public bool DeleteContact(int id)
        {
            return _addressBookServiceRL.DeleteContact(id);
        }
    }
}
