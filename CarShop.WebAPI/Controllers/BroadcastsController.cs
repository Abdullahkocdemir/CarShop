using AutoMapper;
using BusinessLayer.Abstract;
using BusinessLayer.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CarShop.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BroadcastsController : BaseEntityController
    {
        private readonly IBroadcastService _broadcastService;
        private readonly IMapper _mapper;
        protected override string EntityTypeName => "Broadcast";

        public BroadcastsController(IBroadcastService broadcastService, IMapper mapper, EnhancedRabbitMQService rabbitMqService)
            : base(rabbitMqService)
        {
            _broadcastService = broadcastService;
            _mapper = mapper;
        }


    }
}