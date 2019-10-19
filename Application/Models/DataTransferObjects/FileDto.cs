using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Application.Models.DataTransferObjects
{
    public class FileDto
    {
        public string Id { get; set; }
        
        public string BucketLink { get; set; }
        public string UserId { get; set; }
    }
}