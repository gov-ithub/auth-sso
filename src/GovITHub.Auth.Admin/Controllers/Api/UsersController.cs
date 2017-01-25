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
        public IActionResult Get([FromQuery]int currentPage, [FromQuery]int itemsPerPage, [FromQuery]bool sortAscending, [FromQuery]string sortBy, [FromQuery]long organizationId)
        {
            ModelQueryFilter filter = new ModelQueryFilter(currentPage, itemsPerPage, sortAscending, sortBy);

            return new ObjectResult(organizationUserRepository.Filter(filter, organizationId));
        }

        [HttpGet("{id}")]
        public IActionResult Get(long id, [FromQuery]long organizationId)
        {
            Common.Data.Contract.OrganizationUser organizationUser = organizationUserRepository.Find(id, organizationId);

            return Ok(new Models.User()
            {
                Id = organizationUser.Id,
                Level = organizationUser.Level,
                Status = (Models.UserStatus)organizationUser.Status,
                Name = organizationUser.Name
            });
        }

        [HttpPost]
        public void Post([FromBody]Models.User user, [FromQuery]long organizationId)
        {
            organizationUserRepository.Add(new Common.Data.Contract.OrganizationUser()
            {
                Id = user.Id,
                Name = user.Name,
                Level = user.Level,
                Status = (short)user.Status
            }, organizationId);
        }

        [HttpPut("{id}")]
        public void Put([FromBody]Models.User user, [FromQuery]long organizationId)
        {
            organizationUserRepository.Update(new Common.Data.Contract.OrganizationUser()
            {
                Id = user.Id,
                Name = user.Name,
                Level = user.Level,
                Status = (short)user.Status
            }, organizationId);
        }

        [HttpDelete("{id}")]
        public void Delete([FromRoute]long id, [FromQuery]long organizationId)
        {
            organizationUserRepository.Delete(id, organizationId);
        }
    }
}