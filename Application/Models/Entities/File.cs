using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Application.Models.Entities
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string BucketLink { get; set; }
        public string UserId { get; set; }
    }
}