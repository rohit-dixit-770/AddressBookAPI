using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModelLayer.Model;

namespace BusinessLayer.Interface
{
    public interface IAddressBookServiceBL
    {
        IEnumerable<AddressBookEntry> GetContacts();
        AddressBookEntry GetContact(int id);
        AddressBookEntry CreateContact(AddressBookEntry contact);
        AddressBookEntry UpdateContact(int id, AddressBookEntry contact);
        bool DeleteContact(int id);
    }
}
