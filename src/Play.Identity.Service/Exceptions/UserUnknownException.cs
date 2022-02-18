using System;

namespace Play.Identity.Service.Exceptions
{
  [Serializable]
  internal class UserUnknownException : Exception
  {
    public UserUnknownException(Guid userId) :
      base($"Unknown user '{userId}'")
    {
      this.UserId = userId;
    }

    public Guid UserId { get;}

  }
}