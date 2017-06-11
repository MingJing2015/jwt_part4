using jwt.Infrastructure;
using jwt.Models;
using jwt.Repositories;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;


// http://bitoftech.net/2015/01/21/asp-net-identity-2-with-asp-net-web-api-2-accounts-management/

namespace jwt.Controllers {
[RoutePrefix("api/accounts")]
public class AccountsController : BaseApiController {

    // Method “GetUsers” will be responsible to return all the registered users in our system 
    // by calling the enumeration “Users” coming from “ApplicationUserManager” class.

    // ？？ For Test JM [Authorize(Roles="Admin")] 
	[Route("users")]
	public IHttpActionResult GetUsers() {
		return Ok(this.AppUserManager.Users.ToList()
                 .Select(u => this.TheModelFactory.Create(u)));
	}

    [Authorize(Roles="Admin")] 
	[Route("user/{id:guid}", Name = "GetUserById")]
	public async Task<IHttpActionResult> GetUser(string Id) {
		var user = await this.AppUserManager.FindByIdAsync(Id);
		if (user != null) {
			return Ok(this.TheModelFactory.Create(user));
		}
		return NotFound();
	}

    [Authorize(Roles="Admin")] 
	[Route("user/{username}")]
	public async Task<IHttpActionResult> GetUserByName(string username) {
		var user = await this.AppUserManager.FindByNameAsync(username);

		if (user != null)
		{
			return Ok(this.TheModelFactory.Create(user));
		}
		return NotFound();
	}

    [Route("create")]
    public async Task<IHttpActionResult> CreateUser(
                 CreateUserBindingModel createUserModel) {

	    if (!ModelState.IsValid) {
		    return BadRequest(ModelState);
	    }

        Email confirmEmail = new Email();

        var existingUser 
            = await this.AppUserManager
            .FindByEmailAsync(createUserModel.Email.ToLower());

            if(existingUser!= null) {
                // 注册邮件存在
                //return BadRequest(ModelState);
                return BadRequest(createUserModel.Email + " - the email has registered !");
            }

	    var user = new ApplicationUser() {
		    UserName = createUserModel.Username,
		    Email = createUserModel.Email,
		    FirstName = createUserModel.FirstName,
		    LastName = createUserModel.LastName,
		    Level = 3,
		    JoinDate = DateTime.Now.Date,
	    };

	    IdentityResult addUserResult = 
        await this.AppUserManager.CreateAsync(user, createUserModel.Password);

	    if (!addUserResult.Succeeded) {
		    return GetErrorResult(addUserResult);
	    }

        string code = await this.AppUserManager.GenerateEmailConfirmationTokenAsync(user.Id);

        var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", 
                          new { userId = user.Id, code = code }));

        await this.AppUserManager.SendEmailAsync(user.Id,"Confirm your account", 
           "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

        Uri locationHeader = new Uri(Url.Link("GetUserById", new { id = user.Id }));

            //Send confirm email by sendgrid.....
            string emailStr = "<h3>" + user.FirstName + "</h3>" + "<p>Please confirm your account by clicking this link: <a href=\""
                + callbackUrl + "\">Confirm Registration</a></p>";

            confirmEmail.FromAddress = "Ming from SendGrid";
            confirmEmail.ToAddress = user.Email;
            confirmEmail.Message = emailStr;
            confirmEmail.Subject = "Register Confirm Email";

            EmailRepo.SendEmail(confirmEmail);

            // From this result and as good practice we should return the resource created in the location header 
            // and return 201 created status.??

            return Created(locationHeader, TheModelFactory.Create(user));
    }

        // This method can be accessed only by authenticated users who belongs to “Admin” role, that’s why we have added
        // the attribute[Authorize(Roles =”Admin”)]
        // The method accepts the UserId in its URI and array of the roles this user Id should be enrolled in.
        // The method will validates that this array of roles exists in the system, if not, 
        // HTTP Bad response will be sent indicating which roles doesn’t exist.
        // The system will delete all the roles assigned for the user then will assign only the roles sent in the request.


    // ?? jm [Authorize(Roles="Admin")]
    [Route("user/{id:guid}/roles")]
    [HttpPut]
    public async Task<IHttpActionResult> AssignRolesToUser([FromUri] string id,
        [FromBody] string[] rolesToAssign) {
	    var appUser = await this.AppUserManager.FindByIdAsync(id);
	    if (appUser == null) {
		    return NotFound();
	    }
	    var currentRoles   = await this.AppUserManager.GetRolesAsync(appUser.Id);
	    var rolesNotExists = rolesToAssign.Except(this.AppRoleManager
            .Roles.Select(x => x.Name)).ToArray();

	    if (rolesNotExists.Count() > 0) {
		    ModelState.AddModelError("", 
                string.Format("Roles '{0}' does not exixts in the system", 
                string.Join(",", rolesNotExists)));
		    return BadRequest(ModelState);
	    }

	    IdentityResult removeResult = await 
            this.AppUserManager.RemoveFromRolesAsync(appUser.Id, currentRoles.ToArray());
	    if (!removeResult.Succeeded) {
		    ModelState.AddModelError("", "Failed to remove user roles");
		    return BadRequest(ModelState);
	    }

	    IdentityResult addResult 
        = await this.AppUserManager.AddToRolesAsync(appUser.Id, rolesToAssign);
        //= await this.AppUserManager.AddToRolesAsync(appUser.Id, "Admin");

	    if (!addResult.Succeeded) {
		    ModelState.AddModelError("", "Failed to add user roles");
		    return BadRequest(ModelState);
	    }
	    return Ok();
    }

    [Authorize]
    [Route("ChangePassword")]
    public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model) {
        if (!ModelState.IsValid) {

                //return BadRequest(ModelState);
                return BadRequest("Two password are not same !");

            }

        IdentityResult result = await this.AppUserManager
       .ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

        if (!result.Succeeded) {
            //return GetErrorResult(result);
            return BadRequest("Old Password is not correct !");
        }
        return Ok();
    }

        //[Authorize]  ???
        //[Route("Logout")]
        //public async Task<IHttpActionResult> Logout()
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    IdentityResult result = await this.AppUserManager
        //   .ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);

        //    if (!result.Succeeded)
        //    {
        //        return GetErrorResult(result);
        //    }
        //    return Ok();
        //}


    // ??JM 17-05 Should not add [Authorize(Roles="Admin")]
    [HttpGet]
    [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
    public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "") {
        if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code)) {
            ModelState.AddModelError("", "User Id and Code are required");
            return BadRequest(ModelState);
        }

        IdentityResult result = await this.AppUserManager.ConfirmEmailAsync(userId, code);
        if (result.Succeeded) {
            return Ok();
        }
        else {
            return GetErrorResult(result);
        }
    }

        [HttpGet]
        [Route("deleteuser/{id:guid}")]
        public async Task<IHttpActionResult> DeleteUser(string id)
        {
            //Only SuperAdmin or Admin can delete users (Later when implement roles)
            var appUser = await this.AppUserManager.FindByIdAsync(id);

            if (appUser != null)
            {
                IdentityResult result = await this.AppUserManager.DeleteAsync(appUser);

                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                return Ok();
            }
            return NotFound();
        }
    }
}
