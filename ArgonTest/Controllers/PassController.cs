using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Mvc;
using Sodium;

namespace JF.Password.Controllers
{
    [Route("api/password")]
    [ApiController]
    public class PassController : ControllerBase
    {

        [HttpGet]
        /// <summary>  
        ///  This controller class is used to provide a basic API to demonstrate the Password class
        /// </summary>  
        public string Get()
        {
            // Grab the three header variables if they exist
            var auth = this.Request.Headers["Authorization"].FirstOrDefault();
            var key = this.Request.Headers["x-Key"].FirstOrDefault();
            var strength = this.Request.Headers["x-Strength"].FirstOrDefault();

            // If there is no authorization header, there is no point continuing so throw an error
            if (auth == null)
            {
                throw new HttpRequestException("Missing Authorization Header");
            }
            else
            {
                // Create a new Password object using the Basic Authentication token from the Authorization header as the constructor
                var objPass = new Password(auth);


                // If there's no Encryption key, then use a default one, otherwise use the one from the header variable
                if (key == null)
                {
                    objPass.Key = "t7G9MuBRNc8lb0pfkOif0m6bWipFCQVu";
                }
                else
                {
                    objPass.Key = key;
                }

                // if the Strength variable is provided, then add it to the object
                if (strength != null)
                {
                    objPass.Strength = (PasswordHash.StrengthArgon)Enum.ToObject(typeof(PasswordHash.StrengthArgon), Convert.ToInt16(strength));
                }
               

                // Hash, Encrypt and Test the Password                
                objPass.SecurePassword();

                // Create a serialised output to show the contents of the object and return it to the client
               return objPass.PassOutput();

            }
        }


    }

}
