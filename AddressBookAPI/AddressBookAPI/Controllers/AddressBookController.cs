using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using RepositoryLayer.Entity;
using RepositoryLayer.Context;

namespace AddressBookAPI.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class AddressBookController : ControllerBase
    {
        private readonly AddressBookDBContext _context;
        public AddressBookController(AddressBookDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all contacts from the Address Book
        /// </summary>
        /// <returns>A list of all contacts.</returns>
        [HttpGet]
        public ActionResult<IEnumerable<AddressBookEntity>> GetContacts()
        {
            var contacts = _context.AddressBooks.ToList();

            if (contacts.Count == 0)
                return NotFound(new { message = "No contacts found" });

            return Ok(new { message = "Contacts retrieved successfully", data = contacts });
        }

        /// <summary>
        /// Retrieves a specific contact by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The requested contact if found.</returns>
        [HttpGet("{id}")]
        public ActionResult<AddressBookEntity> GetContact(int id)
        {
            var contact = _context.AddressBooks.Find(id);

            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            return Ok(new { message = "Contact retrieved successfully", data = contact });
        }

        /// <summary>
        /// Adds a new contact to the Address Book
        /// </summary>
        /// <param name="contact"></param>
        /// <returns>The created contact</returns>
        [HttpPost]
        public ActionResult<AddressBookEntity> CreateContact([FromBody] AddressBookEntity contact)
        {
            if (contact == null)
                return BadRequest(new { message = "Invalid contact data." });

            _context.AddressBooks.Add(contact);
            _context.SaveChanges();

            return Ok(new { message = "Contact added successfully.", data = contact });
        }

        /// <summary>
        /// Updates an existing contact
        /// </summary>
        /// <param name="id"></param>
        /// <param name="contact"></param>
        /// <returns>A success message if the update is successful</returns>
        [HttpPut("{id}")]
        public IActionResult UpdateContact(int id, [FromBody] AddressBookEntity contact)
        {
            var existingContact = _context.AddressBooks.Find(id);
            if (existingContact == null)
                return NotFound(new { message = "Contact not found" });

            existingContact.Name = contact.Name;
            existingContact.PhoneNumber = contact.PhoneNumber;

            _context.SaveChanges();

            return Ok(new { message = "Contact updated successfully", data = existingContact });
        }

        /// <summary>
        /// Deletes a contact from the Address Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A success message if the contact is deleted</returns>
        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contact = _context.AddressBooks.Find(id);

            if (contact == null)
                return NotFound(new { message = "Contact not found" });

            _context.AddressBooks.Remove(contact);
            _context.SaveChanges();

            return Ok(new { message = "Contact deleted successfully" });
        }
    }
}
