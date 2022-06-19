using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using school_web_api.Data;
using school_web_api.Data.Model;
using school_web_api.Data.ViewModels;
using System.Threading.Tasks;
using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace school_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> usermanager, RoleManager<IdentityRole> roleManager, AppDbContext context, IConfiguration configuration , TokenValidationParameters tokenValidationParameters)
        {
            _usermanager = usermanager;
            _roleManager = roleManager;
            _context = context;
            this.configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }
        [HttpPost("register-user")]
        public  async Task<IActionResult> Register([FromBody]RegisterVM registerVm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please Provide All The Required Fields");
            }
            var userExists = await _usermanager.FindByEmailAsync(registerVm.EmailAddress);
           
            if (userExists != null) 
            {
                return BadRequest($"user {registerVm.EmailAddress} already exists");
            }
            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerVm.FirstName,
                lastName = registerVm.LastName,
                Email = registerVm.EmailAddress,
                UserName = registerVm.UserName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var result = await _usermanager.CreateAsync(newUser, registerVm.Passsword);
            if(result.Succeeded) return Ok("User Successfully Registerd");
            return BadRequest("User Could Not be Registerd");

        }
        [HttpPost("user-Login")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please Provide All required Fields");
            }

            var userExists = await _usermanager.FindByEmailAsync(loginVM.EmailAddress);
            if (userExists != null && await _usermanager.CheckPasswordAsync(userExists, loginVM.Passsword))
            {
                var tokenValue = await GenerateJWTTokenAsync(userExists , null);
                return Ok(tokenValue);
            }
            return Unauthorized();
        }

        [HttpPost("user-Login")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM tokenRequestVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please Provide All required Fields");
            }
            var result = await VerifyAndGenerateTokenAsyc(tokenRequestVM);
            return Ok(result);

         
        }

        private async Task<AuthResultVM> VerifyAndGenerateTokenAsyc(TokenRequestVM tokenRequestVM)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var storedToken = await _context.RefrestTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreToken);
            var dbuser = await _usermanager.FindByIdAsync(storedToken.UserId);
            try
            {
                var tokenCheckResult = jwtTokenHandler.ValidateToken(tokenRequestVM.Token, _tokenValidationParameters,
                    out var validatedtoken);

                return await GenerateJWTTokenAsync(dbuser, storedToken);
            }
            catch(SecurityTokenExpiredException)
            { 
                if(storedToken.DateExpire >= DateTime.UtcNow)
                {
                    return await GenerateJWTTokenAsync(dbuser, storedToken);
                }
                else
                {
                 return   await  GenerateJWTTokenAsync(dbuser, null);
                }
            }
           
        }
        private async Task<AuthResultVM> GenerateJWTTokenAsync(ApplicationUser user , RefrestToken rToken)
        {
            var AuthClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name ,user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email , user.Email),
                new Claim(JwtRegisteredClaimNames.Sub , user.Email),
                new Claim(JwtRegisteredClaimNames.Jti , Guid.NewGuid().ToString())
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(this.configuration["JWT:secret"]));

            var token = new JwtSecurityToken(
                issuer: this.configuration["JWT:Issuer"],
                audience: this.configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1),
                claims: AuthClaims,
                signingCredentials: new SigningCredentials( authSigningKey ,SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefrestToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString()
            };
            await _context.RefrestTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            var response = new AuthResultVM()
            {
                Token = jwtToken,
                Refreshtoken= refreshToken.Token,
                ExpiresAt = token.ValidTo
            };

            return response;
        }
    }
   
}
