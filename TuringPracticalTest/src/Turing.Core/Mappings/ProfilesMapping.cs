using AutoMapper;
using Turing.Core.Models;
using Turing.Data.Entities;

namespace Turing.Core.Mappings
{
    public class ProfilesMapping : Profile
    {
        public ProfilesMapping()
        {
            CreateMap<User, UserProfileModel>(MemberList.Destination)
                .ForMember(d => d.Following, opts => opts.Ignore());
        }
    }
}
