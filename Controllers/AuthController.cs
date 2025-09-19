using Application1.Data;
using Application1.DTOs;
using Application1.Models.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Application1.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AuthController : Controller
    {
        private readonly IConfiguration _config;
        private readonly DataContextDappper _dapper;

        public AuthController(IConfiguration configuration)
        {
            _config = configuration;
            _dapper = new DataContextDappper(configuration);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public ActionResult Register([FromBody] StudentDTO student) {
            if(student.Password == student.ConfirmPassword)
            {
                var userCheckSql = "SELECT Email FROM [StudentsDb].[dbo].[Students] WHERE Email='" + student.Email + "'";
                IEnumerable<string> existsUser = _dapper.LoadData<string>(userCheckSql);
                if (existsUser.Count() == 0) 
                {
                    byte[] passwordSalt = new byte[128/8];
                    using(RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = GetPassWordHash(student.Password, passwordSalt);

                    var insertStudentSql = @"INSERT INTO [StudentsDb].[dbo].[Students] ([Name], [Email], [Standard], [IsActive], [PasswordSalt], [PasswordHash])                
                        VALUES (@Name, @Email, @Standard, @IsActive, @PasswordSalt, @PasswordHash)";

                    var parameters = new
                    {
                        Name = student.Name,
                        Email = student.Email,
                        Standard = student.Standard,
                        IsActive = 1,
                        PasswordSalt = passwordSalt,
                        PasswordHash = passwordHash,
                    };

                    var result = _dapper.ExecuteSqlWithParams( insertStudentSql, parameters );
                    if (result)
                    {
                        return Ok("Registration successfull.");
                    }else
                    {
                        return BadRequest("Unable to register");
                    }
                }
                return BadRequest("User with this email already exists.");
            }
            return BadRequest("Password and confirm password should be same.");
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult Login([FromBody] LoginDTO details) {
            var sql = @"SELECT [Email], [PasswordHash] ,[PasswordSalt]
                        FROM [StudentsDb].[dbo].[Students] WHERE EMAIL='" + details.Email + "'";
            LoginDetailsDTO result = _dapper.LoadDataSingle<LoginDetailsDTO>(sql);
            if(result == null)
            {
                return BadRequest("User doesn't exists.");
            }
            var passwordHash = GetPassWordHash(details.Password,result.PasswordSalt);
            //if(passwordHash == result.PasswordHash) //Doesn't work bcoz it compare address
            for(int index=0; index<passwordHash.Length; index++)
            {
                if (passwordHash[index] != result.PasswordHash[index])
                {
                    return Unauthorized("Incorrect password.");
                }
            }

            string getUseIdSql = "SELECT [Id], [IsAdmin] FROM [StudentsDb].[dbo].[Students] WHERE EMAIL='" + details.Email + "'";
            var userDetails = _dapper.LoadDataSingle <AuthDTO>(getUseIdSql);

            return Ok(new Dictionary<string, string> {
                {
                    "token", CreateToken(userDetails.Id, userDetails.IsAdmin)
                }
            });
        }

        [HttpGet("/RefreshToken")]
        public IActionResult RefreshToken()
        {
            string userId = User.FindFirst("userId")?.Value + "";

            var sql = "SELECT [Id], [IsAdmin] FROM [StudentsDb].[dbo].[Students] WHERE Id=" + userId;
            AuthDTO userfrmDB = _dapper.LoadDataSingle<AuthDTO>(sql);

            return Ok(new Dictionary<string, string >{
                { "token", CreateToken(userfrmDB.Id, userfrmDB.IsAdmin) }
            });
        }

        private byte[] GetPassWordHash(string password, byte[] passwordSalt)
        {
            var passSaltAndString = _config.GetSection("AppSetting:passwordKey").Value + Convert.ToBase64String(passwordSalt);
            return KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passSaltAndString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000,
                numBytesRequested: 256 / 8
            );
        }

        private string CreateToken(int id, bool isAdmin)
        {
            Claim[] claims = new Claim[]{
                new Claim("userId", id.ToString()),
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            };
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSetting:TokenKey").Value)
            );

            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature);

            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor() 
            {
               Subject = new ClaimsIdentity(claims),
               SigningCredentials = credentials,
               Expires = DateTime.UtcNow.AddDays(1),
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();   
            SecurityToken token = tokenHandler.CreateToken(descriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
