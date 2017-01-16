using GovITHub.Auth.Common.Data;
using GovITHub.Auth.Common.Services.Audit.DataContracts;

namespace GovITHub.Auth.Common.Services.Audit
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext dbContext;

        public AuditService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void LogActionExecuting(AuditActionMessage message)
        {
            dbContext.Add(message);
            dbContext.SaveChanges();
        }
    }
}
