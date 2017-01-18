using GovITHub.Auth.Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IOrganizationUserRepository organizationUserRepository;

        public UsersController(IOrganizationUserRepository organizationUserRepository)
        {
            this.organizationUserRepository = organizationUserRepository;
        }

        [HttpGet]
        public IActionResult Get([FromQuery]int currentPage, [FromQuery]int itemsPerPage, [FromQuery]bool sortAscending, [FromQuery]string sortBy)
        {
            ModelQueryFilter filter = new ModelQueryFilter(currentPage, itemsPerPage, sortAscending, sortBy);

            return new ObjectResult(organizationUserRepository.Filter(filter));
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {

        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}