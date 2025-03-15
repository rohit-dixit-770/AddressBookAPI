using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using RepositoryLayer.Context;
using ModelLayer.Model;
using RepositoryLayer.Entity;
using FluentValidation;
using ModelLayer.Validator;

namespace AddressBookAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly AddressBookDBContext _context;
        private readonly IMapper _mapper;
        private readonly AddressBookEntryValidator _validator;

        public AddressBookController(AddressBookDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _validator = new AddressBookEntryValidator();
        }

        /// <summary>
        /// Retrieves all contacts
        /// </summary>
        /// <returns>List of contacts</returns>
        [HttpGet]
        public ActionResult<ResponseModel<IEnumerable<AddressBookEntry>>> GetContacts()
        {
            var contacts = _context.AddressBooks.ToList();
            if (contacts.Count == 0)
            {
                return NotFound(new ResponseModel<IEnumerable<AddressBookEntry>>
                {
                    Success = false,
                    Message = "No contacts found",
                });
            }

            var contactDTOs = _mapper.Map<IEnumerable<AddressBookEntry>>(contacts);
            return Ok(new ResponseModel<IEnumerable<AddressBookEntry>>
            {
                Success = true,
                Message = "Contacts retrieved successfully",
                Data = contactDTOs
            });
        }

        /// <summary>
        /// Retrieves a contact by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Contact details</returns>
        [HttpGet("{id}")]
        public ActionResult<ResponseModel<AddressBookEntry>> GetContact(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = "Contact not found",
                });
            }

            var contactDTO = _mapper.Map<AddressBookEntry>(contact);
            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact retrieved successfully",
                Data = contactDTO
            });
        }

        /// <summary>
        /// Adds a new contact to the address book
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>The created contact</returns>
        [HttpPost]
        public ActionResult<ResponseModel<AddressBookEntry>> CreateContact([FromBody] AddressBookEntry contact)
        {
            var validationResult = _validator.Validate(contact);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                });
            }

            var newContact = _mapper.Map<AddressBookEntity>(contact);
            _context.AddressBooks.Add(newContact);
            _context.SaveChanges();

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact added successfully",
                Data = contact
            });
        }

        /// <summary>
        /// Updates an existing contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns>The updated contact</returns>
        [HttpPut("{id}")]
        public ActionResult<ResponseModel<AddressBookEntry>> UpdateContact(int id, [FromBody] AddressBookEntry contact)
        {
            var existingContact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (existingContact == null)
            {
                return NotFound(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = "Contact not found",
                });
            }

            var validationResult = _validator.Validate(contact);
            if (!validationResult.IsValid)
            {
                return BadRequest(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))
                });
            }

            _mapper.Map(contact, existingContact);
            _context.SaveChanges();

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact updated successfully",
                Data = contact
            });
        }

        /// <summary>
        /// Deletes a contact by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A success or failure message</returns>
        [HttpDelete("{id}")]
        public ActionResult<ResponseModel<string>> DeleteContact(int id)
        {
            var contact = _context.AddressBooks.FirstOrDefault(c => c.Id == id);
            if (contact == null)
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Contact not found",
                    Data = string.Empty
                });
            }

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();

            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Contact deleted successfully",
                Data = string.Empty
            });
        }
    }
}
