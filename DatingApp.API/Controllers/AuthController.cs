using Microsoft.AspNetCore.Mvc;
using DatingApp.API.Data;
using DatingApp.API.Model;
using System.Threading.Tasks;
using DatingApp.API.Dtos;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace DatingApp.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
        public class AuthController :ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo,IConfiguration config){
            this._repo = repo;
            this._config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisteDto){
            
           
          userForRegisteDto.Username=userForRegisteDto.Username.ToLower();

           if(await _repo.UserExists(userForRegisteDto.Username)){
               return BadRequest("Username already exists!");
           }

           var userToCreate = new User{
               UserName = userForRegisteDto.Username

           };

             var createdUser = await _repo.Register(userToCreate,userForRegisteDto.Password);
            
            return StatusCode(201);
        }

    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto _userForLoginDto ){

        var userFromRepo= await _repo.Login(_userForLoginDto.UserName.ToLower(),_userForLoginDto.Password);

        if(userFromRepo==null)
          return Unauthorized();

          var claims = new[]{
              new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
              new Claim(ClaimTypes.Name,userFromRepo.UserName)

          };
          var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_config.GetSection("AppSettings:Token").Value));

         var creds=new SigningCredentials(key,SecurityAlgorithms.HmacSha512Signature);

         var tokenDescriptor = new SecurityTokenDescriptor{
           Subject = new ClaimsIdentity(claims),
           Expires= DateTime.Now.AddDays(1),
           SigningCredentials = creds
           };
         
         var tokenHandler = new JwtSecurityTokenHandler();

         var token = tokenHandler.CreateToken(tokenDescriptor);

         return Ok(new{
             token = tokenHandler.WriteToken(token)
         });
    }



    }
}