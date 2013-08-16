using System;
using Serenity.Data;

namespace Serenity.Services
{
    public class DeleteResponse : ServiceResponse
    {
        public bool WasAlreadyDeleted;
    }
}