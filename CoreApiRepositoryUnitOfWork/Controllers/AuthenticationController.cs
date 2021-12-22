using CoreApiRepositoryUnitOfWork.DatabaseContext;
using CoreApiRepositoryUnitOfWork.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CoreApiRepositoryUnitOfWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        public IConfiguration _configuration;
        public readonly ApplicationDbContext _applcationDbContext;



        public AuthenticationController(IConfiguration configuration, ApplicationDbContext applicationDbContext)
        {
            _configuration = configuration;
            _applcationDbContext = applicationDbContext;
        }


        [HttpPost]
        public async Task<IActionResult> Post(User user)
        {
            if (user != null && user.Name != null && user.Password != null)
            {
                var person = await GetUser(user.Name, user.Password);
                if (person != null)
                {

                    // creating a complex jwt token
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id", user.Id.ToString()),
                        new Claim("Name", user.Name),
                        new Claim("Password", user.Password),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.Now.AddMinutes(20),
                        signingCredentials: signIn
                        );

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Invalid Credentials");
                }
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public async Task<User> GetUser(string name, string password)
        {
            return await _applcationDbContext.User.FirstOrDefaultAsync(a => a.Name == name && a.Password == password);
        }
    }
}
