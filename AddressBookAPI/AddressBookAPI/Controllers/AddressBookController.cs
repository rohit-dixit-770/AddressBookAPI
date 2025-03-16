using Microsoft.AspNetCore.Mvc;
using ModelLayer.Model;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using ModelLayer.Validator;
using BusinessLayer.Interface;

namespace AddressBookAPI.Controllers
{
    /// <summary>
    /// Controller for managing Address Book contacts.
    /// </summary>
    [ApiController]
    [Route("/AddressBook")]
    public class AddressBookController : Controller
    {
        private readonly IAddressBookServiceBL _addressBookServiceBL;
        private readonly AddressBookEntryValidator _validator;

        public AddressBookController(IAddressBookServiceBL addressBookServiceBL)
        {
            _addressBookServiceBL = addressBookServiceBL;
            _validator = new AddressBookEntryValidator();
        }

        /// <summary>
        /// Retrieves all contacts from the address book
        /// </summary>
        /// <returns>List of contacts</returns>
        [HttpGet]
        public ActionResult<ResponseModel<IEnumerable<AddressBookEntry>>> GetContacts()
        {
            var contacts = _addressBookServiceBL.GetContacts();

            if (!contacts.Any())
            {
                return NotFound(new ResponseModel<IEnumerable<AddressBookEntry>>
                {
                    Success = false,
                    Message = "No contacts found",
                });
            }

            return Ok(new ResponseModel<IEnumerable<AddressBookEntry>>
            {
                Success = true,
                Message = "Contacts retrieved successfully",
                Data = contacts
            });
        }

        /// <summary>
        /// Retrieves a contact by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Contact details</returns>
        [HttpGet("{id}")]
        public ActionResult<ResponseModel<AddressBookEntry>> GetContact(int id)
        {
            var contact = _addressBookServiceBL.GetContact(id);

            if (contact == null)
            {
                return NotFound(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = "Contact not found",
                });
            }

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact retrieved successfully",
                Data = contact
            });
        }

        /// <summary>
        /// Creates a new contact in the address book
        /// </summary>
        /// <param name="contact">Contact details</param>
        /// <returns>Created contact</returns>
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

            var createdContact = _addressBookServiceBL.CreateContact(contact);
            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact added successfully",
                Data = createdContact
            });
        }

        /// <summary>
        /// Updates an existing contact by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <param name="contact">Updated contact details</param>
        /// <returns>Updated contact</returns>
        [HttpPut("{id}")]
        public ActionResult<ResponseModel<AddressBookEntry>> UpdateContact(int id, [FromBody] AddressBookEntry contact)
        {
            var updatedContact = _addressBookServiceBL.UpdateContact(id, contact);

            if (updatedContact == null)
            {
                return NotFound(new ResponseModel<AddressBookEntry>
                {
                    Success = false,
                    Message = "Contact not found",
                });
            }

            return Ok(new ResponseModel<AddressBookEntry>
            {
                Success = true,
                Message = "Contact updated successfully",
                Data = updatedContact
            });
        }

        /// <summary>
        /// Deletes a contact by ID
        /// </summary>
        /// <param name="id">Contact ID</param>
        /// <returns>Success message</returns>
        [HttpDelete("{id}")]
        public ActionResult<ResponseModel<string>> DeleteContact(int id)
        {
            if (!_addressBookServiceBL.DeleteContact(id))
            {
                return NotFound(new ResponseModel<string>
                {
                    Success = false,
                    Message = "Contact not found",
                });
            }

            return Ok(new ResponseModel<string>
            {
                Success = true,
                Message = "Contact deleted successfully",
            });
        }
    }
}
