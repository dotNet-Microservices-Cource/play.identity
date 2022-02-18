using System;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace Play.Identity.Service.Entities
{
  [CollectionName("Role")]
  public class ApplicationRole : MongoIdentityRole<Guid>
  {
    
  }
}