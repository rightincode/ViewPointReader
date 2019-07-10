using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ViewPointReader.Core.Models;
using ViewPointReader.Data.Interfaces;

namespace ViewPointReader.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionsController : ControllerBase
    {
        private readonly IViewPointReaderRepository _repository;

        public SubscriptionsController(IViewPointReaderRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> Get()
        {
            var result = await _repository.GetFeedSubscriptionsAsync();

            return Ok();
        }

        [HttpPost]
        public async Task Post([FromBody] FeedSubscription feedSubscription)
        {
            await _repository.SaveFeedSubscriptionAsync(feedSubscription);
        }

        [HttpPost]
        [Route("buildmodel")]
        public async Task BuildModel()
        {
            var modelBuilder = new ModelBuilder.ModelBuilder(_repository);
            await modelBuilder.BuildModel();
        }

        [HttpPost]
        [Route("scorefeed")]
        public async Task<float> ScoreFeed([FromBody] FeedSubscription feedSubscription)
        {
            var modelBuilder = new ModelBuilder.ModelBuilder(_repository);
            return await modelBuilder.ScoreFeed(feedSubscription);
        }
    }
}