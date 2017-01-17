using GovITHub.Auth.Admin.Services;
using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Data.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    //[Authorize]
    [Route("api/[controller]")]
    public class OrganizationsController : Controller
    {
        private IOrganizationRepository _repository;

        public OrganizationsController(IOrganizationRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery]int currentPage, [FromQuery]int itemsPerPage,
            [FromQuery]bool sortAscending, [FromQuery]string sortBy)
        {
            var filter = new ModelQueryFilter(currentPage, itemsPerPage, sortAscending, sortBy);
            return new ObjectResult(_repository.Filter(filter));
        }

        [HttpGet("{id}", Name = "GetOrganization")]
        public IActionResult GetById(long id)
        {
            var item = _repository.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpPost]
        public IActionResult Create([FromBody] OrganizationViewModel item)
        {
            if (item == null)
            {
                return BadRequest();
            }
            string userName = User.Claims.GetClaim(JwtClaimTypes.Name);
            _repository.Add(item, userName);
            return CreatedAtRoute("GetOrganization", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] Organization item)
        {
            if (item == null || item.Id != id)
            {
                return BadRequest();
            }

            var todo = _repository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _repository.Update(item);
            return new NoContentResult();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var todo = _repository.Find(id);
            if (todo == null)
            {
                return NotFound();
            }

            _repository.Remove(id);
            return new NoContentResult();
        }


    }
}
