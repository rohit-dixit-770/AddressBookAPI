using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Context;
using RepositoryLayer.Entity;
using RepositoryLayer.Interface;

namespace RepositoryLayer.Service
{
    public class AddressBookServiceRL : IAddressBookServiceRL
    {
        private readonly AddressBookDBContext _context;

        public AddressBookServiceRL(AddressBookDBContext context)
        {
            _context = context;
        }

        public IEnumerable<AddressBookEntity> GetContacts()
        {
            return _context.AddressBooks.ToList();
        }

        public AddressBookEntity GetContact(int id)
        {
            var existingContact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if(existingContact == null)
            {
                return null;
            }
            return existingContact;
        }

        public AddressBookEntity CreateContact(AddressBookEntity contact)
        {
            _context.AddressBooks.Add(contact);
            _context.SaveChanges();
            return contact;
        }

        public AddressBookEntity UpdateContact(AddressBookEntity contact)
        {
            _context.AddressBooks.Update(contact);
            _context.SaveChanges();
            return contact;
        }

        public bool DeleteContact(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact != null)
            {
                _context.AddressBooks.Remove(contact);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
