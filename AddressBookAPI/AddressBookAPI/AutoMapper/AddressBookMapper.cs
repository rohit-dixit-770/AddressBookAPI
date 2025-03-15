using AutoMapper;
using ModelLayer.Model;
using RepositoryLayer.Entity;

namespace AddressBookAPI.AutoMapper
{
    public class AddressBookMapper : Profile
    {
        public AddressBookMapper()
        {
            CreateMap<AddressBookEntity, AddressBookEntry>().ReverseMap();
        }
    }
}
