using GovITHub.Auth.Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Common.Data.Contract.IOrganizationUserRepository organizationUserRepository;

        public UsersController(Common.Data.Contract.IOrganizationUserRepository organizationUserRepository)
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
        public IActionResult Get(long id)
        {
            Common.Data.Contract.OrganizationUser organizationUser = organizationUserRepository.Find(id);

            return Ok(new Models.User()
            {
                Id = organizationUser.Id,
                Level = organizationUser.Level,
                Status = (Models.UserStatus)organizationUser.Status,
                Name = organizationUser.Name
            });
        }

        [HttpPost]
        public void Post([FromBody]string value)
        {

        }

        [HttpPut("{id}")]
        public void Put([FromBody]Models.User user)
        {
            organizationUserRepository.Update(new Common.Data.Contract.OrganizationUser()
            {
                Id = user.Id,
                Name = user.Name,
                Level = user.Level,
                Status = (short)user.Status
            });
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}