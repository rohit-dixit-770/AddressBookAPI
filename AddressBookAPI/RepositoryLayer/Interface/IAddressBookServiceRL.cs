using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RepositoryLayer.Entity;

namespace RepositoryLayer.Interface
{
    public interface IAddressBookServiceRL
    {
        IEnumerable<AddressBookEntity> GetContacts();
        AddressBookEntity GetContact(int id);
        AddressBookEntity CreateContact(AddressBookEntity contact);
        AddressBookEntity UpdateContact(int id, AddressBookEntity contact);
        bool DeleteContact(int id);
    }
}
