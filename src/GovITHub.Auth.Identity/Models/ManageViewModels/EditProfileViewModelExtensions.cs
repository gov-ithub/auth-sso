using GovITHub.Auth.Common.Models;
using IdentityModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;

namespace GovITHub.Auth.Identity.Models.ManageViewModels
{
    public static class EditProfileViewModelExtensions
    {
        public static EditProfileViewModel ToViewModel(this IEnumerable<Claim> claims)
        {
            EditProfileViewModel model = new EditProfileViewModel();
            model.FirstName = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.FamilyName)?.Value;
            model.LastName = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.GivenName)?.Value;
            string genderValue = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.Gender)?.Value;
            if (!string.IsNullOrWhiteSpace(genderValue))
            {
                Gender gender;
                if (Enum.TryParse(genderValue, true, out gender))
                {
                    model.Gender = gender;
                }
            }
            string birthdateValue = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.BirthDate)?.Value;
            if (!string.IsNullOrWhiteSpace(birthdateValue))
            {
                DateTime birthdate;
                if (DateTime.TryParseExact(birthdateValue, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out birthdate))
                {
                    model.BirthDate = birthdate;
                }
            }
            string address = claims.FirstOrDefault(t => t.Type == JwtClaimTypes.Address)?.Value;
            if (!string.IsNullOrWhiteSpace(address))
            {
                var jsonAddress = new { locality = "", region = "", street_address = "" };
                var deserializedAddress = JsonConvert.DeserializeAnonymousType(address, jsonAddress);
                model.County = deserializedAddress?.region;
                model.City = deserializedAddress?.locality;
                model.StreetAddress = deserializedAddress?.street_address;
            }
            return model;
        }

        public static ICollection<Claim> ToClaims(this EditProfileViewModel model)
        {
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(JwtClaimTypes.FamilyName, model.FirstName));
            claims.Add(new Claim(JwtClaimTypes.GivenName, model.LastName));
            claims.Add(new Claim(JwtClaimTypes.Gender,
                model.Gender.HasValue ? model.Gender.Value.ToString().ToLower() : null));
            claims.Add(new Claim(JwtClaimTypes.BirthDate,
                model.BirthDate.HasValue ? model.BirthDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) : null));
            var addressClaimValue = new { locality = model.City, region = model.County, street_address = model.StreetAddress };
            claims.Add(new Claim(JwtClaimTypes.Address, JsonConvert.SerializeObject(addressClaimValue)));
            return claims;
        }
    }
}
