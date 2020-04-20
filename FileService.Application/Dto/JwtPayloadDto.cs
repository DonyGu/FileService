using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace FileService.Application.Dto
{
    [DataContract]
    public class JwtPayloadDto
    {
        [DataMember(Name="iss")]
        public string iss { get; set; }
        [DataMember(Name ="scope")]
        public ScopeDto scope { get; set; }
    }
    public class ScopeDto
    {
        [DataMember(Name ="siteId")]
        public int siteId { get; set; }
        [DataMember(Name ="ip")]
        public string ip { get; set; }
        [DataMember(Name ="fileExpireInDays")]
        public int fileExpireInDays { get; set; }
    }
}


