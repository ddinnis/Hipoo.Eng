﻿using Common.Domain.Models;
using Listening.Domain.Entities;

namespace Listening.Main.WebAPI.Controllers.Albums
{
    public record AlbumVM(Guid Id, MultilingualString Name, Guid CategoryId)
    {
        public static AlbumVM? Create(Album? a)
        {
            if (a == null)
            {
                return null;
            }
            return new AlbumVM(a.Id, a.Name, a.CategoryId);
        }

        public static AlbumVM[] Create(Album[] items)
        {
            return items.Select(e => Create(e)!).ToArray();
        }
    }

}
