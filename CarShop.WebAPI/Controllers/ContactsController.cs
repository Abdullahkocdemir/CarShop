using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using DTOsLayer.WebApiDTO.ContactDTO; // Yeni DTO'larınızı buraya ekledik
using EntityLayer.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : BaseEntityController
    {
        private readonly IContactService _contactService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Contact";

        public ContactsController(IContactService contactService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _contactService = contactService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetListAllContacts()
        {
            var values = _mapper.Map<List<ResultContactDTO>>(_contactService.BGetListAll());
            return Ok(values);
        }

        [HttpGet("{id}")]
        public IActionResult GetContactById(int id)
        {
            var value = _contactService.BGetById(id);
            return Ok(value);
        }

        [HttpPost]
        public IActionResult CreateContact(CreateContactDTO dto)
        {

            var contact = _mapper.Map<Contact>(dto);
            _contactService.BAdd(contact);
            PublishEntityCreated(contact);

            return Ok(new { Message = "İletişim mesajı başarıyla eklendi ve mesaj gönderildi.", ContactId = contact.ContactId });
        }

        [HttpPut]
        public IActionResult UpdateContact(UpdateContactDTO dto)
        {
            var existingContact = _contactService.BGetById(dto.ContactId);
            _mapper.Map(dto, existingContact);
            _contactService.BUpdate(existingContact);
            PublishEntityUpdated(existingContact);

            return Ok(new { Message = "İletişim mesajı başarıyla güncellendi ve mesaj yayınlandı.", ContactId = existingContact.ContactId });
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contactToDelete = _contactService.BGetById(id);

            _contactService.BDelete(contactToDelete);
            PublishEntityDeleted(contactToDelete);
            return Ok(new { Message = "İletişim mesajı başarıyla silindi ve mesaj yayınlandı.", ContactId = id });
        }
    }
}