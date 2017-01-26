using GovITHub.Auth.Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    [Authorize]
    [Route("api/organizations/{organizationId}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly Common.Data.Contract.IOrganizationUserRepository organizationUserRepository;

        public UsersController(Common.Data.Contract.IOrganizationUserRepository organizationUserRepository)
        {
            this.organizationUserRepository = organizationUserRepository;
        }

        [HttpGet]
        public IActionResult Get([FromRoute]long organizationId, [FromQuery]int currentPage, [FromQuery]int itemsPerPage, [FromQuery]bool sortAscending, [FromQuery]string sortBy)
        {
            ModelQueryFilter filter = new ModelQueryFilter(currentPage, itemsPerPage, sortAscending, sortBy);

            return new ObjectResult(organizationUserRepository.Filter(filter, organizationId));
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute]long organizationId, [FromRoute]long id)
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
        public void Post([FromRoute]long organizationId, [FromBody]Models.User user)
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
        public void Put([FromRoute]long organizationId, [FromBody]Models.User user)
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
        public void Delete([FromRoute]long organizationId, [FromRoute]long id)
        {
            organizationUserRepository.Delete(id, organizationId);
        }
    }
}