using AutoMapper;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace BusinessLayer.AutoMapper
{
    public class AddressBookMapper : Profile
    {
        public AddressBookMapper()
        {
            CreateMap<AddressBookEntity, AddressBookEntry>().ReverseMap();
        }
    }
}
