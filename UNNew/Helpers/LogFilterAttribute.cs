using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using UNNew.Models;

namespace UNNew.Helpers
{
    public class LogFilterAttribute : ActionFilterAttribute
    {
        static public int? UserId { get; set; }
        private readonly ILogger<LogFilterAttribute> _logger;
        private readonly UNDbContext _dbContext;
        private readonly IServiceProvider _serviceProvider;
        public static List<ActionParamaters> MyParametersList { get; set; } = new List<ActionParamaters>();

        public class ActionParamaters
        {
            public Guid GuidId { get; set; }
            public IDictionary<string, object> Parameters { get; set; }
        }

        // تغيير: حقن IServiceProvider بدلاً من IServiceCollection
        public LogFilterAttribute(ILogger<LogFilterAttribute> logger, UNDbContext context, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _dbContext = context;
            _serviceProvider = serviceProvider;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Guid GuidId = Guid.Parse(filterContext.HttpContext.TraceIdentifier);
            Trace.CorrelationManager.ActivityId = GuidId;
            IDictionary<string, object> Parameters = filterContext.ActionArguments;
            MyParametersList.Add(new ActionParamaters
            {
                GuidId = GuidId,
                Parameters = Parameters
            });

            string token = filterContext.HttpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token) && token.ToLower() != "bearer null")
                UserId = Int32.Parse(new JwtSecurityTokenHandler().ReadJwtToken(token.Split(" ")[1]).Claims.ToList()[0].Value);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            try
            {
                var UserName = _dbContext.Users.FirstOrDefault(x => x.Id == UserId)?.UserName;
                LogAction NewLog = new LogAction();

                // 1. Date
                NewLog.ActDate = DateTime.UtcNow;

                // 2. User Id
                if (UserId != null)
                    NewLog.UserName = UserName;

                // 3. Controller Name
                List<object> Controller_Function_Name = filterContext.RouteData.Values.Values.ToList();
                NewLog.ControllerName = Controller_Function_Name[1].ToString();

                // 4. Action Name
                NewLog.ActionName = Controller_Function_Name[0].ToString();

                // 5. Body Parameters
                IDictionary<string, object> Parameters = MyParametersList
                    .Where(x => x.GuidId == Guid.Parse(filterContext.HttpContext.TraceIdentifier))
                    .Select(x => x.Parameters)
                    .FirstOrDefault();
                NewLog.BodyParameters = Newtonsoft.Json.JsonConvert.SerializeObject(Parameters);

                // 6. Header Parameters
                NewLog.HeaderParameters = Newtonsoft.Json.JsonConvert.SerializeObject(filterContext.HttpContext.Request.Headers);

                // 7. Handle Exception (if any)
                Exception Exceptions = filterContext.Exception;
                if (Exceptions != null)
                {
                    NewLog.ResponseStatus = "Failed";
                    NewLog.Result = Exceptions.Message;
                }
                else
                {
                    IActionResult ActionResult = filterContext.Result;
                    if (ActionResult is OkObjectResult json)
                    {
                        dynamic DynamicObject = new ExpandoObject();
                        DynamicObject = json.Value;
                        if (!string.IsNullOrEmpty(DynamicObject.Errors))
                        {
                            NewLog.ResponseStatus = "Failed";
                            NewLog.Result = DynamicObject.Errors;
                        }
                        else
                        {
                            NewLog.ResponseStatus = "Success";
                            NewLog.Result = Newtonsoft.Json.JsonConvert.SerializeObject(DynamicObject);
                        }
                    }
                }

                // 8. Save Log
                _dbContext.LogActions.Add(NewLog);
                _dbContext.SaveChanges();
            }
            catch (Exception err)
            {
                _logger.LogError(err.Message);
            }
        }
    }
}
