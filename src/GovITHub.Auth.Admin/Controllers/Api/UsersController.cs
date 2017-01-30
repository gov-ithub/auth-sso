using GovITHub.Auth.Common.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GovITHub.Auth.Admin.Controllers.Api
{
    [Authorize(Policy = "LinkedToOrganizationPolicy")]
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

            return new ObjectResult(organizationUserRepository.Filter(organizationId, filter));
        }

        [HttpGet("{id}")]
        public IActionResult Get([FromRoute]long organizationId, [FromRoute]long id)
        {
            Common.Data.Contract.OrganizationUser organizationUser = organizationUserRepository.Find(organizationId, id);

            return Ok(new Models.User()
            {
                Id = organizationUser.Id,
                Level = organizationUser.Level,
                Status = (Models.UserStatus)organizationUser.Status,
                Name = organizationUser.Name
            });
        }

        [HttpPost]
        public IActionResult Post([FromRoute]long organizationId, [FromBody]Models.User user)
        {
            ValidationError validationError = organizationUserRepository.Add(new Common.Data.Contract.OrganizationUser()
            {
                Id = user.Id,
                OrganizationId = organizationId,
                Name = user.Name,
                Level = user.Level,
                Status = (short)user.Status
            });

            if (validationError != null)
                return BadRequest(validationError);

            return Ok();
        }

        [HttpPut("{id}")]
        public IActionResult Put([FromRoute]long organizationId, [FromBody]Models.User user)
        {
            ValidationError validationError = organizationUserRepository.Update(new Common.Data.Contract.OrganizationUser()
            {
                Id = user.Id,
                OrganizationId = organizationId,
                Name = user.Name,
                Level = user.Level,
                Status = (short)user.Status
            });

            if (validationError != null)
                return BadRequest(validationError);

            return Ok();
        }

        [HttpDelete("{id}")]
        public void Delete([FromRoute]long organizationId, [FromRoute]long id)
        {
            organizationUserRepository.Delete(organizationId, id);
        }
    }
}